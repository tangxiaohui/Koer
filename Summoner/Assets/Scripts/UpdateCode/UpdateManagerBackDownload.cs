using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UpdateSystem.Data;
using UpdateSystem.Delegate;
using UpdateSystem.Download;
using UpdateSystem.Enum;
using UpdateSystem.Flow;
using UpdateSystem.Log;

namespace UpdateSystem.Manager
{
    /// <summary>
    /// 管理后台更新
    /// </summary>
    public partial class UpdateManager : SingleInstance<UpdateManager>
    {
        #region //Instance
        private Flow8ExCheckResource _instance;
        private Flow8ExCheckResource InstanceFlow8Ex
        {
            get
            {
                if (_instance == null) _instance = FlowInstance<Flow8ExCheckResource>();
                return _instance;
            }
        }
        #endregion

        //总的静默更新资源大小
        private int _totalBaseResSize = 0;
        //场景资源列表
        private Dictionary<DataLevel, List<MapFileData>> _sceneResList = new Dictionary<DataLevel, List<MapFileData>>();

        //存储下载完成的文件
        private Dictionary<string, int> _downloadedFileDic = new Dictionary<string, int>();

        private void AddDataList(IEnumerable dataList, DataLevel level, bool append = false)
        {
            if (dataList != null && !append)
            {
                BackDownload.ClearData(level);
                if (_sceneResList.ContainsKey(level) && _sceneResList[level] != null)
                {
                    _sceneResList[level].Clear();
                }
            }
            Flow8ExCheckResource resource = InstanceFlow8Ex;
            string fixPath = "";
            IEnumerator enumerator = dataList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                //不存在的文件才加入到下载队列
                if (!Exist(enumerator.Current as string, out fixPath))
                {
                    lock (BackDownload.GetLocker())
                    {
                        MapFileData mapFileData = resource.GetMapFileDataByPath(fixPath);
                        if (((mapFileData == null) || mapFileData.Downloading) || mapFileData.Downloaded)
                        {
                            UpdateLog.WARN_LOG("no map data: " + fixPath);
                        }
                        else
                        {
                            mapFileData.FullPath = fixPath;
                            if (mapFileData.DataLevel > level)
                            {
                                mapFileData.DataLevel = level;
                            }
                            BackDownload.AddDataToPool(mapFileData, level);
                            //添加到当前场景资源列表，用于统计大小，显示进度
                            AddToCurSceneResList(level, mapFileData);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 添加到当前场景资源
        /// </summary>
        /// <param name="level"></param>
        /// <param name="data"></param>
        private void AddToCurSceneResList(DataLevel level, MapFileData data)
        {
            if (level == DataLevel.CurScene)
            {
                if (!_sceneResList.ContainsKey(level))
                {
                    var dataList = new List<MapFileData>();
                    dataList.Add(data);
                    _sceneResList.Add(level, dataList);
                }
                else
                {
                    _sceneResList[level].Add(data);
                }
            }
        }

        /// <summary>
        /// 本地存在或者map中存在，都视为有效
        /// </summary>
        /// <param name="absolutPath"></param>
        /// <returns></returns>
        public bool IsValid(string absolutPath)
        {
            string fixPath = absolutPath;
            MapFileData data = null;

            bool localExist = IsExistOrDownloaded(absolutPath, out data, out fixPath);
            return localExist || data != null;
        }


        /// <summary>
        /// 游戏内调用，用于判断某场景是否存在
        /// 如果不存在，则需要提示4G玩家等待
        /// </summary>
        /// <param name="absolutPath"></param>
        /// <returns></returns>
        public bool IsExist(string absolutPath)
        {
            string fixPath = absolutPath;
            MapFileData data = null;

            return IsExistOrDownloaded(absolutPath, out data, out fixPath);
        }

        /// <summary>
        /// 判断是否正在下载中，如果是，则不做提示
        /// </summary>
        /// <param name="absolutPath"></param>
        /// <returns></returns>
        public bool IsExist(string absolutPath, out bool downloading)
        {
            downloading = false;
            string fixPath = absolutPath;
            MapFileData data = null;
            bool ret = IsExistOrDownloaded(absolutPath, out data, out fixPath);
            if (data != null)
            {
                downloading = data.Downloading;
            }

            return ret;
        }

        /// <summary>
        /// 下载资源
        /// </summary>
        /// <param name="absolutPath"></param>
        /// <param name="callback"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool DownloadResource(string absolutPath, UpdateAction<string, bool, object> callback, object obj)
        {
            string fixPath = absolutPath;
            MapFileData data = null;

            if (!IsExistOrDownloaded(absolutPath, out data, out fixPath))
            {
                if (data != null)
                {
                    lock (BackDownload.GetLocker())
                    {
                        //暂停中，则不接受请求，原样返回
                        if (IsPaused())
                        {
                            UpdateLog.WARN_LOG("当前是暂停状态，直接返回请求：" + absolutPath);
                            data.DownloadCallBack = null;
                            data.ArgObj = null;
                            callback(fixPath, false, obj);
                            return true;
                        }
                        else
                        {
                            data.DownloadCallBack = callback;
                            data.ArgObj = obj;
                            data.DataLevel = DataLevel.High;
                        }
                    }

                    if (!data.Downloading)
                    {
                        BackDownload.AddDataToPool(data, DataLevel.High);
                    }
                    else
                        UpdateLog.WARN_LOG("resource is downloading: " + fixPath);
                }
                else
                {
                    //UpdateLog.WARN_LOG("no map data found: " + absolutPath);
                    callback(absolutPath, false, obj);
                    return true;
                }
            }
            else
            {
                callback(fixPath, true, obj);
            }

            return false;
        }

        /// <summary>
        /// 获取当前场景下载信息
        /// </summary>
        /// <param name="total"></param>
        /// <param name="downloaded"></param>
        public void GetCurSceneTotalResSize(out long total, out long downloaded)
        {
            downloaded = 0;
            total = 0;
            try
            {
                if (_sceneResList.ContainsKey(DataLevel.CurScene))
                {
                    var resList = _sceneResList[DataLevel.CurScene];
                    for (int i = 0; i < resList.Count; ++i)
                    {
                        if (resList[i].Downloaded ||
                            (!resList[i].Downloading && File.Exists(resList[i].FullPath)))
                        {
                            downloaded += resList[i].FileSize;
                        }
                        total += resList[i].FileSize;
                    }
                }
            }
            catch (System.Exception ex)
            {
                UpdateLog.ERROR_LOG(ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 暂停所有
        /// </summary>
        public void PauseAll()
        {
            BackDownload.PauseAll();
        }

        /// <summary>
        /// 恢复所有
        /// </summary>
        public void ResumeAll()
        {
            BackDownload.ResumeAll();
        }

        /// <summary>
        /// 暂停后台下载
        /// </summary>
        public void PauseBaseDownload()
        {
            BackDownload.PauseLevel(DataLevel.Low);
        }

        /// <summary>
        /// 恢复后台下载，与pause成对
        /// </summary>
        public void ResumeBaseDownload()
        {
            BackDownload.ResumeLevel(DataLevel.Low);
        }

        /// <summary>
        /// 暂停当前场景下载
        /// </summary>
        public void PauseCurSceneDownload()
        {
            BackDownload.PauseLevel(DataLevel.CurScene);
        }

        /// <summary>
        /// 恢复当前场景下载
        /// </summary>
        public void ResumeCurSceneDownload()
        {
            BackDownload.ResumeLevel(DataLevel.CurScene);
        }

        public bool IsPaused()
        {
            return BackDownload.IsPaused();
        }

        /// <summary>
        /// 是否暂停静默更新
        /// </summary>
        /// <returns></returns>
        public bool IsBaseResPaused()
        {
            return BackDownload.IsBaseResPaused();
        }

        /// <summary>
        /// 是否暂停当前场景更新
        /// </summary>
        /// <returns></returns>
        public bool IsCurScenePaused()
        {
            return BackDownload.IsCurScenePaused();
        }

        /// <summary>
        /// 当前场景资源是否下载完了
        /// 需要判断队列中是否存在和是否正在下载
        /// </summary>
        /// <returns></returns>
        public bool IsCurSceneDownloadFinish()
        {
            int curCount = BackDownload.GetDataCount(DataLevel.CurScene);
            bool downloading = BackDownload.IsDateLevelDownloading(DataLevel.CurScene);
            UpdateLog.DEBUG_LOG(string.Format("当前场景资源个数剩余：{0} {1} ，是否还有正在下载的：{2}", curCount, DataLevel.CurScene, downloading));
            return curCount == 0 && downloading == false;
        }

        /// <summary>
        /// 是否静默更新下载完了
        /// </summary>
        /// <returns></returns>
        public bool IsBaseResDownloadFinish()
        {
            return BackDownload.GetDataCount(DataLevel.Low) == 0;
        }

        /// <summary>
        /// 本身存在或者下载完成的文件
        /// </summary>
        /// <param name="absolutPath"></param>
        /// <param name="data"></param>
        /// <param name="fixPath"></param>
        /// <returns></returns>
        internal bool IsExistOrDownloaded(string absolutPath, out MapFileData data, out string fixPath)
        {
            bool ret = false;
            fixPath = absolutPath;
            data = null;

            //如果是包内路径
            if (!string.IsNullOrEmpty(_appPath) && (absolutPath.IndexOf(_appPath) >= 0))
            {
                //拼接存储路径
                string path = absolutPath.Replace(_appPath, _storePath);
                //包内存在文件，存储路径不存在文件，直接返回
                if (!File.Exists(path) && File.Exists(absolutPath))
                {
                    ret = true;
                    return ret;
                }

                //将路径转换成存储路径
                fixPath = path;
                absolutPath = fixPath;
            }
            
            {
                //已经下载好了，直接返回
                if (_downloadedFileDic.ContainsKey(absolutPath))
                {
                    ret = true;
                }
                else
                {
                    //获取对应的MapData数据
                    MapFileData mapFileDataByPath = InstanceFlow8Ex.GetMapFileDataByPath(absolutPath);
                    if (mapFileDataByPath != null)
                    {
                        bool skipDownload = false;

                        //lock (BackDownload.GetLocker())
                        {
                            //没有在下载中，并且存在这个文件，直接返回
                            if (!mapFileDataByPath.Downloading && File.Exists(absolutPath))
                            {
                                _downloadedFileDic.Add(absolutPath, 0);
                                skipDownload = true;
                            }
                            //下载完成状态，直接返回
                            else if (mapFileDataByPath.Downloaded)
                            {
                                _downloadedFileDic.Add(absolutPath, 0);
                                skipDownload = true;
                            }
                            else
                            {
                                //没下载或下载中状态，添加到高优先级的下载队列
                                data = mapFileDataByPath;
                            }
                        }
                        ret = skipDownload;
                    }
                    else if (File.Exists(absolutPath))
                    {
                        //没有找到MapData数据，但是文件存在，依然返回true
                        ret = true;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// 后台更新当前场景资源
        /// </summary>
        /// <param name="normalList"></param>
        /// <param name="append"></param>
        internal void BackDownloadCurSceneData(string[] normalList, bool append = false)
        {
            UpdateLog.WARN_LOG("Add cur scene resources");
            AddDataList(normalList, DataLevel.CurScene, append);
        }

        /// <summary>
        /// 后台更新下一场景资源
        /// </summary>
        /// <param name="preLoadList"></param>
        /// <param name="append"></param>
        internal void BackDownloadNextSceneData(string[] preLoadList, bool append = false)
        {
            AddDataList(preLoadList, DataLevel.NextScene, append);
        }

        /// <summary>
        /// 下载场景资源
        /// </summary>
        /// <param name="mapID">要下载的场景ID</param>
        /// <param name="current">是否当前场景</param>
        public void BackDownloadSceneData(int mapID, bool current = false, bool append = false)
        {
            string str = string.Format("{1}/{0}.txt", mapID, _sceneConfigPath);
            string path = _storePath + "/" + str;
            if (File.Exists(path))
            {
                UpdateLog.WARN_LOG("download scene resource: " + str);
                string[] normalList = File.ReadAllLines(path);
                if (current)
                {
                    BackDownloadCurSceneData(normalList, append);
                }
                else
                {
                    BackDownloadNextSceneData(normalList, append);
                }
            }
            else
            {
                UpdateLog.WARN_LOG("找不到文件： " + path);
            }
        }

        /// <summary>
        /// 静默后台更新所有资源
        /// </summary>
        public void BackDownloadTotalData()
        {
            Flow8ExCheckResource resource = InstanceFlow8Ex;
            if (resource.BackDownloadList != null)
            {
                for (int i = 0; i < resource.BackDownloadList.Count; i++)
                {
                    BackDownload.AddDataToPool(resource.BackDownloadList[i], DataLevel.Low);
                    _totalBaseResSize += resource.BackDownloadList[i].FileSize;
                }
            }
        }

        /// <summary>
        /// 总的后台下载资源大小
        /// </summary>
        /// <returns></returns>
        public int GetTotalBaseResSize()
        {
            return _totalBaseResSize;
        }

        /// <summary>
        /// 总的已下载大小
        /// </summary>
        /// <returns></returns>
        public int GetTotalDownloadedSize()
        {
            return BackDownload.TotalDownloadedSize;
        }

        /// <summary>
        /// 获取下载速度
        /// </summary>
        /// <returns></returns>
        public int GetDownloadSpeed()
        {
            return Download.Download.GetDownloadSpeed();
        }

        /// <summary>
        /// 初始化后台更新
        /// </summary>
        internal void InitBackDownload(int halfCpuCoreCount = 4)
        {
            //初始化后台更新模块
            Download.BackDownload.InitPool(halfCpuCoreCount, convertMyActionCall(delegate(string arg1, bool arg2, object arg3)
            {
                MapFileData data = arg3 as MapFileData;
                if (!arg2)
                {
                    UpdateLog.ERROR_LOG("下载失败： " + arg1);
                }
                var func = data.DownloadCallBack;
                //需要设置为null，不然重复下载的文件会出问题
                data.DownloadCallBack = null;

                if (func != null)
                {
                    func(arg1, arg2, data.ArgObj);
                }

                data.Downloading = false;
                data.Downloaded = arg2;
            }));
        }

        private bool Exist(string path, out string fixPath)
        {
            if (path.IndexOf(_storePath) == 0)
            {
                fixPath = path;
            }
            else
            {
                fixPath = Path.Combine(_storePath, path).Replace(@"\", "/");
            }
            return File.Exists(fixPath);
        }
    }
}

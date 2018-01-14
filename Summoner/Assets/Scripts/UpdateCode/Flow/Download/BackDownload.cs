namespace UpdateSystem.Download
{
    using UpdateSystem.Data;
    using UpdateSystem.Delegate;
    using UpdateSystem.Flow;
    using UpdateSystem.Log;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class BackDownload
    {
        private const int _MAX_THREAD_COUNT = 4;
        private const int _RETRAY_TIMES = 3;
        private static UpdateAction<string, bool, object> _finishDownloadCallback = null;
        private static List<UpdateSystem.Download.Download> _httpInsList = new List<UpdateSystem.Download.Download>();
        private static bool _init = false;
        private static string _TAG = "BackDownload.cs ";
        private static ThreadMapDataPool _threadPool = null;
        //线程池的同步锁
        private static object m_locker;
        //下载完成后需要线程同步
        private static object m_downloadedLocker = new object();

        //总下载文件大小
        public static int TotalDownloadedSize;

        internal static void InitPool(int cpuCoreCount, UpdateAction<string, bool, object> callback)
        {
            if (!_init)
            {
                _init = true;
                _finishDownloadCallback = callback;
                Download.ResetDownloadedSize();
                _threadPool = new ThreadMapDataPool(cpuCoreCount, (BackDownload.DownloadAction));
                m_locker = _threadPool.Locker;
            }
        }

        /// <summary>
        /// 中断下载
        /// </summary>
        /// <param name="callback"></param>
        internal static void AbortAll(AbortFinishCallback callback)
        {
            UpdateLog.DEBUG_LOG("Abort back download!!!");
            lock (m_locker)
            {
                if (_threadPool != null)
                {
                    _threadPool.WaitForFinish();
                }
                for (int i = 0; i < _httpInsList.Count; i++)
                {
                    _httpInsList[i].Abort(null);
                }
            }
            if (callback != null)
            {
                callback(true);
            }
        }

        /// <summary>
        /// 添加数据到下载队列， level：队列类型
        /// </summary>
        /// <param name="data"></param>
        /// <param name="level"></param>
        internal static void AddDataToPool(MapFileData data, DataLevel level)
        {
            _threadPool.AddData(data, level);
        }

        /// <summary>
        /// 添加失败的数据到下载队列， 如果是因为中断操作造成的，则添加到之前的下载队列中
        /// 如果是正常失败，则添加到失败队列，在所有资源下载完成后在重新下载
        /// </summary>
        /// <param name="data"></param>
        /// <param name="byAbort"></param>
        internal static void AddFailDataToPool(MapFileData data)
        {
            if (data.ErrorCode == CodeDefine.RET_SKIP_BY_ABORT)
            {
                //只有静默更新的资源才放回失败列表
                if (data.DataLevel == DataLevel.Low)
                {
                    _threadPool.AddData(data, data.DataLevel, true);
                }
            }
            else
            {
                //_threadPool.AddFailData(data);
            }
        }

        /// <summary>
        /// 添加下载实例，正在执行下载的线程
        /// </summary>
        /// <param name="ins"></param>
        private static void AddIns(UpdateSystem.Download.Download ins)
        {
            lock (m_locker)
            {
                _httpInsList.Add(ins);
            }
        }

        internal static void ClearData(DataLevel level)
        {
            _threadPool.ClearData(level);
        }

        private static void DownloadAction(MapFileData state)
        {
            MapFileData fileData = state;
            fileData.ErrorCode = CodeDefine.RET_INIT;

            string saveFilePath = fileData.SaveDir + "/" + fileData.Dir + fileData.Name;
            int num = DownloadMapData(fileData, saveFilePath);
            string str2 = MD5.MD5File(saveFilePath);
            bool success = (num >= CodeDefine.RET_SUCCESS) && fileData.Md5.Equals(str2);
            if (!success)
            {
                UpdateLog.ERROR_LOG("download fail: " + saveFilePath);
                //下载失败，删除已下载文件
                File.Delete(saveFilePath);
                //重置下载状态
                state.Downloading = false;
                //将失败data放回下载队列
                AddFailDataToPool(fileData);

                //如果是因为中止操作造成下载失败，则把文件加入到当前场景下载队列，继续下载
                if (num == CodeDefine.RET_SKIP_BY_ABORT)
                {
                    UpdateLog.ERROR_LOG("download fail because of stop action: " + saveFilePath);
                    //return;
                }
            }
            else
            {
                //累加下载大小
                TotalDownloadedSize += state.FileSize;
            }

            lock (m_downloadedLocker)
            {
                fileData.Downloaded = success;
                if (fileData.DownloadCallBack == null)
                {
                    return;
                }
            }

            if (_finishDownloadCallback != null)
            {
                _finishDownloadCallback(saveFilePath, success, state);
            }
        }

        private static int DownloadFile(MapFileData data, string url, string saveFilePath, long begin = 0L, long end = 0L)
        {
            int num = 0;
            FileStream outFile = new FileStream(saveFilePath, FileMode.Create);
            UpdateSystem.Download.Download ins = new UpdateSystem.Download.Download();
            ins.MapFileData = data;
            AddIns(ins);
            num = ins.HttpDownload(url, outFile, begin, end);
            RemoveIns(ins);
            return num;
        }

        private static int DownloadMapData(MapFileData fileData, string saveFilePath)
        {
            int code = CodeDefine.RET_INIT;
            try
            {
                lock (m_locker)
                {
                    if (!Directory.Exists(fileData.SaveDir + "/" + fileData.Dir))
                    {
                        Directory.CreateDirectory(fileData.SaveDir + "/" + fileData.Dir);
                    }
                }
                long begin = fileData.Begin + 4*32 + fileData.DirLen + fileData.NameLen + fileData.Md5Len;
                long end = fileData.End - 1;
                int retryTimes = _RETRAY_TIMES;
                while ((retryTimes > 0) && (code <= CodeDefine.RET_INIT))
                {
                    retryTimes--;
                    if (fileData.Name.Contains("RemoteVersion.xml") || fileData.Name.ToLower().Contains("localversion.xml"))
                    {
                        return CodeDefine.RET_SUCCESS;
                    }
                    code = DownloadUseBackCdn(fileData, fileData.ResUrl, saveFilePath, begin, end);
                    if (code == CodeDefine.RET_SKIP_BY_ABORT)
                    {
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                if (saveFilePath.Contains("ClassesResources.xml"))
                {
                    code = CodeDefine.RET_SUCCESS;
                }
                UpdateLog.ERROR_LOG(_TAG + "ThreadCallBack(object state) download fail: file= " + saveFilePath + 
                    "\n error" + exception.Message + "\n" + exception.StackTrace);
                UpdateLog.EXCEPTION_LOG(exception);
            }
            return code;
        }

        private static int DownloadUseBackCdn(MapFileData data, string url, string saveFilePath, long begin = 0L, long end = 0L)
        {
            int num = 0;
            if (UpdateSystem.Download.Download.UseBackupCdn(url, UpdateSystem.Download.Download.BackupCdn))
            {
                for (int i = 0; ((num <= 0)) && (i < UpdateSystem.Download.Download.BackupCdn.Length); i++)
                {
                    num = DownloadFile(data, url.Replace(Download.BackupCdn[0], Download.BackupCdn[i]), saveFilePath, begin, end);
                    if (num == CodeDefine.RET_SKIP_BY_ABORT)
                    {
                        break;
                    }
                }
                return num;
            }
            return DownloadFile(data, url, saveFilePath, begin, end);
        }

        internal static object GetLocker()
        {
            return m_downloadedLocker;
        }

        /// <summary>
        /// 暂停后台下载
        /// </summary>
        internal static void PauseAll()
        {
            //先暂停线程池
            if (_threadPool != null)
            {
                _threadPool.Pause();
            }
            //中断所有正在下载的http请求
            PauseLevel(DataLevel.High);
            PauseLevel(DataLevel.CurScene);
            PauseLevel(DataLevel.NextScene);
            PauseLevel(DataLevel.Low);
        }

        /// <summary>
        /// 恢复下载
        /// </summary>
        internal static void ResumeAll()
        {
            ResumeLevel(DataLevel.High);
            ResumeLevel(DataLevel.CurScene);
            ResumeLevel(DataLevel.NextScene);
            ResumeLevel(DataLevel.Low);
        }

        /// <summary>
        /// 暂停指定下载队列
        /// </summary>
        /// <param name="level"></param>
        internal static void PauseLevel(DataLevel level)
        {
            if (_threadPool != null)
            {
                //1.设置暂停等级
                _threadPool.PauseLevel(level);
                //2.中断http下载，让线程尽快结束任务
                AbortHttp(level);
            }
        }

        internal static void ResumeLevel(DataLevel level)
        {
            if (_threadPool != null)
            {
                //1.设置恢复等级
                _threadPool.ResumeLevel(level);
                //2.线程池唤醒
                _threadPool.Resume();
            }
        }

        /// <summary>
        /// 是否暂停中
        /// </summary>
        /// <returns></returns>
        internal static bool IsPaused()
        {
            bool paused = true;
            if (_threadPool != null)
            {
                paused = _threadPool.IsPaused();
            }

            return paused;
        }

        /// <summary>
        /// 是否暂停了静默更新
        /// </summary>
        /// <returns></returns>
        internal static bool IsBaseResPaused()
        {
            bool paused = true;
            if (_threadPool != null)
            {
                paused = _threadPool.IsBaseResPaused();
            }

            return paused;
        }

        /// <summary>
        /// 是否暂停了当前场景资源更新
        /// </summary>
        /// <returns></returns>
        internal static bool IsCurScenePaused()
        {
            bool paused = true;
            if (_threadPool != null)
            {
                paused = _threadPool.IsCurScenePaused();
            }

            return paused;
        }

        /// <summary>
        /// 获取对应队列数据个数
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        internal static int GetDataCount(DataLevel level)
        {
            int count = 0;
            if (_threadPool != null)
            {
                count = _threadPool.GetDataCount(level);
            }
            return count;
        }

        internal static bool IsDateLevelDownloading(DataLevel level)
        {
            lock (m_locker)
            {
                for (int i = 0; i < _httpInsList.Count; i++)
                {
                    if (_httpInsList[i].MapFileData.DataLevel == level)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 中断指定http下载
        /// </summary>
        private static void AbortHttp(DataLevel level)
        {
            lock (m_locker)
            {
                for (int i = 0; i < _httpInsList.Count; i++)
                {
                    if (level == DataLevel.All)
                    {
                        _httpInsList[i].Abort(null);
                    }
                    else if (_httpInsList[i].MapFileData != null && 
                        _httpInsList[i].MapFileData.DataLevel == level)
                    {
                        _httpInsList[i].Abort(null);
                    }
                }
            }
        }

        private static void RemoveIns(UpdateSystem.Download.Download ins)
        {
            lock (m_locker)
            {
                _httpInsList.Remove(ins);
            }
        }
    }
}


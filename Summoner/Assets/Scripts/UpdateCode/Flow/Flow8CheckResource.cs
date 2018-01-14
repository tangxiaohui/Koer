using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UpdateSystem.Xml;
using UpdateSystem.Log;
using UpdateSystem.Data;

namespace UpdateSystem.Flow
{
    /// <summary>
    /// 8. 资源检查，通过map文件比对本地文件大小或者md5，判断哪些文件需要做修复
    /// </summary>
    public class Flow8CheckResource : BaseFlow
    {
        //包含分段资源数据
        private DataModel _currentData;
        //本地xml
        private LocalVersionXml _localXml;
        //强制检查md5的文件列表
        private List<string> _forceCheckMd5List;
        //已经检查过的文件个数，用于展示进度
        private int _checkedCount;
        //解析出来的map数据
        private List<MapFileData> _parsedMapDataList;


        public override void Inititalize()
        {
            base.Inititalize();
            MapFileDataListForDownload = new List<MapFileData>();
            _parsedMapDataList = new List<MapFileData>();
        }

        public void SetExternalData(List<string> forceCheckList)
        {
            _forceCheckMd5List = forceCheckList;
        }

        public override void OnEnter(BaseFlow oldFlow)
        {
            base.OnEnter(oldFlow);
            _localXml = LocalXml;
            _currentData = CurrentRemoteData;
        }

        public override int Work()
        {
            if (!CheckLastFlowResult()) return LastFlowResult;
            int ret = parseMapFiles();
            if (ret >= CodeDefine.RET_SUCCESS)
            {
                if (updatePathVersion())
                {
                    ret = checkLocalFileMD5();
                }
                else
                    ret = checkLocalFileSize();
            }

            return ret;
        }

        public override void GetCurDownInfo(out string url, out int total, out int downloaded)
        {
            url = "";
            total = _parsedMapDataList.Count;
            downloaded = _checkedCount;
        }

        public override void Uninitialize()
        {
            if (MapFileDataListForDownload != null)
            {
                MapFileDataListForDownload.Clear();
            }

            if (_parsedMapDataList != null)
            {
                _parsedMapDataList.Clear();
            }
        }

        //需要下载的文件列表
        public List<MapFileData> GetMapListDataToDownload()
        {
            return MapFileDataListForDownload;
        }

        //解析本地所有的map文件
        private int parseMapFiles()
        {
            UpdateLog.DEBUG_LOG("解析map文件+++");
            int ret = CodeDefine.RET_SUCCESS;
            MapFileDataListForDownload.Clear();
            _parsedMapDataList.Clear();

            string resUrl = "";
            for (int i = 0; i < _currentData.VersionModelBaseList.Count; i++)
            {
                VersionModel mapModel = _currentData.VersionModelBaseList[i];

                //分段版本比本地分段更大，则跳过解析，本地可能没有
				if (LocalXml.BaseResVersion.CompareTo(mapModel.ToVersion.Replace("。", ".")) < 0)
                {
                    continue;
                }

                string mapUrl = mapModel.Map_url.Replace("\\", "/");
                string mapName = mapUrl.Substring(mapUrl.LastIndexOf("/") + 1); 
				UnityEngine.Debug.Log ("mapName:" + mapName);
                string localMapFile = System.IO.Path.Combine(_storeDir, mapName);
                resUrl = mapModel.ResourceUrl;
                MapFileManage mapManager = new MapFileManage();
                ret = mapManager.parseMapFile(localMapFile, mapModel.ResourceUrl, _storeDir);

                if (ret <= CodeDefine.RET_FAIL)
                {
                    return ret;
                }

                _parsedMapDataList.AddRange(mapManager.GetMapFileDataList());
            }

            UpdateLog.DEBUG_LOG("解析map文件---");
            return ret;
        }


        //根据解析出来的map文件，比较本地文件的md5，如果不同，则需要重新下载
        private int checkLocalFileMD5()
        {
            UpdateLog.DEBUG_LOG("检查本地文件md5+++");

            int ret = CodeDefine.RET_SUCCESS;
            int total = _parsedMapDataList.Count;
            _checkedCount = 0;

            for (int i = 0; i < _parsedMapDataList.Count; i++)
            {
                MapFileData fileData = _parsedMapDataList[i];
				if (fileData.Name.ToLower().Contains("localversion.xml") || fileData.Name.ToLower().Contains("resourceassetbundles"))
                {
                    continue;
                }
                string localFile = (_storeDir + "/" + fileData.Dir + fileData.Name).Replace("\\", "/").Replace("//", "/");
                string localFileMD5 = MD5.MD5File(localFile);
                if (localFileMD5.Equals("") || fileData.Md5.Equals(localFileMD5) == false)
                {
                    MapFileDataListForDownload.Add(fileData);
                }
                _checkedCount++;
            }

            if (_checkedCount > 0)
            {
                UpdateLog.WARN_LOG("需要下载文件");
            }

            UpdateLog.DEBUG_LOG("检查本地文件md5---");
            return ret;
        }

        //根据文件大小判断来获取下载列表
        private int checkLocalFileSize()
        {
            UpdateLog.DEBUG_LOG("检查本地文件size+++");

            int ret = CodeDefine.RET_SUCCESS;
            int total = _parsedMapDataList.Count;
            _checkedCount = 0;

            for (int i = 0; i < _parsedMapDataList.Count; i++)
            {
                MapFileData fileData = _parsedMapDataList[i];
				if (fileData.Name.ToLower().Contains("localversion.xml") || fileData.Name.ToLower().Contains("resourceassetbundles"))
                {
                    continue;
                }
                string localFile = (_storeDir + "/" + fileData.Dir + fileData.Name).Replace("\\", "/").Replace("//", "/");
                if (isInForceList(localFile))
                {
                    string localFileMD5 = MD5.MD5File(localFile);
                    if (localFileMD5.Equals("") || fileData.Md5.Equals(localFileMD5) == false)
                    {
                        MapFileDataListForDownload.Add(fileData);
                    }
                }
                else
                {
                    FileInfo fileInfo = new FileInfo(localFile);
                    if (!fileInfo.Exists || fileInfo.Length != fileData.FileSize)
                    {
                        MapFileDataListForDownload.Add(fileData);
                    }
                }
                _checkedCount++;
            }

            if (_checkedCount > 0)
            {
                UpdateLog.WARN_LOG("需要下载文件");
            }

            UpdateLog.DEBUG_LOG("检查本地文件size---");
            return ret;
        }


        //是否需要更新补丁版本
        private bool updatePathVersion()
        {
            string localPathVersion = _localXml.PatchResVersion;
            string latestPathVersion = localPathVersion;
            if (_currentData.VersionModelPatchList.Count > 0)
            {
                latestPathVersion = _currentData.VersionModelPatchList[_currentData.VersionModelPatchList.Count - 1].ToVersion;
            }

            return latestPathVersion.CompareTo(localPathVersion) > 0;
        }


        //是否在强制需要判断md5的文件列表中
        private bool isInForceList(string filePath)
        {
            filePath = filePath.Replace("\\", "/");
            for (int i = 0; _forceCheckMd5List != null && i < _forceCheckMd5List.Count; ++i)
            {
                if (filePath.IndexOf(_forceCheckMd5List[i]) >= 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

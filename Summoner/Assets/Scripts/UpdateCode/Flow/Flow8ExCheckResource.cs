namespace UpdateSystem.Flow
{
    using UpdateSystem.Data;
    using UpdateSystem.Log;
    using UpdateSystem.Xml;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class Flow8ExCheckResource : BaseFlow
    {
        private int _checkedCount;
        private DataModel _currentData;
        private List<string> _forceCheckMd5List;
        private List<MapFileData> _parsedMapDataList;
        private Dictionary<string, MapFileData> _backDownloadDict;
        //本地不存在的文件，需要加入到静默更新列表中
        internal List<MapFileData> BackDownloadList;

        private int checkLocalFileMD5()
        {
            UpdateLog.DEBUG_LOG("检查本地文件md5+++");
            int count = this._parsedMapDataList.Count;
            this._checkedCount = 0;
            for (int i = 0; i < this._parsedMapDataList.Count; i++)
            {
                this._checkedCount++;
                MapFileData item = this._parsedMapDataList[i];
                string path = BaseFlow._storeDir + "/" + item.Dir + item.Name;
				if (item.Name.ToLower().IndexOf("localversion.xml") == -1 || item.Name.ToLower().IndexOf("resourceassetbundles") == -1)
                {
                    string str2 = BaseFlow._appDir + "/" + item.Dir + item.Name;
                    if (File.Exists(path) || File.Exists(str2))
                    {
                        string str3 = MD5.MD5File(path);
                        if (string.IsNullOrEmpty(str3))
                        {
                            str3 = MD5.MD5File(str2);
                        }
                        if (!(!str3.Equals("") && item.Md5.Equals(str3)))
                        {
                            RepairList.Add(item);
                        }
                    }
                    else
                    {
						if (item.Name.ToLower ().IndexOf ("resourceassetbundles") == -1) {
							RepairList.Add (item);
							continue;
							//后台下载(是否后台下载)
							_backDownloadDict.Add (path, item);
							this.BackDownloadList.Add (item);
						}
                    }
                }
            }
            if (this._checkedCount > 0)
            {
                UpdateLog.WARN_LOG("需要下载文件");
            }

            MapFileDataListForDownload = RepairList;

            UpdateLog.DEBUG_LOG("检查本地文件md5---");
            return 1;
        }

        public override void GetCurDownInfo(out string url, out int total, out int downloaded)
        {
            url = "";
            total = (this._parsedMapDataList == null) ? 0 : this._parsedMapDataList.Count;
            downloaded = this._checkedCount;
        }

        public MapFileData GetMapFileDataByPath(string path)
        {
            MapFileData data = null;
            string str = Path.GetDirectoryName(path).Replace(@"\", "/") + "/";
            string fileName = Path.GetFileName(path);
            //绝对路径
            if (path.IndexOf(_storeDir) == -1)
            {
                path = _storeDir + "/" + path;
            }
            _backDownloadDict.TryGetValue(path, out data);
            return data;
        }

        public List<MapFileData> GetMapListDataToDownload()
        {
            return RepairList;
        }

        public override void Inititalize()
        {
            base.Inititalize();
            RepairList = new List<MapFileData>();
            this._parsedMapDataList = new List<MapFileData>();
            this.BackDownloadList = new List<MapFileData>();
            _backDownloadDict = new Dictionary<string, MapFileData>();
        }

        public override void OnEnter(BaseFlow oldFlow)
        {
            base.OnEnter(oldFlow);
            this._currentData = base.CurrentRemoteData;
        }

        private int parseMapFiles()
        {
            UpdateLog.DEBUG_LOG("解析map文件+++");
            int num = 1;
            RepairList.Clear();
            this.BackDownloadList.Clear();
            _backDownloadDict.Clear();
            this._parsedMapDataList.Clear();
            string resourceUrl = "";
            for (int i = 0; i < this._currentData.VersionModelBaseList.Count; i++)
            {
                VersionModel model = this._currentData.VersionModelBaseList[i];
                string str2 = model.Map_url.Replace(@"\", "/");
                string str3 = str2.Substring(str2.LastIndexOf("/") + 1);
                string mapFile = Path.Combine(BaseFlow._storeDir, str3);
                resourceUrl = model.ResourceUrl;
                MapFileManage manage = new MapFileManage();
                num = manage.parseMapFile(mapFile, model.ResourceUrl, BaseFlow._storeDir);
                if (num <= -1)
                {
                    return num;
                }
                this._parsedMapDataList.AddRange(manage.GetMapFileDataList());
            }
            UpdateLog.DEBUG_LOG("解析map文件---");
            return num;
        }

        public void SetExternalData(List<string> forceCheckList)
        {
            this._forceCheckMd5List = forceCheckList;
        }

        public override void Uninitialize()
        {
            if (RepairList != null)
            {
                RepairList.Clear();
            }
            if (this._parsedMapDataList != null)
            {
                this._parsedMapDataList.Clear();
            }
        }

        public override int Work()
        {
            if (!base.CheckLastFlowResult())
            {
                return base.LastFlowResult;
            }
            int num = this.parseMapFiles();
            if (num >= 1)
            {
                num = this.checkLocalFileMD5();
            }
            return num;
        }
    }
}


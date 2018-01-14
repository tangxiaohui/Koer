namespace UpdateSystem.Flow
{
    using UpdateSystem.Data;
    using UpdateSystem.Download;
    using UpdateSystem.Log;
    using UpdateSystem.Xml;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class Flow7ExDownloadMapFile : BaseFlow
    {
        private DataModel _currentData;
        private FileDownload _fileDownload;

        private bool checkNeedDownloadMapFile(string localMapFile, string onlineMapMd5)
        {
            if (File.Exists(localMapFile))
            {
                string str = MD5.MD5File(localMapFile);
                if ((str != null) && str.Equals(onlineMapMd5))
                {
                    return false;
                }
            }
            return true;
        }

        private int downloadMapFile()
        {
            UpdateLog.INFO_LOG("downloadMapFile +++");
            int num = 1;
            List<VersionModel> list = new List<VersionModel>();
           // if (this._currentData.VersionModelBaseList.Count > 0)
            {
				list.AddRange(this._currentData.VersionModelBaseList);
            }
            if (list.Count == 0)
            {
                num = 0;
            }
            for (int i = 0; i < list.Count; i++)
            {
                VersionModel model = list[i];
                string str = model.Map_url.Replace(@"\", "/");
                string str2 = str.Substring(str.LastIndexOf("/") + 1);
                string localMapFile = Path.Combine(BaseFlow._storeDir, str2);
                if (this.checkNeedDownloadMapFile(localMapFile, model.Map_md5))
                {
                    string url = model.Map_url;
                    int num3 = this._fileDownload.DownloadUseBackCdn(localMapFile, url, this.ParseInt(model.Map_size), false);
                    if ((num3 >= 0) && this.checkNeedDownloadMapFile(localMapFile, model.Map_md5))
                    {
                        string str5 = MD5.MD5File(localMapFile);
                        UpdateLog.ERROR_LOG("Download map file error md5: " + localMapFile + " md5=" + str5 + "\n online md5: " + model.Map_url + " md5=" + model.Map_md5);
                        num3 = -6;
                    }
                    if (num3 <= -1)
                    {
                        return num3;
                    }
                }
            }
            UpdateLog.INFO_LOG("downloadMapFile ---");
            return num;
        }

        public override void GetCurDownInfo(out string url, out int total, out int downloaded)
        {
            base.GetCurDownInfo(out url, out total, out downloaded);
            if (this._fileDownload != null)
            {
                this._fileDownload.GetCurDownInfo(out url, out total, out downloaded);
            }
        }

        public override void Inititalize()
        {
            base.Inititalize();
            this._fileDownload = new FileDownload();
            base.UseDownload = true;
        }

        public override void OnEnter(BaseFlow oldFlow)
        {
            base.OnEnter(oldFlow);
            this._currentData = base.CurrentRemoteData;
        }

        private int ParseInt(string str)
        {
            int result = 0;
            int.TryParse(str, out result);
            return result;
        }

        public override void Uninitialize()
        {
            if (this._fileDownload != null)
            {
                this._fileDownload.Abort(null);
            }
        }

        public override int Work()
        {
            if (!base.CheckLastFlowResult())
            {
                return base.LastFlowResult;
            }
            return this.downloadMapFile();
        }
    }
}


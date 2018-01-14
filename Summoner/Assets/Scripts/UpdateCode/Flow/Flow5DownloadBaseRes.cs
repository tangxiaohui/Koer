using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UpdateSystem.Xml;
using UpdateSystem.Log;
using UpdateSystem.Download;

namespace UpdateSystem.Flow
{
    /// <summary>
    /// 5. 下载基础资源，分段资源
    /// a. 当本地分段号为0时，强制下载分段资源第一段
    /// b. 当本地分段号大于0时，则需要外部开关来决定是否开启下载
    /// c. 支持后台下载,后台下载完后不做资源释放
    /// </summary>
    public class Flow5DownloadBaseRes : BaseFlow
    {
        //本地分段号
        private string _localBaseResVersion;
        //包含分段资源数据
        private DataModel _currentData;
        //下载器
        private FileDownload _fileDownload;
        //后台下载
		private bool _backDownload = false;
        //是否允许下载分段资源，需要游戏内确认
		private bool _enableDwonBase = false;

        private List<VersionModel> _needDownloadList;

        public override void Inititalize()
        {
            base.Inititalize();
            _fileDownload = new FileDownload();
            UseDownload = true;
        }

        public override void OnEnter(BaseFlow oldFlow)
        {
            base.OnEnter(oldFlow);
            _localBaseResVersion = LocalBaseResVersion;
            _currentData = CurrentRemoteData;
        }

        public override int Work()
        {
            if (!CheckLastFlowResult()) return LastFlowResult;
            UpdateLog.DEBUG_LOG("下载base资源+++");
            int ret = CodeDefine.RET_FAIL;

            if (!_backDownload && needTransResource())
            {
                ret = CodeDefine.RET_SKIP_BY_FORCE_TRANS_RESOURCE;
                return ret;
            }

            //本地段号为0时，强制做分段资源下载，下载第一段，当段号为0时，前面就已经做了转移资源了
            if (_localBaseResVersion == "0" || _enableDwonBase)
            {
				ret = DownloadBaseRes(false);
            }
            else if(_backDownload)
            {
                BackgroundDownload();
                ret = CodeDefine.RET_SKIP_BY_BACKDOWNLOAD;
            }
            else
            {
                ret = CodeDefine.RET_SUCCESS;
                UpdateLog.DEBUG_LOG("不需要下载分段资源");
            }

            UpdateLog.DEBUG_LOG("下载base资源---");
            return ret;
        }

        public override void OnLeave(int ret)
        {
            base.OnLeave(ret);

            //流程成功才做重置操作，不然重试下载会跳过
            if (ret >= CodeDefine.RET_SUCCESS)
            {
                //需要重置
                _backDownload = false;
                _enableDwonBase = false;
            }
        }

        public override void GetCurDownInfo(out string url, out int total, out int downloaded)
        {
            base.GetCurDownInfo(out url, out total, out downloaded);

            if (_fileDownload != null)
            {
                _fileDownload.GetCurDownInfo(out url, out total, out downloaded);
            }
        }

        public override void Uninitialize()
        {
            if (_fileDownload != null)
            {
                _fileDownload.Abort(null);
                _fileDownload = null;
            }
        }

        public override void Abort()
        {
            base.Abort();
            if (_fileDownload.Downloading)
            {
                _fileDownload.Abort(null);
            }
        }

        //后台下载
        public void SetBackDownload(bool back)
        {
            _backDownload = back;
        }


        //下载分段资源，在游戏内提示下载后续资源后
        public void SetEnableDownBase(bool enable)
        {
            _enableDwonBase = enable;
        }

        public int BackgroundDownload()
        {
            UpdateLog.DEBUG_LOG("后台下载base资源+++");
            int ret = CodeDefine.RET_FAIL;
            ret = DownloadBaseRes(true);

            UpdateLog.DEBUG_LOG("后台下载base资源---");
            return ret;
        }

        //后台下载切到前台
        public void ChangeBackToFrontDownload()
        {
            Abort();
        }

        public int GetAllBaseList()
        {
            if (_currentData != null)
            {
                return _currentData.VersionModelBaseList.Count;
            }

            return 0;
        }

        public int DownloadBaseRes(bool downloadAll)
        {
            int ret = CodeDefine.RET_FAIL;
            _needDownloadList = needDownloadBaseResList();
            if (_needDownloadList.Count == 0)
            {
                UpdateLog.INFO_LOG("do not need download base res, the version is the latest one. : version = " + _localBaseResVersion);
                return CodeDefine.RET_SUCCESS;
            }

            int currentState = 0;
            for (int i = 0; i < _needDownloadList.Count; i++)
            {
                currentState = int.Parse(_needDownloadList[i].ToVersion);
                var toDownloadModel = _needDownloadList[i];
                string resourceUrl = toDownloadModel.ResourceUrl.Replace("\\", "/");
                string resName = resourceUrl.Substring(resourceUrl.LastIndexOf("/") + 1);
                string storePath = System.IO.Path.Combine(_storeDir, resName);
                long downloadedSize = 0;
                if (checkFinishDownload(storePath, toDownloadModel, out downloadedSize))
                {
                    ret = CodeDefine.RET_SUCCESS;
                    UpdateLog.INFO_LOG("分段资源已经下载好了: " + storePath);
                    continue;
                }
                long totalSize = long.Parse(toDownloadModel.FileSize);
				long mapSize = long.Parse (toDownloadModel.Map_size);
                long needDownloadSize = totalSize - downloadedSize;
                UpdateLog.INFO_LOG("UpdateFlow: 需要下载基础资源 " + storePath + "totalSize = " + totalSize + " needDownloadSize=" + needDownloadSize);

                //后台下载不做提示
                if (!_backDownload && !Pause((int)totalSize))
                {
                    return CodeDefine.RET_SKIP_BY_CANCEL;
                }

                ret = _fileDownload.DownloadUseBackCdn(storePath, toDownloadModel.ResourceUrl, (int)totalSize, true);

                //失败、非all、非后台下载都中断
                if (ret <= CodeDefine.RET_FAIL || !downloadAll || !_backDownload)
                {
                    break;
                }
            }

            UpdateLog.DEBUG_LOG("下载base资源---");
            return ret;
        }


        //有基础资源需要下载
        public List<VersionModel> GetNeedDownloadBaseResList()
        {
            _needDownloadList = needDownloadBaseResList();
            return _needDownloadList;
        }

        //检查是否已经下载完了
        private bool checkFinishDownload(string storePath, VersionModel baseRes, out long downloadedSize)
        {
            downloadedSize = 0;
            FileInfo fileInfo = new FileInfo(storePath);
            if (!fileInfo.Exists)
            {
                return false;
            }

            downloadedSize = fileInfo.Length;

            if (fileInfo.Length == long.Parse(baseRes.FileSize))
            {
                return true;
            }

            return false;
        }

        //需要下载的分段资源列表
        private List<VersionModel> needDownloadBaseResList()
        {
            if (_needDownloadList == null)
            {
                _needDownloadList = new List<VersionModel>();
            }

            _needDownloadList.Clear();

            string baseVersion = _localBaseResVersion;
            for (int i = 0; _currentData != null && i < _currentData.VersionModelBaseList.Count; ++i)
            {
                string from = _currentData.VersionModelBaseList[i].FromVersion;
                string to = _currentData.VersionModelBaseList[i].ToVersion;
                if (from.Equals(baseVersion))
                {
                    _needDownloadList.Add(_currentData.VersionModelBaseList[i]);
                    baseVersion = to;
                }
            }
            return _needDownloadList;
        }

        //是否需要转移资源
        private bool needTransResource()
        {
            if (!HasTransedResource())
            {
                return needUpdatePathVersion();
            }

            return false;
        }

        //是否需要更新补丁版本
        private bool needUpdatePathVersion()
        {
            string localPathVersion = LocalXml.PatchResVersion;
            string latestPathVersion = localPathVersion;
            if (_currentData.VersionModelPatchList.Count > 0)
            {
                latestPathVersion = _currentData.VersionModelPatchList[_currentData.VersionModelPatchList.Count - 1].ToVersion;
            }

            return latestPathVersion.CompareTo(localPathVersion) > 0;
        }
    }
}

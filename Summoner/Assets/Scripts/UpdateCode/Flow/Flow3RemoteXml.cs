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
    /// 3. 下载RemoteVersion.xml，
    /// </summary>
    public class Flow3RemoteXml : BaseFlow
    {
        //继续后的localXml数据
        private LocalVersionXml _localXml;
        //下载器
        private FileDownload _fileDownload;
        //RemoteXml name
        private string _remoteXmlName;
        //进入测试流程用到的imei、mac、idfa
        private string _imieOrMacOrIdfa;
        //进入测试流程用的ip
        private string _whiteIP;
        //使用测试流程
        private bool _useTestFlow;

        //外部数据，在初始化前调用
        public void SetExternalData(string imeiOrMacOrIdfa, string ip)
        {
            _imieOrMacOrIdfa = imeiOrMacOrIdfa;
            _whiteIP = ip;
        }

        public override void Inititalize()
        {
            base.Inititalize();
            _fileDownload = new FileDownload();
            UseDownload = true;
        }


        public override int Work()
        {
            if (!CheckLastFlowResult()) return LastFlowResult;
            int ret = CodeDefine.RET_INIT;

            _localXml = LocalXml;
            if (_localXml == null)
                return ret;

            //跳过更新
            if(_localXml.EnableDownload.Equals("false"))
            {
                ret = CodeDefine.RET_SKIP_BY_DISABLEDOWNLOAD;
                return ret;
            }

            //有数据，则不重新下载xml了
            if (RemoteXml != null && _recentResult >= CodeDefine.RET_SUCCESS)
            {
                ret = _recentResult;
                return ret;
            }
            
            _remoteXmlName = _localXml.ResourceVersionUrl.Substring(_localXml.ResourceVersionUrl.LastIndexOf("/") + 1);

            //1. 下载
            ret = downloadRemoteXml(_localXml.ResourceVersionUrl, _storeDir);
            if (ret >= CodeDefine.RET_SUCCESS)
            {
                string downloadedXmlPath = System.IO.Path.Combine(_storeDir, _remoteXmlName);
                //2. 解析
                ret = parseResourceXml(downloadedXmlPath);
            }
            if (ret >= CodeDefine.RET_SUCCESS)
            {
                //3. 判断使用正式流程还是测试流程
                CurrentRemoteData = useTestFlow() ? RemoteXml.TestFollow : RemoteXml.NormalFollow;
                sortBaseVersion();
            }

            return ret;
        }

        public override void Uninitialize()
        {
            RemoteXml = null;
            _useTestFlow = false;
            CurrentRemoteData = null;
        }

        //下载
        private int downloadRemoteXml(string remoteUrl, string storeDir)
        {
            UpdateLog.INFO_LOG("下载resourceVersionXml " + remoteUrl);
            int ret = CodeDefine.RET_SUCCESS;

            if (string.IsNullOrEmpty(_remoteXmlName))
            {
                UpdateLog.ERROR_LOG("downloadResourceVersion(): resourceXmlName == null || \"\".Equals(resourceXmlName)");
                ret = CodeDefine.RET_FAIL_RES_XML_PATH_ERROR;
                return ret;
            }

            string savePath = (storeDir + "/" + _remoteXmlName).Replace("\\", "/").Replace("//", "/");

            if (!Directory.Exists(storeDir))
            {
                Directory.CreateDirectory(storeDir);
            }
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Thread.Sleep(1);
            }

            ret = _fileDownload.DownloadUseBackCdn(savePath, remoteUrl, 0, false);

            if (ret < CodeDefine.RET_SUCCESS )
            {
                UpdateLog.ERROR_LOG("downloadResourceVersion(): resource file is not exist or download fail!");
                ret = CodeDefine.RET_FAIL_DOWNLOAD_RES_XML;
            }

            return ret;
        }

        //解析
        private int parseResourceXml(string remoteXmlPath)
        {
            RemoteXml = new ResourceVersionXml();
            return RemoteXml.parseResouceVersionXml(remoteXmlPath);
        }

        //检查是否进入测试流程
        private bool useTestFlow()
        {
            if (_useTestFlow)
            {
                return _useTestFlow;
            }

            string appVersion = _localXml.LocalAppVersion;

            //judge white users
            string whiteAppUser = appVersion + ":true";

            //version control， 1.3.1:true
            for (int i = 0; i < RemoteXml.WhiteUsers.Count; i++)
            {
                if (whiteAppUser.Equals(RemoteXml.WhiteUsers[i]))
                {
                    _useTestFlow = true;
                    break;
                }
            }

            //imei、mac、idfa， 需要判断大小版本是否匹配
            if (!string.IsNullOrEmpty(_imieOrMacOrIdfa) && !_useTestFlow)
            {
                for (int i = 0; i < RemoteXml.WhiteCode.Count; i++)
                {
                    if (RemoteXml.WhiteCode[i].Equals(_imieOrMacOrIdfa) &&
                        (appVersion.Equals(RemoteXml.TestFollow.BigAppVersion) || appVersion.Equals(RemoteXml.TestFollow.SmallAppVersion)))
                    {
                        _useTestFlow = true;
                        break;
                    }
                }
            }

            //judge white ip
            if (!string.IsNullOrEmpty(_whiteIP) && !_useTestFlow)
            {
                for (int i = 0; i < RemoteXml.WhiteIp.Count; i++)
                {
                    if (RemoteXml.WhiteIp[i].Equals(_whiteIP))
                    {
                        _useTestFlow = true;
                        break;
                    }
                }
            }

            return _useTestFlow;
        }

        //基础资源按照段号排序，从小到大
        private void sortBaseVersion()
        {
            if (CurrentRemoteData == null)
            {
                return;
            }

            CurrentRemoteData.VersionModelPatchList.Sort(compare);
            CurrentRemoteData.VersionModelBaseList.Sort(compare);
        }

        private int compare(VersionModel t1, VersionModel t2)
        {
            return (t1.ToVersion.CompareTo(t2.ToVersion));
        }
    }
}

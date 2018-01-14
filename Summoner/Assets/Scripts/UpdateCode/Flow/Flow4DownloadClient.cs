using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UpdateSystem.Xml;
using UpdateSystem.Log;
using UpdateSystem.Delegate;
using UpdateSystem.Download;

namespace UpdateSystem.Flow
{
    /// <summary>
    /// 4. 需不需要下载更新客户端
    /// </summary>
    public class Flow4DownloadClient : BaseFlow
    {
        //下载客户端的方式可以自定义
        private CustomDownClientFunc _customDownClientFunc;
        //客户端下载完成调用
        private ClientDownloadFinishCallback _onClientDownFinish;
        //ios不用下载客户端，是跳转到url
        private bool _ios;
        //下载器
        private FileDownload _fileDownload;

        public override void Inititalize()
        {
            base.Inititalize();
            _fileDownload = new FileDownload();
            UseDownload = true;
        }

        public void SetExternalData(ClientDownloadFinishCallback func, CustomDownClientFunc customDownClientFunc, bool ios)
        {
            _onClientDownFinish = func;
            _customDownClientFunc = customDownClientFunc;
            _ios = ios;
        }

        public override int Work()
        {
            if (!CheckLastFlowResult()) return LastFlowResult;
            
            if (!CurrentRemoteData.EnableForceUpdate)
            {
                UpdateLog.DEBUG_LOG("Do not support force update client, skip download!!!");
                return CodeDefine.RET_SUCCESS;
            }

            UpdateLog.DEBUG_LOG("开始下载客户端+++");
            int ret = CodeDefine.RET_INIT;
            var localXml = LocalXml;
            var remoteData = CurrentRemoteData;
            string appVersion = localXml.LocalAppVersion;
            string clientUrl = remoteData.ClientUrl.Replace("\\", "/");
            string clientName = clientUrl.Substring(clientUrl.LastIndexOf("/") + 1);
            string clientPath = System.IO.Path.Combine(_storeDir, clientName);

            //远端有更高客户端版本，则检查下载
            if (remoteData.AppVersion.CompareTo(appVersion) > 0)
            {
                if (_customDownClientFunc != null)
                {
                    UpdateLog.DEBUG_LOG("使用外部方法下载客户端");
                    _customDownClientFunc(remoteData.ClientUrl, _storeDir);
                    ret = CodeDefine.RET_SUCCESS;
                    callClientDownloadFinish(true);
                }
                else
                {
                    if (_ios)
                    {
                        callClientDownloadFinish(true);
                        return CodeDefine.RET_SKIP_BY_DOWNLOAD_APP;
                    }

                    int appSize = int.Parse(remoteData.AppSize);

                    //下载前提醒，如果取消则直接退出当前流程
                    if (!Pause(appSize))
                        return CodeDefine.RET_SKIP_BY_CANCEL;
                    
                    ApkStorePath = clientPath;
                    ret = _fileDownload.DownloadUseBackCdn(clientPath, clientUrl, appSize, true);
                    
                    FileInfo clientFile = new FileInfo(clientPath);
                    if (ret >= CodeDefine.RET_SUCCESS && clientFile.Length < appSize)
                    {
                        ret = CodeDefine.RET_FAIL_EXCEPTION_DOWNLOAD;
                        UpdateLog.ERROR_LOG("download Client: size is not correct: " + clientFile.Length + " -> " + appSize);
                    }
                    callClientDownloadFinish(ret >= CodeDefine.RET_SUCCESS);
                }

                //下载成功则跳过后续流程
                if (ret == CodeDefine.RET_SUCCESS)
                    ret = CodeDefine.RET_SKIP_BY_DOWNLOAD_APP;
                UpdateLog.DEBUG_LOG("下载客户端结束");
            }
            else
            {
                if (File.Exists(clientPath))
                {
                    File.Delete(clientPath);
                    UpdateLog.DEBUG_LOG("删除已下载好的客户端！！！");
                }

                ret = CodeDefine.RET_SUCCESS;
            }
            UpdateLog.DEBUG_LOG("开始下载客户端---");
            return ret;
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
            _customDownClientFunc = null;
        }

        public override void Abort()
        {
            base.Abort();
            if (_fileDownload.Downloading)
            {
                _fileDownload.Abort(null);
            }
        }

        private void callClientDownloadFinish(bool result)
        {
            if (_onClientDownFinish != null)
            {
                _onClientDownFinish(result);
            }
        }
    }
}

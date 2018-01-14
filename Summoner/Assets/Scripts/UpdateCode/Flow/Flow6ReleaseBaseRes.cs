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
    /// 6. 释放分段资源
    /// a. 一次性释放本地所有下载好的资源
    /// b. 修改本地分段号
    /// </summary>
    public class Flow6ReleaseBaseRes : BaseFlow
    {
        //包含分段资源数据
        private DataModel _currentData;
        //本地xml
        private LocalVersionXml _localXml;

        public override void OnEnter(BaseFlow oldFlow)
        {
            base.OnEnter(oldFlow);
            _localXml = LocalXml;
            _currentData = CurrentRemoteData;
        }

        public override int Work()
        {
            if (LastFlowResult == CodeDefine.RET_SKIP_BY_BACKDOWNLOAD)
            {
                UpdateLog.DEBUG_LOG("因为是后台下载分段，跳过资源释放流程");
                return CodeDefine.RET_SUCCESS;
            }
            if (!CheckLastFlowResult()) return LastFlowResult;
            UpdateLog.DEBUG_LOG("释放分段资源+++");
            int ret = CodeDefine.RET_SUCCESS;
            for (int i = 0; i < _currentData.VersionModelBaseList.Count; i++)
            {
                VersionModel vModel = _currentData.VersionModelBaseList[i];
                //本地分段版本号更大，则跳过
                if (LocalXml.BaseResVersion.CompareTo(vModel.FromVersion) > 0)
                {
                    continue;
                }

                string resourceUrl = vModel.ResourceUrl.Replace("\\", "/");
                string resourceName = resourceUrl.Substring(resourceUrl.LastIndexOf("/") + 1);
                string localResourceFile = System.IO.Path.Combine(_storeDir, resourceName);

                if (!File.Exists(localResourceFile))
                {
                    UpdateLog.DEBUG_LOG("释放资源时，没有资源： " + localResourceFile);
                    continue;
                }

                //用大小比较
                FileInfo fileInfo = new FileInfo(localResourceFile);
                long toDownloadFileLenght = 0;
                if (long.TryParse(vModel.FileSize, out toDownloadFileLenght))
                {
                    if (fileInfo.Length == toDownloadFileLenght)
                    {
                        if (!HasTransedResource())
                        {
                            UpdateLog.DEBUG_LOG("还没有转移过资源，不能做资源释放，先跳转到转移资源流程!!!");
                            ret = CodeDefine.RET_SKIP_BY_FORCE_TRANS_RESOURCE;
                            break;
                        }

                        UpdateLog.DEBUG_LOG("释放分段资源： " + localResourceFile);
                        UnzipResource unzip = new UnzipResource(localResourceFile, _storeDir);
                        ret = unzip.UnzipRes();

                        //更新本地分段号
                        _localXml.BaseResVersion = vModel.ToVersion;
                        _localXml.save(_localXml);
                        //资源释放完了就删除下载好的资源
                        if (File.Exists(localResourceFile))
                        {
                            File.Delete(localResourceFile);
                        }
                    }
                    else
                        UpdateLog.DEBUG_LOG(string.Format("分段资源没下载完：{0} -> {1} ", localResourceFile, fileInfo.Length));
                }
            }

            UpdateLog.DEBUG_LOG("释放分段资源---");
            return ret;
        }

        public override void Uninitialize()
        {
            _currentData = null;
            _localXml = null;
        }
    }
}

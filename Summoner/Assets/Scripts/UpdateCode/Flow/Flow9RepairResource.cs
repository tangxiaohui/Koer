using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UpdateSystem.Xml;
using UpdateSystem.Log;
using UpdateSystem.Download;
using UpdateSystem.Data;

namespace UpdateSystem.Flow
{
    /// <summary>
    /// 9. 资源修复，下载Flow8检查出的需要修复的资源列表
    /// </summary>
    public class Flow9RepairResource : BaseFlow
    {
        //包含分段资源数据
        private DataModel _currentData;
        //资源修复下载器
        private RepairDownload _repairDownload;
        //本地xml
        private LocalVersionXml _localXml;
        //解析的map文件数据
        List<MapFileData> _mapFileDataList;

        //需要下载的总大小
        private int _totalSize;

        public override void Inititalize()
        {
            base.Inititalize();
            _repairDownload = new RepairDownload();
            _totalSize = 0;
            UseDownload = true;
        }

        public override void OnEnter(BaseFlow oldFlow)
        {
            base.OnEnter(oldFlow);
            _localXml = LocalXml;
            _currentData = CurrentRemoteData;
            _mapFileDataList = MapFileDataListForDownload;
            UpdateSystem.Download.Download.MutiDownloadedSize = 0;
            _totalSize = 0;
        }

        public override int Work()
        {
            if (!CheckLastFlowResult()) return LastFlowResult;
            UpdateLog.DEBUG_LOG("资源修复++++");
            int ret = CodeDefine.RET_INIT;
            ret = repairResource();
            UpdateLog.DEBUG_LOG("资源修复----");

            return ret;
        }

        public override void OnLeave(int ret)
        {
            base.OnLeave(ret);
            //Update.Download.Download.MutiDownloadedSize = 0;
        }

        //需要下载的总大小
        public int GetTotalSize()
        {
            return _totalSize;
        }

        public override void GetCurDownInfo(out string url, out int total, out int downloaded)
        {
            base.GetCurDownInfo(out url, out total, out downloaded);
            total = _totalSize;
            downloaded = UpdateSystem.Download.Download.MutiDownloadedSize;
        }

        public override void Uninitialize()
        {
            if (_mapFileDataList != null)
            {
                _mapFileDataList.Clear();
            }
        }

        public override void Abort()
        {
            base.Abort();
            if (_repairDownload != null)
            {
                _repairDownload.AbortAll(null);
            }
        }

        private int repairResource()
        {
            int ret = CodeDefine.RET_INIT;
            if (_mapFileDataList == null || _mapFileDataList.Count == 0)
            {
                ret = CodeDefine.RET_SUCCESS;
                UpdateLog.DEBUG_LOG("没有资源需要修复");
                updateLocalPathVersion();
                return ret;
            }
            UpdateLog.DEBUG_LOG("开始资源修复");

            for (int i = 0; i < _mapFileDataList.Count; ++i)
            {
                _totalSize += _mapFileDataList[i].FileSize;
            }

            if (!Pause(_totalSize))
            {
                ret = CodeDefine.RET_SKIP_BY_CANCEL;
            }
            else
                ret = _repairDownload.DownloadFileByMultiThread(_mapFileDataList);

            if (ret >= CodeDefine.RET_SUCCESS)
            {
                updateLocalPathVersion();
            }

            return ret;
        }

        //更新本地补丁版本号
        private void updateLocalPathVersion()
        {
            string localPathVersion = _localXml.PatchResVersion;
            string latestPathVersion = localPathVersion;
            int patchCount = _currentData.VersionModelPatchList.Count;
            if (patchCount > 0)
            {
                latestPathVersion = _currentData.VersionModelPatchList[patchCount - 1].ToVersion;
            }

            if (latestPathVersion.CompareTo(localPathVersion) > 0)
            {
                LocalXml.save("", CurrentRemoteData.VersionModelPatchList[patchCount - 1].ToVersion);
            }
        }
    }
}

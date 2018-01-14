using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UpdateSystem.Xml;
using UpdateSystem.Log;
using UpdateSystem.Trans;
using UpdateSystem.Enum;

namespace UpdateSystem.Flow
{
    /// <summary>
    /// 1. 资源转移，从客户端中释放资源到指定存储位置
    /// a. IOS如果包内有资源，启动的时候不做资源释放
    /// b. 安卓启动的时候判断是否需要释放资源
    /// c. PC版本不做资源释放
    /// d. 如果存储位置没有资源，同时app内LocalVersion的分段资源号是0，则强制释放资源
    /// e. 如果远端的补丁版本比本地补丁版本更高，则判断是否需要释放资源
    /// </summary>
    public class Flow1TransResource : BaseFlow
    {
        //平台类型
        private PlatformType _platformType;
        //资源转移功能实例
        TransResource _transInstance;

        /// <summary>
        /// 强制转移资源
        /// 1. ios在进入游戏后有资源更新，会返回到更新界面做资源转移
        /// 2. 
        /// </summary>
        private bool _forceTrans = false;

        public void SetExternalData(string sourcePath, PlatformType type)
        {
            _platformType = type;

            switch (_platformType)
            {
                case PlatformType.Android:
                    _transInstance = new TransAndroidResource();
                    break;
                case PlatformType.IOS:
                    _transInstance = new TransIOSResource();
                    break;
                case PlatformType.Windows:
                    _transInstance = new TransPCResource();
                    break;
                default:
                    _transInstance = new TransPCResource();
                    break;
            }

            _transInstance.SetUnzipPath(sourcePath, _storeDir, TransResourceFinish);
        }

        //强制转移资源，因为要更新资源，不管是基础资源还是补丁资源
        public void ForceTransWhenNeedUpdate()
        {
            _forceTrans = true;
        }

        //强制做资源释放, 在ios通过游戏内更新分段资源时调用
        public void SetForceUnzip()
        {
            _forceTrans = true;
        }

        public override void OnEnter(BaseFlow oldFlow)
        {
            base.OnEnter(oldFlow);

            //转移过资源，xml就从storePath取。否则从包内取
            ChangeLocalXmlPath(HasTransedResource());
        }

        public override int Work()
        {
            if (!CheckLastFlowResult()) return LastFlowResult;

            int ret = CodeDefine.RET_INIT;
            if (!checkNeedTrans())
            {
                ret = CodeDefine.RET_SUCCESS;
            }
            else
            {
                //如果有老的xml，需要先保存数据
                LocalVersionXml oldXml = getOldXmlData(_storedLocalXmlPath);

                _transInstance.StartUnzipByThread();

                bool success = _transInstance.GetTransReslult();

                if (success)
                {
                    //将老数据：分段版本、补丁版本、app版本填充的新的xml中
                    updateLocalXmlData(oldXml, _storedLocalXmlPath, true);
                    ChangeLocalXmlPath(true);
                }

                ret = success ? CodeDefine.RET_SUCCESS : CodeDefine.RET_FAIL_TRANS_FAIL;
                _transInstance.CallFinish(success);
                _forceTrans = false;
            }
            return ret;
        }

        public override void Uninitialize()
        {
            LocalXml = null;
        }

        public override void GetCurDownInfo(out string url, out int total, out int downloaded)
        {
            base.GetCurDownInfo(out url, out total, out downloaded);
            url = "";
            total = _transInstance.GetTotalValue();
            downloaded = _transInstance.GetCurrentProgress();
        }

        public void TransResourceFinish(bool result)
        {

        }

        //判断是否需要转移资源
        private bool checkNeedTrans()
        {
            UpdateLog.DEBUG_LOG("检查是否需要转移资源");
            bool ret = false;
            LocalVersionXml localXml = getOldXmlData(_storedLocalXmlPath);
            string storedAppVersion = localXml == null ? "" : localXml.LocalAppVersion;
            string hasCopy = localXml == null ? "" : localXml.HasCopy;

            //存储的app版本号不存在（没释放过资源, 本地LocalVersion.xml不存在）
            if (string.IsNullOrEmpty(storedAppVersion))
            {
                if (_inAppBaseVersion == "0")
                {
                    //同时包内分段号为0，则说明是下载器，需要强制释放资源
                    ret = true;
                    UpdateLog.DEBUG_LOG("inAppBaseVerson==0，需要强制转移资源");
                }
                else if (_platformType == PlatformType.IOS && !_forceTrans)
                {
                    string inAppXmlAppVer = getIOSAppVerInXml(_inAppLocalXmlPath);
                    if (!string.IsNullOrEmpty(inAppXmlAppVer) && _inAppClientVersion.CompareTo(inAppXmlAppVer) > 0) 
                    {
                        ret = true;
                        UpdateLog.DEBUG_LOG("IOS平台，包内版本比包内xml中app版本更高，需要转移资源");
                    }
                    else
                    {
                        //没释放过资源，并且是ios平台，分段号又不为0，说明包内有资源可以运行，不做资源释放
                        ret = false;
                        UpdateLog.DEBUG_LOG("游戏启动，IOS平台，跳过转移资源");
                    }
                }
                else
                {
                    ret = true;
                    UpdateLog.DEBUG_LOG("需要转移资源");
                }
            }
            else if (_inAppClientVersion.CompareTo(storedAppVersion) > 0 || hasCopy.ToLower() != _hasCopyTag)
            {
                UpdateLog.DEBUG_LOG(string.Format("_inAppClientVersion={0} storedAppVersion={1} hasCopy={2}", _inAppClientVersion, storedAppVersion, hasCopy));
                //包内版本比本地版本要大，说明是新客户端，需要做新资源释放覆盖
                ret = true;
                UpdateLog.DEBUG_LOG("客户端更新或者上次转移失败，需要重新转移资源");
            }
            else
                UpdateLog.DEBUG_LOG("不需要转移资源");

            return ret;
        }

        private LocalVersionXml getOldXmlData(string oldXmlPath)
        {
            LocalVersionXml xml = null;
            if (File.Exists(oldXmlPath))
            {
                xml = new LocalVersionXml();
                xml.parseLocalVersionXml(oldXmlPath);
            }

            return xml;
        }

        private string getIOSAppVerInXml(string localXmlPath)
        {
            string appVer = "";
            if (!string.IsNullOrEmpty(localXmlPath) && File.Exists(localXmlPath))
            {
                var xml = new LocalVersionXml();
                xml.parseLocalVersionXml(localXmlPath);
                appVer = xml.LocalAppVersion;
            }

            return appVer;
        }

        private void updateLocalXmlData(LocalVersionXml oldXml, string newXmlPath, bool transed)
        {
            if (File.Exists(newXmlPath))
            {
                LocalVersionXml newXml = new LocalVersionXml();
                newXml.parseLocalVersionXml(newXmlPath);
                if (oldXml != null)
                {
                    newXml.save(oldXml.BaseResVersion, oldXml.PatchResVersion, "", _inAppClientVersion);
                }
                
                if (transed)
                {
                    newXml.save("", "", _hasCopyTag);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading;
using UpdateSystem.Xml;
using UpdateSystem.Log;
using UpdateSystem.Enum;
using UpdateSystem.Delegate;

namespace UpdateSystem.Trans
{
    /// <summary>
    /// UILoginFormScript的逻辑处理类
    /// </summary>
    public class TransManager : SingleInstance<TransManager>
    {
        //解析本地xml
        public LocalVersionXml LocalXml;

        //包内的app版本，比如在Resource目录，需要通过Resources.Load来获取
        private string _inAppClientVersion;
        //包内的分段版本号
        private string _inAppBaseVersion;
        //平台类型
        private PlatformType _platformType;
        //资源转移功能实例
        private TransResource _transInstance;
        //本地的localVersion.xml路径
        private string _localXmlPath;

        /// <summary>
        /// 强制转移资源
        /// 1. ios在进入游戏后有资源更新，会返回到更新界面做资源转移
        /// 2. 
        /// </summary>
        private bool _forceTrans = false;

        public void SetTransData(string localXmlPath, string sourcePath, string storeDir, string inAppClientVersion, string inAppBaseVersion,
            PlatformType type, TransResourceFinishCallback callback)
        {
            _localXmlPath = localXmlPath;
            _inAppClientVersion = inAppClientVersion;
            _inAppBaseVersion = inAppBaseVersion;
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

            _transInstance.SetUnzipPath(sourcePath, storeDir, callback);
        }

        public void StartTrans()
        {
            {
                _forceTrans = false;
                if (_transInstance != null)
                {
                    _transInstance.StartUnzipByThread();
                }
                else
                    UpdateLog.ERROR_LOG("_transInstance 没有初始化");
            }
        }

        //强制做资源转移
        public void SetForceTrans()
        {
            _forceTrans = true;
        }

        public bool IsForceTrans()
        {
            return _forceTrans;
        }
    }
}
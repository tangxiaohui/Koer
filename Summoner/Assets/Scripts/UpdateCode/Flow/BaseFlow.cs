using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UpdateSystem.Xml;
using UpdateSystem.Log;
using UpdateSystem.Data;
using UpdateSystem.Delegate;
using UpdateSystem.Enum;

namespace UpdateSystem.Flow
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseFlow
    {
        public bool ConfirmAfterPause;
        //当前流程的结果
        public int CurrentFlowResult;
        //是否使用了http下载，用于显示进度的时候转换下载速度
        public bool UseDownload;

        /// <summary>
        /// 流程名字
        /// </summary>
        public FlowEnum FlowType;
        //修复列表
        public List<MapFileData> RepairList;
        //暂停，等待确认
        private bool _pause;
        
        //初始化
        private bool _initialized;
        //最近一次流程结果
        protected int _recentResult = CodeDefine.RET_INIT;

        #region 静态数据，版本、路径等
        //正在使用的xml路径
        protected static string _localXmlPath;
        //包内localXml路径, 主要是ios有释放资源和不释放资源的路径访问问题
        protected static string _inAppLocalXmlPath;
        //本地localXml路径（释放出去的路径）
        protected static string _storedLocalXmlPath;
        //包内的app版本和分段版本
        protected static string _inAppClientVersion;
        protected static string _inAppBaseVersion;
        //本地保存资源的根路径
        protected static string _storeDir;
        protected static string _appDir;
        //是否支持提示下载
        private static bool _enablePause = true;

        //调用下载提示，弹出提示框
        private static DownloadNoticeCall _callDownloadNotice;
        //每个流程结束调用
        private static ActionCall _perFlowActionCall;
        //已经转移过资源的标签值
        protected const string _hasCopyTag = "yes";
        #endregion

        #region //上一个流程传递到下一个流程的数据,没有值的就不传
        //继续后的localXml数据
        public LocalVersionXml LocalXml;
        //RemoteXml解析数据
        public ResourceVersionXml RemoteXml;
        //正式流程或者测试流程，由UseTestFlow决定
        public DataModel CurrentRemoteData;
        //客户端下载存放地址
        public string ApkStorePath;
        //解析的map文件数据
        public List<MapFileData> MapFileDataListForDownload;
        //强制检查md5的文件列表
        public List<string> ForceCheckMd5List;
        //本地分段号
        public string LocalBaseResVersion;
        //上一个流程的结果
        public int LastFlowResult = CodeDefine.RET_SUCCESS;

        #endregion


        public BaseFlow()
        {
            Inititalize();
        }


        /// <summary>
        /// 设置基础路径
        /// </summary>
        /// <param name="storedLocalXmlPath">存储LocalVersion.xml的绝对路径</param>
        /// <param name="inAppLocalXmlPath">包内LocalVersion.xml的绝对路径</param>
        /// <param name="storedPath">释放/存储资源的根路径</param>
        public static void SetPath(string storedLocalXmlPath, string inAppLocalXmlPath, string storedPath, string appPath)
        {
            _localXmlPath = storedLocalXmlPath;
            _storedLocalXmlPath = storedLocalXmlPath;
            _inAppLocalXmlPath = inAppLocalXmlPath;
            _storeDir = storedPath;
            _appDir = appPath;
            CreateDir(storedPath);
        }

        /// <summary>
        /// 设置包内保存的相关版本
        /// </summary>
        /// <param name="inAppClientVer">包内的客户端版本</param>
        /// <param name="inAppBaseVer">包内分段版本</param>
        public static void SetInAppVer(string inAppClientVer, string inAppBaseVer)
        {
            _inAppClientVersion = inAppClientVer;
            _inAppBaseVersion = inAppBaseVer;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="storedLocalXmlPath">存储在指定位置的LocalVersion.xml路径</param>
        /// <param name="inAppLocalXmlPath">存储在app中的LocalVersion.xml路径</param>
        /// <param name="storeDir">指定存储资源的根路径</param>
        public virtual void Inititalize()
        {
            _initialized = true;
        }

        /// <summary>
        /// 做数据拷贝，将上一个流程的数据转到当前流程
        /// </summary>
        /// <param name="oldFlow"></param>
        public virtual void OnEnter(BaseFlow oldFlow)
        {
            if (!_initialized)
            {
                //Inititalize();
            }

            if (_perFlowActionCall != null)
            {
                _perFlowActionCall(null);
            }

            if (oldFlow == null)
            {
                return;
            }

            //数据拷贝
            if (oldFlow.LocalXml != null) LocalXml = oldFlow.LocalXml;
            if (oldFlow.RemoteXml != null) RemoteXml = oldFlow.RemoteXml;
            if (oldFlow.CurrentRemoteData != null) CurrentRemoteData = oldFlow.CurrentRemoteData;
            if (!string.IsNullOrEmpty(oldFlow.LocalBaseResVersion)) LocalBaseResVersion = oldFlow.LocalBaseResVersion;
            if (oldFlow.MapFileDataListForDownload != null) MapFileDataListForDownload = oldFlow.MapFileDataListForDownload;
            if (oldFlow.ForceCheckMd5List != null) ForceCheckMd5List = oldFlow.ForceCheckMd5List;

            LastFlowResult = oldFlow.CurrentFlowResult;
        }


        /// <summary>
        /// 执行当前流程
        /// </summary>
        /// <returns></returns>
        public virtual int Work()
        {
            return 0;
        }

        /// <summary>
        /// 当前流程结束，保存结果
        /// </summary>
        /// <param name="ret"></param>
        public virtual void OnLeave(int ret)
        {
            CurrentFlowResult = ret;
            _recentResult = ret;
        }

        //获取下载信息，链接、大小、已下载
        public virtual void GetCurDownInfo(out string url, out int total, out int downloaded)
        {
            url = "";
            total = 0;
            downloaded = 0;
        }

        public virtual void Uninitialize()
        {

        }

        public virtual void Abort()
        {
            Resume(false);
        }

        public virtual bool Pause(int size = 0)
        {
            ConfirmAfterPause = false;
            _pause = true;

            if (!_enablePause)
            {
                return true;
            }

            if (_callDownloadNotice != null)
            {
                _callDownloadNotice(size);
            }

            //等待用户确认
            while (_pause)
            {
                UpdateLog.DEBUG_LOG("有下载，等待用户确认");
                Thread.Sleep(1000);
            }

            return ConfirmAfterPause;
        }

        public virtual void Resume(bool continueFlow)
        {
            _pause = false;
            ConfirmAfterPause = continueFlow;
        }

        /// <summary>
        /// 检查上一个流程的结果
        /// </summary>
        /// <returns>true: 继续执行  false: 跳过</returns>
        public bool CheckLastFlowResult()
        {
            //失败或者跳过
            if (LastFlowResult < CodeDefine.RET_SUCCESS || LastFlowResult == CodeDefine.RET_SKIP_BY_CANCEL)
            {
                UpdateLog.DEBUG_LOG("Skip flow: " + GetType().Name);
                return false;
            }
            return true;
        }

        //是否已经转移过资源了
        public static bool HasTransedResource()
        {
            if (File.Exists(_storedLocalXmlPath))
            {
                var localXml = new LocalVersionXml();
                localXml.parseLocalVersionXml(_storedLocalXmlPath);
                return localXml.HasCopy.ToLower() == _hasCopyTag;
            }
            return false;
        }

        /// <summary>
        /// 切换xml的读取路径
        /// </summary>
        /// <param name="storedPath">外部存储路径</param>
        public void ChangeLocalXmlPath(bool storedPath)
        {
            if (storedPath)
            {
                _localXmlPath = _storedLocalXmlPath;
            }
            else
                _localXmlPath = _inAppLocalXmlPath;
        }

        public string FlowName()
        {
            return GetType().Name;
        }


        public static void CreateDir(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        /// <summary>
        /// 在有资源下载时，支持暂停更新流程，提示用户下载资源
        /// 某些平台强制要求提示玩家有下载
        /// </summary>
        /// <param name="enable"></param>
        public static void SetEnablePause(bool enable)
        {
            _enablePause = enable;
        }

        /// <summary>
        /// 有下载需要弹出提示框，在这里做处理
        /// </summary>
        /// <param name="func"></param>
        public static void SetDownloadNoticeCallback(DownloadNoticeCall func)
        {
            _callDownloadNotice = func;
        }

        /// <summary>
        /// 每个流程结束时调用
        /// </summary>
        /// <param name="func"></param>
        public static void SetPerFlowActionCallback(ActionCall func)
        {
            _perFlowActionCall = func;
        }

        public static string GetStorePath()
        {
            return _storeDir;
        }
    }
}

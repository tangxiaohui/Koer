using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Update.Platform;
using UpdateSystem.Data;
using UpdateSystem.Delegate;
using UpdateSystem.Download;
using UpdateSystem.Enum;
using UpdateSystem.Flow;
using UpdateSystem.Log;

namespace UpdateSystem.Manager
{
    public partial class UpdateManager  : SingleInstance<UpdateManager>
    {
		public Flow5DownloadBaseRes _Flow5DownloadBaseRes = null;
        private string _localXmlPath;
        private PlatformType _platformType;
        private string _storePath;
        private string _appPath;

        #region //预加载资源，显示进度用的
        //开启预加载，显示进度用
        private bool _showPreloadRes;
        //总共需要预加载的资源数
        private int _totalPreloadRes;
        //已经加载好的资源数
        private int _loadedRes;
        #endregion

        //是否初始化
        private bool _initialized;
        //流程是否结束，仅用来判断线程结束
        private bool _threadFinish = true;
        //重新开始，为true则在所有流程结束的时候重新开启整个流程
        private bool _restart;
        //中断流程
        private bool _abortFlows;

        //场景资源列表配置文件路径
        private string _sceneConfigPath;

        public BaseFlow CurrentFlow;

        //流程列表
        private List<BaseFlow> _flowList;
        //流程结束调用
        private FinishCallback _onFinish;
		private Thread thread = null;

        //重新开始，为true则在所有流程结束的时候重新开启整个流程
        private static MyRuntimePlatform _runtimePlatform = MyRuntimePlatform.StandaloneEditor;



		public void Initialize(string storedLocalXmlPath, string inAppLocalXmlPath, string appPath, string storePath,PlatformType platformtype, int halfCpuCoreCount = 4)
        {
            if(_initialized)
            {
                return;
            }
            _localXmlPath = storedLocalXmlPath;
            this._threadFinish = true;
            this._localXmlPath = storedLocalXmlPath;
            this._storePath = storePath;
            this._appPath = appPath;
			this._platformType = platformtype;

            this._flowList = new List<BaseFlow>();
            //整包下载流程
            /*this._flowList.Add(new Flow1TransResource());
            this._flowList.Add(new Flow2LocalXml());
            this._flowList.Add(new Flow3RemoteXml());
            this._flowList.Add(new Flow4DownloadClient());
            //_flowList.Add(new Flow5DownloadBaseRes());
            //_flowList.Add(new Flow6ReleaseBaseRes());
            //_flowList.Add(new Flow7DownloadMapFile());
             this._flowList.Add(new Flow7ExDownloadMapFile());
            //_flowList.Add(new Flow8CheckResource());
            this._flowList.Add(new Flow8ExCheckResource());
            this._flowList.Add(new Flow9RepairResource());
            this._flowList.Add(new FlowFinish());
            this._initialized = true;*/

			//分段下载流程
			this._flowList.Add(new Flow1TransResource());
            this._flowList.Add(new Flow2LocalXml());
            this._flowList.Add(new Flow3RemoteXml());
            this._flowList.Add(new Flow4DownloadClient());
			_Flow5DownloadBaseRes = new Flow5DownloadBaseRes();
			this._flowList.Add(_Flow5DownloadBaseRes);
			this._flowList.Add(new Flow6ReleaseBaseRes());
			_flowList.Add(new Flow7ExDownloadMapFile());
			this._flowList.Add(new Flow8CheckResource());
            this._flowList.Add(new Flow9RepairResource());
            this._flowList.Add(new FlowFinish());
            this._initialized = true;

            BaseFlow.SetPath(storedLocalXmlPath, inAppLocalXmlPath, storePath, appPath);
            InitBackDownload(halfCpuCoreCount);
        }

		public void CloseThread()
		{
			_threadFinish = true;
			if (thread != null) {
				thread.Abort ();
				thread = null;
			}
		}

        public void StartUpdate()
        {
            //未初始化
            if (!_initialized) return;
            //未完成
            if (!_threadFinish) return;

            //需要先调用一次，避免UI线程的函数在子线程中调用了
            Update();

            _threadFinish = false;
            _restart = false;
			if (thread != null) {
				thread.Abort ();
				thread = null;
			}
			thread = new Thread(updateFlowByThread);
            thread.Start();
        }

        /// <summary>
        /// 在线程中执行更新流程
        /// </summary>
        private void updateFlowByThread()
        {
            bool continueFlow = false;
            CurrentFlow = null;
            _abortFlows = false;
            int resultCode = CodeDefine.RET_SUCCESS;
            int runFlowCount = 0;
            while (true && !_abortFlows)
            {
                BaseFlow oldFlow = null;
                continueFlow = false;
                runFlowCount = 0;
                for (int i = 0; !_abortFlows && i < _flowList.Count; ++i)
                {
                    runFlowCount++;
                    CurrentFlow = _flowList[i];
                    UpdateLog.DEBUG_LOG(CurrentFlow.FlowName());

                    CurrentFlow.OnEnter(oldFlow);
                    resultCode = CurrentFlow.Work();
                    CurrentFlow.OnLeave(resultCode);

                    //更新客户端，跳过所有流程
                    if (resultCode == CodeDefine.RET_SKIP_BY_DOWNLOAD_APP)
                    {
                        UpdateLog.DEBUG_LOG("Download Client finish, skip all left flows!!!");
                        break;
                    }

                    //需要强制释放资源，重新走更新流程
                    if (resultCode == CodeDefine.RET_SKIP_BY_FORCE_TRANS_RESOURCE)
                    {
                        FlowInstance<Flow1TransResource>().SetForceUnzip();
                        continueFlow = true;
                        break;
                    }

                    //中断操作
                    if (resultCode == CodeDefine.RET_SKIP_BY_ABORT)
                    {
                        UpdateLog.DEBUG_LOG("Abort flow -> " + CurrentFlow.FlowName());
                        break;
                    }

                    if (resultCode == CodeDefine.RET_SKIP_BY_DISABLEDOWNLOAD)
                    {
                        UpdateLog.DEBUG_LOG("Not support download, skip all flows!!!");
                        break;
                    }

                    //取消操作
                    if (resultCode == CodeDefine.RET_SKIP_BY_CANCEL)
                    {
                        UpdateLog.DEBUG_LOG("Skip flow by cancel download option, exit game!!!");
                        break;
                    }

                    if (resultCode < CodeDefine.RET_SUCCESS)
                    {
                        break;
                    }

                    oldFlow = CurrentFlow;
                }

                if (!continueFlow)
                    break;
            }

            if (runFlowCount != _flowList.Count)
            {
                FlowInstance<FlowFinish>().FinishWithError(resultCode);
            }

            _threadFinish = true;

            //重新开启
            if (_restart)
            {
                UpdateLog.DEBUG_LOG("Restart update");
                StartUpdate();
            }
            else
                UpdateLog.DEBUG_LOG("Finish update flow!!! " + resultCode);
        }

        /// <summary>
        /// 获取下载信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="total"></param>
        /// <param name="progressValue"></param>
        public void GetDownloadInfo(out string url, out int total, out int progressValue, out bool isDownloadProgress)
        {
            isDownloadProgress = CurrentFlow.UseDownload;
            CurrentFlow.GetCurDownInfo(out url, out total, out progressValue);
        }

        /// <summary>
        /// 更新流程是否正在运行
        /// </summary>
        /// <returns></returns>
        public bool Running()
        {
            return !_threadFinish;
        }

        /// 日志回调注册
        /// </summary>
        /// <param name="log1"></param>
        /// <param name="log2"></param>
        /// <param name="log3"></param>
        public void RegisterLog(DefaultLog log1, WarnLog log2, ErrorLog log3)
        {
            UpdateLog.RegisterLogCallback(log1, log2, log3);
        }

        /// <summary>
        /// 测试流程要用到的数据
        /// </summary>
        /// <param name="imeiOrMacOrIdfa">imei、mac地址、idfa</param>
        /// <param name="ip">ip地址，基本没用了</param>
        public void SetImeiOrMacOrIdfa(string imeiOrMacOrIdfa)
        {
            UpdateLog.WARN_LOG("UpdateManager ---> SetImeiOrMacOrIdfa() ");
            if (this._initialized)
            {
                this.FlowInstance<Flow3RemoteXml>().SetExternalData(imeiOrMacOrIdfa, "");
            }
        }

        /// <summary>
        /// 每个流程开始时调用，用来记录流程进程
        /// </summary>
        /// <param name="callback"></param>
        public void SetOnFlowBeginCallback(ActionCall callback)
        {
            if (!_initialized) return;
            convertActionCall(callback);
            BaseFlow.SetPerFlowActionCallback(onActionCall);
        }

        /// <summary>
        /// 设置转移资源是要用到的数据
        /// </summary>
        /// <param name="inAppClientVersion">包内app版本</param>
        /// <param name="inAppBaseVersion">包内分段版本</param>
        /// <param name="appPath">app安装路径，ios是var目录，apk是/data/data目录</param>
        public void SetTransData(string inAppClientVersion, string inAppBaseVersion, string appPath)
        {
            if (!_initialized) return;
            BaseFlow.SetInAppVer(inAppClientVersion, inAppBaseVersion);
            FlowInstance<Flow1TransResource>().SetExternalData(appPath, _platformType);
        }

        /// <summary>
        /// 设置备份的cdn列表，当前cdn下载失败后，自动转到下一个cdn下载
        /// </summary>
        /// <param name="backupCdnArray"></param>
        public void SetBackupCdn(string[] backupCdnArray)
        {
            UpdateSystem.Download.Download.BackupCdn = backupCdnArray;
        }

        /// <summary>
        /// 设置场景文件关联的资源列表的配置文件路径：SceneConfig
        /// </summary>
        /// <param name="path"></param>
        public void SetSceneReferenceResConfigPath(string dir)
        {
            _sceneConfigPath = dir;
        }

        /// <summary>
        /// 客户端下载完成后的回调：安卓是直接安装，IOS是打开网页
        /// 自定义下载客户端的方法，ios不下载，windows不下载，一般设为null
        /// </summary>
        /// <param name="func1"></param>
        public void SetClientDownClientFunc(ClientDownloadFinishCallback onFinish, CustomDownClientFunc customFunc = null)
        {
            if (!_initialized) return;
            convertFuncClientDownloadFinishCallback(onFinish);
            FlowInstance<Flow4DownloadClient>().SetExternalData(onClientDownloadFinishCallback, customFunc, _platformType == PlatformType.IOS);
        }

        /// <summary>
        /// 有资源需要下载时，要提示给用户，这个函数是通知弹出提示框用的
        /// </summary>
        /// <param name="func"></param>
        public void SetDownloadNoticeFunc(DownloadNoticeCall func)
        {
            if (!_initialized) return;
            convertFuncDownloadNoticeCall(func);
            BaseFlow.SetDownloadNoticeCallback(onDownloadNoticeCall);
        }

        /// <summary>
        /// 流程结束的回调
        /// </summary>
        /// <param name="func"></param>
        public void SetFinishCallback(FinishCallback func)
        {
            if (!_initialized) return;
            convertFuncFinishCallback(func);
            _onFinish = onFinishCallback;
            FlowInstance<FlowFinish>().SetExternalData(onFinishCallback);
        }

        /// <summary>
        /// 获取本地版本信息
        /// </summary>
        /// <param name="appVer">本地app版本</param>
        /// <param name="resVer">本地资源版本</param>
        public void GetVersionInfo(out string appVer, out string resVer)
        {
            appVer = "";
            resVer = "";
            if (CurrentFlow != null && CurrentFlow.LocalXml != null)
            {
                appVer = CurrentFlow.LocalXml.LocalAppVersion;
                resVer = CurrentFlow.LocalXml.PatchResVersion;
            }
        }

        /// <summary>
        /// 远端最新app版本
        /// </summary>
        /// <returns></returns>
        public string GetRemoteAppVersion()
        {
            return CurrentFlow.CurrentRemoteData.AppVersion;
        }

        /// <summary>
        /// 远端最新资源版本
        /// </summary>
        /// <returns></returns>
        public string GetRemoteResVersion()
        {
            return CurrentFlow.CurrentRemoteData.PatchVersion;
        }

        /// <summary>
        /// 流程类型
        /// </summary>
        /// <returns></returns>
        public FlowEnum GetCurFlowType()
        {
            var flowName = CurrentFlow.FlowName();
            if (System.Enum.IsDefined(typeof(FlowEnum), flowName))
            {
                return (FlowEnum)System.Enum.Parse(typeof(FlowEnum), flowName);
            }
            return CurrentFlow.FlowType;
        }

        /// <summary>
        /// 设置预加载信息，用来显示进度
        /// </summary>
        /// <param name="total"></param>
        /// <param name="loadedCount"></param>
        public void SetPreloadTotal(int total)
        {
            UpdateLog.WARN_LOG("start preload+++++++++++++");
            _showPreloadRes = true;
            _totalPreloadRes = total;
        }

        /// <summary>
        /// 已经预加载好的数量
        /// </summary>
        /// <param name="loaded"></param>
        public void SetPreloadedCount(int loaded)
        {
            _loadedRes = loaded;
        }



        public T FlowInstance<T>() where T : BaseFlow
        {
            for (int i = 0; i < _flowList.Count; ++i)
            {
                if (_flowList[i] is T)
                {
                    return (T)_flowList[i];
                }
            }

            return default(T);
        }

        /// <summary>
        /// 获取客户端下载地址，在Flow3后才能获取
        /// </summary>
        /// <returns></returns>
        public string GetClientUrl()
        {
            if (CurrentFlow != null && CurrentFlow.CurrentRemoteData != null)
                return CurrentFlow.CurrentRemoteData.ClientUrl;

            return "";
        }

        /// <summary>
        /// 返回apk存放位置，仅安卓平台可用
        /// </summary>
        /// <returns></returns>
        public string GetApkPath()
        {
            if (CurrentFlow != null)
                return CurrentFlow.ApkStorePath;
            return "";
        }


        public static void SetRuntimePlatform(int runtimePlatformID)
        {
            _runtimePlatform = (MyRuntimePlatform)runtimePlatformID;
        }

        public MyRuntimePlatform GetRuntimePlatform()
        {
            return _runtimePlatform;
        }

        /// <summary>
        /// 在中断所有流程后重新开始
        /// </summary>
        public void Restart()
        {
            AbortFlows();
            if (Running())
            {
                _restart = true;
            }
            else
                StartUpdate();
        }

        /// <summary>
        /// 中断更新流程
        /// </summary>
        public void AbortFlows()
        {
            _abortFlows = true;
            if (CurrentFlow != null)
            {
                CurrentFlow.Abort();
                UpdateLog.WARN_LOG("中断流程 " + CurrentFlow.FlowName());
            }

            UpdateSystem.Download.BackDownload.AbortAll(null);
        }
    }

}
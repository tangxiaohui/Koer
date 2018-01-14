using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UpdateSystem.Enum;
using UpdateSystem.Flow;
using UpdateSystem.Manager;
using UpdateForm.Center;
using UpdateForm.Hardware;
using UpdateSystem;
using UpdateForm.Form;
using Update.Platform;
using System;
using System.Reflection;

public class UIUpdateForm : MonoBehaviour
{
	public bool bDownBaseRes = false;
	public bool bBackGround = false;

    //刷新频率
    private const float REFRESH_RATE = 0.3f;
    //刷新频率
    private float _refreshDelta = 0f;
    //每秒倒计时
    private float _perSecondTimer;
    private int _speed = 0;
    //旧进度，用来计算下载速度的
    private int _oldProgressValue;
    private PlatformType _pType = PlatformType.Windows;

    //释放资源存放路径
    private static string _storePath = "";
    //释放后的xml读取路径，通过_storePath和_localXmlRelativePath组合
    private static string _localXmlAbsPath = "";

    //TODO::Update 接入的时候可能需要修改这里的路径
    //资源读取相对路径
    private static string _localXmlRelativePath = "Config/LocalVersion.xml";
    //场景关联文件列表的配置表所在路径
    private static string _sceneConfigPath = "Assets/GameAssets/Resources/SceneConfig";


    UIText _StepLabel = null;
    UIImage _Progress = null;
    UIText _PercentageLabel = null;
    UIText _ProgressMinToMaxLabel = null;
    private void DebugLog(string log)
    {
        string msg = "\r\nDebugLog: " + log;
        UnityEngine.Debug.Log(msg);
    }

    private void WarnLog(string log)
    {
        string msg = "\r\nWarnLog: " + log;
        UnityEngine.Debug.LogWarning(msg);
    }


    private void ErrorLog(string log)
    {
        string msg = "\r\nErrorLog: " + log;
        UnityEngine.Debug.LogError(msg);

    }


    void Awake()
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        _storePath = Application.persistentDataPath;

        _pType = PlatformType.Windows;

        switch (LauncherUpdate.RuntimePlatform)
        {
            case MyRuntimePlatform.AndroidEditor:
            case MyRuntimePlatform.IOSEditor:
            case MyRuntimePlatform.StandaloneEditor:
                _storePath = Application.streamingAssetsPath;
                UpdateSystem.Trans.TransAndroidResource.ForTest = true;
                break;
            case MyRuntimePlatform.Android:
                _storePath = Application.persistentDataPath;
                _pType = PlatformType.Android;
                break;
            case MyRuntimePlatform.IOS:
                _storePath = Application.persistentDataPath;
                _pType = PlatformType.IOS;
                break;
            case MyRuntimePlatform.Standalone:
                _storePath = Application.streamingAssetsPath;
                _pType = PlatformType.Windows;
                break;
            default:
#if UNITY_EDITOR
                //正式接入的时候要去掉这个
                UpdateSystem.Trans.TransAndroidResource.ForTest = true;
                _storePath = Application.streamingAssetsPath;//Resource目录
#endif
#if UNITY_ANDROID
                    _pType = PlatformType.Android;
#elif UNITY_IPHONE
                    _pType = PlatformType.IOS;
#else
                _pType = PlatformType.Windows;
#endif
                break;
        }

        _localXmlAbsPath = _storePath + "/" + _localXmlRelativePath;

        FindAllComponents();
        RegUICallback();
    }

    private void FindAllComponents()
    {
        _StepLabel = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "ContentText");
        _PercentageLabel = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "SpeedText");
        _ProgressMinToMaxLabel = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "MinMaxText");
        _StepLabel.text = string.Empty;
        _PercentageLabel.text = string.Empty;
        _ProgressMinToMaxLabel.text = string.Empty;
        _Progress =  Utility.GameUtility.FindDeepChild<UIImage>(gameObject, "Process");
       // Debug.Break();
    }

    /// <summary>
    /// 注册UI上面的事件，比如点击事件等
    /// </summary>
    private void RegUICallback()
    {

    }

    void Start()
    {
        if(!DevelopSetting.HotFix)
        {
            EnterGame();
            return;
        }
        //判断网络状态
        if (LauncherUpdate.IsNetworkEnable())
        {
            if (LauncherUpdate.Is4G())
            {
                UIMsgBoxForm.Open("当前不是WIFI网络，可能会产生流量，确定要继续吗？", "继续", "退出", StartUpdate, ExitGame);
            }
            else
            {
                StartUpdate();
            }
        }
        else
        {
            UIMsgBoxForm.Open("当前网络不可用，请检查网络", "退出", null, ExitGame);
        }
    }

    private void ExitGame()
    {
        //if (!PlatformInterface.ExitGame())
        {
            Application.Quit();
        }
    }
    public void StartUpdate()
    {
        DebugLog("StartUpdate++++");
		Common.ULogFile.sharedInstance.LogEx("update", "StartUpdate++++");
        if (UpdateManager.Instance.Running())
        {
            DebugLog("running!!! return");
            return;
        }

        string localXmlPath = _localXmlAbsPath;
        string storeDir = _storePath;
        //进入测试流程用到的数据
        string imeiOrMacOrIdfa = HardwareManager.GetMacOrImeiOrIdfa();
        //打包的时候写在代码里面的版本
        string inAppClientVersion = BuildVersion.AppVer;
        //打包的时候写在代码里面的分段版本,固定为1
        string inAppBaseVersion = "1";
        //apk或者ipa所在路径，安装路径。Unity中通过Application.streamingAssetsPath获取
        string installedPath = "";
        //IOS需要读取包内xml的地址
        string inAppLocalXmlPath = localXmlPath.Replace(storeDir, Application.streamingAssetsPath);
        if (_pType == PlatformType.IOS || _pType == PlatformType.Windows)
        {
            //Application/xxxxx/xxx.app/Data/Raw
            installedPath = Application.streamingAssetsPath;
        }
        else if (_pType == PlatformType.Android)
        {
            ///data/app/xxx.xxx.xxx.apk
            installedPath = Application.dataPath;
        }
        DebugLog(string.Format("localXmlPath={0} \ninAppLocalXmlPath={1} \nstoreDir={2}", localXmlPath, inAppLocalXmlPath, storeDir));
        string[] backupCdnArray = null;
        int cpuCoreCount = SystemInfo.processorCount;
        if (cpuCoreCount <= 0) cpuCoreCount = 4;
        cpuCoreCount = cpuCoreCount / 2;


        UpdateManager.Instance.Initialize(localXmlPath, inAppLocalXmlPath, installedPath, storeDir, _pType, cpuCoreCount);
        //注册日志回调
        UpdateManager.Instance.RegisterLog(DebugLog, WarnLog, ErrorLog);
        //设置进入测试流程的条件
        UpdateManager.Instance.SetImeiOrMacOrIdfa(imeiOrMacOrIdfa);
        //设置资源转移的路径和版本信息
        UpdateManager.Instance.SetTransData(inAppClientVersion, inAppBaseVersion, installedPath);
        //设置备份的cdn，主要是台湾版本要用
        UpdateManager.Instance.SetBackupCdn(backupCdnArray);
        //设置客户端下载完成的回调
        UpdateManager.Instance.SetClientDownClientFunc(OnClientDownloadFinish, null);
        //设置下载提示的回调  
        UpdateManager.Instance.SetDownloadNoticeFunc(OnDownloadNotice);
        //设置每个流程的回调
        UpdateManager.Instance.SetOnFlowBeginCallback(onFlowBeginCallback);
        //设置更新流程结束的回调函数
        UpdateManager.Instance.SetFinishCallback(FinishCallback);
        //场景关联资源列表的配置文件
        UpdateManager.Instance.SetSceneReferenceResConfigPath(_sceneConfigPath);
		UpdateManager.Instance._Flow5DownloadBaseRes.SetEnableDownBase (UpdateCenter.bDownBaseRes);
		UpdateManager.Instance._Flow5DownloadBaseRes.SetBackDownload (UpdateCenter.bBackGround);
        //开始更新流程
        UpdateManager.Instance.StartUpdate();

        DebugLog("StartUpdate---");
    }

    /// <summary>
    /// 每个流程开始时调用
    /// </summary>
    /// <param name="obj"></param>
    private void onFlowBeginCallback(object obj)
    {
        refreshProgress();

        string appVers = "";
        string resVers = "";
        UpdateManager.Instance.GetVersionInfo(out appVers, out resVers);
   //     _AppVer.text = appVers;
    //    _ResVer.text = resVers;

        if (UpdateManager.Instance.CurrentFlow == null) return;

        string labelText = "";
        FlowEnum flowType = UpdateManager.Instance.GetCurFlowType();
        UnityEngine.Debug.LogWarning("Cur flow : " + flowType);
        switch (flowType)
        {
            case FlowEnum.Flow1TransResource:
                labelText = "转移资源（不消耗流量）";
                break;
            case FlowEnum.Flow2LocalXml:
            case FlowEnum.Flow3RemoteXml:
                labelText = "资源初始化";
                break;
            case FlowEnum.Flow4DownloadClient:
                string newVer = UpdateManager.Instance.CurrentFlow.CurrentRemoteData.AppVersion;
                labelText = "更新客户端版本" + "(AppVer:" + newVer + ")";
                break;
            case FlowEnum.Flow5DownloadBaseRes:
                labelText = "更新分段资源";
                break;
            case FlowEnum.Flow6ReleaseBaseRes:
                labelText = "释放资源（不消耗流量）";
                break;
            case FlowEnum.Flow7ExDownloadMapFile:
            case FlowEnum.Flow8CheckResource:
            case FlowEnum.Flow8ExCheckResource:
                labelText = "资源检查";
                break;
            case FlowEnum.Flow9RepairResource:
                labelText = "资源更新";
                break;
            default:
                labelText = "资源加载中";
                break;
        }
        setFlowLabel(labelText);
    }

    /// <summary>
    /// 下载前的提示，比如弹出下载提示框，如果不需要，直接Resume即可
    /// </summary>
    /// <param name="size"></param>
    private void OnDownloadNotice(int size)
    {
        string msg = string.Format("检测到有{0:F1}MB游戏资源需要更新!", (float)size / 1024 / 1024);//MB

        UIMsgBoxForm.Open(msg, "确定", "退出", () =>
        {
            UpdateManager.Instance.CurrentFlow.Resume(true);
        }, () =>
        {
				UpdateManager.Instance.CloseThread();
				UpdateCenter.Close();
            UnityEngine.Debug.LogError("退出游戏");
            //Application.Quit();
        });
    }

    /// <summary>
    /// 更新结束
    /// </summary>
    /// <param name="result">true:成功；false：失败</param>
    /// <param name="ret">返回码</param>
    private void FinishCallback(bool result, int ret)
    {
        string msg = "整个更新流程结束 " + " result=" + result + " ret = " + ret;
        DebugLog(msg);

        //再调一次收尾，最后显示的进度
        refreshProgress();

        if (result)
        {
            setFlowLabel("开始资源预加载...");
            setProgress(0, 0, false);
            EnterGame();
        }
        else
        {
            msg = string.Format("资源初始化失败({0})", GetErrorMsg(ret));
            if (!LauncherUpdate.IsNetworkEnable())
            {
                msg = "网络已断开，请检查网络后重试!";
            }
            UIMsgBoxForm.Open(msg, "重试", "退出游戏",
                () =>
                {
                    UpdateManager.Instance.Restart();
                },
                () => { Application.Quit(); });
        }
    }


    void Update()
    {
        UpdateManager.Instance.Update();

        UpdateUI(Time.deltaTime);

        //RefreshPreloadProgress();

        if ((Input.GetKeyUp(KeyCode.Escape)))
        {
            UIMsgBoxForm.Open("退出游戏？", "确定", "取消",
                () =>
                {
                    Application.Quit();
                });
        }
    }

    public void UpdateUI(float delta)
    {
        _perSecondTimer += delta;
        if (_perSecondTimer >= 1.0f)
        {
            _perSecondTimer = 0;
        }

        if (UpdateManager.Instance.Running())
        {
            refreshProgress();
        }
    }

    #region //预加载资源时显示进度
    //真实已经加载的
    private int _curLodedCount = 0;
    //真实总量
    private float _total = 0;
    //假进度刷新频率
    private float _deltaTime = 0;
    //每秒10%的进度
    private float _speedPreload = 10;
    private float _progress = 0;
    private void RefreshPreloadProgress()
    {
        if (!UpdateManager.Instance.ShowPreloadPregress()) return;

        if (_total == 0)
        {
            _progress += _speedPreload * Time.deltaTime;
            if (_progress > 60)
            {
                _progress = 60;
            }

            float total = UpdateManager.Instance.GetPreloadTotal();
            _total = total;
        }
        else
        {
            if (_progress < 60)
            {
                _progress = 60;
            }

            int curCount = UpdateManager.Instance.GetPreloadedCount();

            //加上剩余的40%
            _progress += curCount / _total * 40;
        }

        setProgress(_progress / 100, 0, false);
    }

    #endregion


    /// <summary>
    /// 刷新进度
    /// </summary>
    private void refreshProgress()
    {
        string url = "";
        int progressValue = 0;
        int total = 0;
        //当前进度是否下载进度, 如果是下载，则需要显示网速
        bool useDownload = false;
        var curFlow = UpdateManager.Instance.CurrentFlow;
        if (curFlow == null) return;

        UpdateManager.Instance.GetDownloadInfo(out url, out total, out progressValue, out useDownload);

        if (_oldProgressValue == 0)
            _oldProgressValue = progressValue;

        if (_perSecondTimer == 0)
        {
            _speed = _perSecondTimer == 0 ? (progressValue - _oldProgressValue) / 1024 : 0;
            _oldProgressValue = progressValue;

            if (_speed < 0)
                _speed = 0;
        }

        float percetage = total <= 0 ? 1 : progressValue / (float)total;

        setProgress(percetage, _speed, useDownload);
        setProgressMinToMax(progressValue, total, useDownload);
    }

    private string GetErrorMsg(int code)
    {
        string msg = "" + code;
        switch (code)
        {
            case CodeDefine.RET_FAIL_DOWNLOAD_LOCAL_XML:
                msg = "";
                break;
            case CodeDefine.RET_FAIL_DOWNLOAD_MAP_FILE:
                msg = "下载map文件失败";
                break;
            case CodeDefine.RET_FAIL_DOWNLOAD_RES_XML:
                msg = "下载RemoteVersion.xml失败";
                break;
            case CodeDefine.RET_FAIL_EXCEPTION_DOWNLOAD:
                msg = "网络不稳定";
                break;
            case CodeDefine.RET_FAIL_LOCAL_XML_NOT_EXIST:
                msg = "LocalXml不存在";
                break;
            case CodeDefine.RET_FAIL_MAP_MD5_ERROR:
                msg = "map文件md5不正确";
                break;
            case CodeDefine.RET_FAIL_PARSE_LOCAL_XML_FILE:
                msg = "LocalXml解析失败";
                break;
            case CodeDefine.RET_FAIL_PARSE_MAP_FILE:
                msg = "Map文件解析失败";
                break;
            case CodeDefine.RET_FAIL_PARSE_RES_XML_FILE:
                msg = "RemoteXml解析失败";
                break;
            case CodeDefine.RET_FAIL_RES_FILE_MD5_ERROR:
                msg = "RemoteXml文件md5不正确";
                break;
            case CodeDefine.RET_FAIL_RES_FILE_NOT_EXIST:
                msg = "RemoteXml文件不存在";
                break;
            case CodeDefine.RET_FAIL_RES_XML_PATH_ERROR:
                msg = "RemoteXml路径不正确";
                break;
            case CodeDefine.RET_FAIL_SAVE_LOCAL_XML_FILE:
                msg = "LocalXml保存失败";
                break;
            case CodeDefine.RET_FAIL_TRANS_FAIL:
                msg = "资源转移失败";
                break;
            case CodeDefine.RET_FAIL_UNZIP_RES_FILE:
                msg = "释放资源失败";
                break;
            case CodeDefine.RET_SKIP_BY_ABORT:
                msg = "中断操作";
                break;
        }

        return msg;
    }

    /// <summary>
    /// 更新完毕，进入游戏
    /// </summary>
    private void EnterGame()
    { 
        UpdateManager.Instance.SetPreloadTotal(0);

        //TODO::Update 这里手动添加启动游戏场景的逻辑
        //UnityEngine.Debug.LogError("~~~~~~~~~在这里添加启动游戏的逻辑~~~~~~~~");
        GameStart();
    }

    private static void GameStart()
    {
        Launcher.Launcher.Launch();
    }

    /// <summary>
    /// 进度百分比和下载速度
    /// </summary>
    /// <param name="percatage"></param>
    /// <param name="speed"></param>
    /// <param name="useDownload"></param>
    private void setProgress(float percatage, int speed, bool useDownload = false)
     {
        if (_Progress == null)
            return;

        if (percatage > 1) percatage = 1;
        if (percatage < 0) percatage = 0;

        _Progress.fillAmount = (percatage);

        percatage = percatage * 100;
        if (useDownload)
            _PercentageLabel.text = (string.Format("{0:F1}% ({1} kb/s)", percatage, speed));
        else
            _PercentageLabel.text = (string.Format("{0:F1}%", percatage));

    }

    /// <summary>
    /// 进度显示
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="useDownload"></param>
    private void setProgressMinToMax(int min, int max, bool useDownload = false)
    {
        if (max == 0)
            _ProgressMinToMaxLabel.text = "";
        else if (useDownload)
        {
            min = Mathf.Min(min, max);
            _ProgressMinToMaxLabel.text = string.Format("{0:F2}MB/{1:F2}MB", min / (1024f * 1024f), max / (1024f * 1024f));
        }
        else
            _ProgressMinToMaxLabel.text = string.Empty;

    }

    /// <summary>
    /// 设置流程名字
    /// </summary>
    /// <param name="str"></param>
    private void setFlowLabel(string str)
    {
        _StepLabel.text = str;
    }

    /// <summary>
    /// 客户端下载完成
    /// Android：直接安装
    /// IOS：跳转到url
    /// PC：不处理
    /// </summary>
    /// <param name="success"></param>
    private void OnClientDownloadFinish(bool success)
    {
        string msg = "客户端下载完成 " + " success=" + success;
        //if (UpdateManager.Instance.GetPlatformType() == PlatformType.IOS)
        {
            DebugLog("ios平台，跳转到url: " + UpdateManager.Instance.GetClientUrl());
        }
    }
}

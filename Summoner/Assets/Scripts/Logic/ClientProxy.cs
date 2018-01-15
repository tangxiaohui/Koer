using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientProxy : MonoBehaviour
{
    private gxGameState m_gameState = null;
    private static ClientProxy m_Instance = null;
    private GameStateEnum m_LateGameStateEnum = GameStateEnum.GameStateEnum_None;
    private GameStateEnum m_curGameStateEnum = GameStateEnum.GameStateEnum_None;
    private LocalVersion m_localVersion = new LocalVersion();
    private ServerNoticeXml m_ServerNoticeXml = new ServerNoticeXml();
    public static bool isBackLogin = false;
    private static WaitForEndOfFrame defaultWaitForEndOfFrame = new WaitForEndOfFrame();
    public GameStateEnum curGameStateEnum
    {
        get
        {
            return m_curGameStateEnum;
        }
    }
    public static ClientProxy Instance
    {
        get
        {
            return m_Instance;
        }
    }

    public gxGameState curgameState
    {
        get
        {
            return m_gameState;
        }
    }
    #region localVesion
    public string photo_adress
    {
        get
        {
            return m_localVersion.stringPhotoAdress;
        }
    }
    public string ip
    {
        get
        {
            return m_localVersion.ip;
        }
    }

    public string iport
    {
        get
        {
            return m_localVersion.iport;
        }
    }

    public string version
    {
        get
        {
            return m_localVersion.version;
        }
    }

    public string ChargeAddress
    {
        get {
            
            return m_localVersion.ChargeAddress;
        }
    }
    #endregion

    #region ServerNoticeXml
    public string notice_title
    {
        get
        {
            return m_ServerNoticeXml.notice_title;
        }
    }

    public string notice_content
    {
        get
        {
            return m_ServerNoticeXml.notice_content;
        }
    }
    public string account_notice_title
    {
        get
        {
            return m_ServerNoticeXml.account_notice_title;
        }
    }

    public string account_notice_content
    {
        get
        {
            return m_ServerNoticeXml.account_notice_content;
        }
    }

    #endregion


    private void Awake()
    {
        transform.SetParent(Common.Root.root);


        Application.backgroundLoadingPriority = UnityEngine.ThreadPriority.High;
        Application.runInBackground = true;
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        m_Instance = this;



        Utility_Profiler.BeginSample("ClientProxy.Initalize()");
        Initalize();
        Utility_Profiler.EndSample();


        EnterGame();
        Common.Root.coro.StartCoroutine(StartClientProxy());
    }

    IEnumerator StartClientProxy()
    {
        Debug.Log("StartClientProxy is Start");
        yield return defaultWaitForEndOfFrame;
        Utility_Profiler.BeginSample("TextManager.Instance.Initalize()");
        TextManager.Instance.Initalize();
        Utility_Profiler.EndSample();
        Utility_Profiler.BeginSample(" SinglePanelManger.Instance.Initalize()");
        SinglePanelManger.Instance.Initalize();
        Utility_Profiler.EndSample();
        Debug.Log("StartClientProxy is Finish");
    }

    //初始化
    private void EnterGame()
    { 
        ChangeState(GameStateEnum.GameStateEnum_Login);
    }

    private void Initalize()
    {
#if UNITY_STANDALONE_WIN
        Screen.SetResolution(750,1334 , false);
#endif
        m_localVersion.Initalize();
        m_ServerNoticeXml.Initalize();
    //    Pet.Load();
    }


    public void Update()
    {
        if(m_gameState != null)
        {
            m_gameState.Update(Time.deltaTime);
        }
        UpdateQuit();
    }

    protected void UpdateQuit()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
           
        }
    }

    protected void GameQuit()
    {
        Application.Quit();
    }
    private void OnApplicationQuit()
    {
        UIManager.Instance.Deinitialization();
    }

    private void LateUpdate()
    {
        Utility_Profiler.BeginSample("ClientProxy LateUpdate Start");
        if (m_gameState != null)
        {
            m_gameState.LateUpdate(Time.deltaTime);
        }

        if (m_LateGameStateEnum != GameStateEnum.GameStateEnum_None)
        {
            LateChangeState(m_LateGameStateEnum);
            m_LateGameStateEnum = GameStateEnum.GameStateEnum_None;
        }
        Utility_Profiler.EndSample();
    }

    void OnApplicationPause(bool paused)
    {
    
    }

    //等这一帧运行完，在进行调用
    protected void LateChangeState(GameStateEnum eGameStateEnum)
    {
        if(m_curGameStateEnum == eGameStateEnum)
        {
            return;
        }
        if (m_gameState != null)
        {
            m_gameState.Exit();
            m_gameState.Deinitialization();
            m_gameState = null;
        }

        switch (eGameStateEnum)
        {
            case GameStateEnum.GameStateEnum_Login:
            case GameStateEnum.GameStateEnum_Resert:
                m_gameState = new GameLoginState();
                break;
        }
        m_gameState.Initalize();
        if(eGameStateEnum == GameStateEnum.GameStateEnum_Resert)
        {
            eGameStateEnum = GameStateEnum.GameStateEnum_Login;
        }
        m_curGameStateEnum = eGameStateEnum;
    }
    

    public void Resume()
    {
        
    }
    public void ChangeState(GameStateEnum eGameStateEnum)
    {
        m_LateGameStateEnum = eGameStateEnum;
    }

    public void ChangeState(int eGameStateEnum)
    {
        m_LateGameStateEnum = (GameStateEnum)eGameStateEnum;
    }
}

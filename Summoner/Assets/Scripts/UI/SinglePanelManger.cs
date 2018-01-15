using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 全局面板管理
/// </summary>

public class SinglePanelManger : SingleInstance<SinglePanelManger>
{

    Dictionary<Type, UIBase> m_uiDic = new Dictionary<Type, UIBase>();
    public override void Initalize()
    {
        base.Initalize();
        AddSingPanel();
        InitalizeGame();
    }

    public void InitalizeGame()
    {
        AddGamePanel();
    }
    
    public void DeinitializationGame()
    {
        List<Type> list = new List<Type>();
        foreach(var item in m_uiDic)
        {
            if(item.Value.isGame == true)
            {
                list.Add(item.Key);
            }
        }

        for(int i = 0; i < list.Count; ++i)
        {
            m_uiDic[list[i]].Deinitialization();
            m_uiDic.Remove(list[i]);
        }
    }

    public void AddSingPanel()
    {
        #region 从Login开始就初始化的UI
        //Add<GameLoadingPanel>(typeof(GameLoadingPanel), "LoadingUI", EUICanvas.EUICanvas_Top,false);
        Add<WattingUI>(typeof(WattingUI), "WattingUI", EUICanvas.EUICanvas_Top);
        Add<TipsUI>(typeof(TipsUI), "TipsUI", EUICanvas.EUICanvas_Top);
        Add<ServerNoticeUI>(typeof(ServerNoticeUI), "ServerNoticeUI", EUICanvas.EUICanvas_Normal, true, true); 
        Add<MessageBoxPanelUI>(typeof(MessageBoxPanelUI), "MessageBoxPanelUI", EUICanvas.EUICanvas_Top,true,true);
        Add<AccountUI>(typeof(AccountUI), "LoginUI/AccountUI", EUICanvas.EUICanvas_Normal, true, true);
        //Add<BlackUI>(typeof(BlackUI), "BlackUI", EUICanvas.EUICanvas_Top);
        //Add<ShieldUI>(typeof(ShieldUI), "ShieldUI", EUICanvas.EUICanvas_Top);
        #endregion
        if (DevelopSetting.ShowFPS)
        {
            Add<ShowFPS>(typeof(ShowFPS), "ShowFPS", EUICanvas.EUICanvas_Top, false);
        }

        if(DevelopSetting.isDevelop)
        {
            //Add<GMUI>(typeof(GMUI), "GMUI", EUICanvas.EUICanvas_Top, true,true);
        }
        UIManager.Instance.OpenUI(serverNoticeUI);
    }

    public void OpenGMUI()
    {
        //Get<GMUI>().OpenUI();
    }

    public void AddGamePanel()
    {
        //#region WPTODO: 从Game开始就初始化的UI，为什么扔这里，只是暂时性，后面绝对会改 
        //UIManager.Instance.RegisterLuaObject("UI/HomeUI/HomeUI", (int)EUICanvas.EUICanvas_Normal, "HomeUI/HomeUI", true, (int)EUIType.EUIType_Home, false, true);
        AddGame<HomeUI>(typeof(HomeUI), "HomeUI/HomeUI", EUICanvas.EUICanvas_Normal);
        AddGame<BattleUI>(typeof(BattleUI), "BattleUI/BattleUI", EUICanvas.EUICanvas_Normal);
        AddGame<WallpaperUI>(typeof(WallpaperUI), "HomeUI/WallpaperUI", EUICanvas.EUICanvas_Normal);
		AddGame<RegistUI> (typeof(RegistUI), "LoginUI/RegistUI", EUICanvas.EUICanvas_Normal);
        //AddGame<GuideUI>(typeof(GuideUI), "GuideUI", EUICanvas.EUICanvas_Top);

        //#endregion
    }


    public T Get<T>(int index = 0) where T : UIBase
    {
        UIBase s = null;
        if (m_uiDic.TryGetValue(typeof(T), out s))
        {
            return s as T;
        }
        return null;
    }

    public bool Remove(Type type)
    {
        UIBase s = null;
        if (!m_uiDic.TryGetValue(type, out s))
        {
            return false;
        }
        UIManager.Instance.CloseUI(s);
        s.Deinitialization();
        m_uiDic.Remove(type);
        return true;
    }

    public void ClearAllUI()
    {
        foreach(var item in m_uiDic.Values)
        {
            item.Deinitialization();
        }
        m_uiDic.Clear();
    }

    public bool AddGame<T>(Type type, string uiPath, EUICanvas eCanvas, bool isAutoReset = true, bool isUseBack = false) where T : UIBase
    {
        if (m_uiDic.ContainsKey(type))
        {
            return false;
        }
        GameObject obj = Res.ResourcesManager.Instance.SyncGetResource<GameObject>(uiPath, Res.ResourceType.UI);
        T com = obj.AddComponent<T>();
        UIManager.Instance.SetCanvas(com, eCanvas, isAutoReset);
        string name = obj.name.Replace("(Clone)", string.Empty);
        com.name = name;
        com.useBack = isUseBack;
        com.isGame = true;
        com.eUICanvas = eCanvas;
        com.Initalize();
        com.gameObject.SetActive(false);
        m_uiDic[type] = com;
        return true;
    }

    public bool Add<T>(Type type, string uiPath, EUICanvas eCanvas, bool isAutoReset = true,bool isUseBack = false) where T: UIBase
    {
        if (m_uiDic.ContainsKey(type))
        {
            return false;
        }
        GameObject obj = Res.ResourcesManager.Instance.SyncGetResource<GameObject>(uiPath, Res.ResourceType.UI);
        T com =  obj.AddComponent<T>();
        UIManager.Instance.SetCanvas(com, eCanvas, isAutoReset);
        string name = obj.name.Replace("(Clone)", string.Empty);
        com.name = name;
        com.useBack = isUseBack;
        com.isGame = false;
        com.eUICanvas = eCanvas;
        com.Initalize();
        m_uiDic[type] = com;
        return true;
    }


    #region MessageBox
    public MessageBoxPanelUI messageBoxPanelui
    {
        get
        {
            return Get<MessageBoxPanelUI>();
        }
    }

    public void ShowMessage(string title, string content, string okText = "", string cancelText = "")
    {
        if(messageBoxPanelui != null)
        {
            if(!messageBoxPanelui.activeSelf)
            {
                UIManager.Instance.OpenUI(messageBoxPanelui);
            }
        }
        if(messageBoxPanelui != null)
        {
            messageBoxPanelui.Show(title, content, okText, cancelText);
        }
    }

    public void ShowSoloMessage(string title, string content,string okText = "")
    {
        if (messageBoxPanelui != null)
        {
            if (!messageBoxPanelui.activeSelf)
            {
                UIManager.Instance.OpenUI(messageBoxPanelui);
            }
        }
        if (messageBoxPanelui != null)
        {
            messageBoxPanelui.ShowSolo(title, content, okText);
        }
    }

    public void ShowSoloMessage(string title, string content)
    {
        if (messageBoxPanelui != null)
        {
            if (!messageBoxPanelui.activeSelf)
            {
                UIManager.Instance.OpenUI(messageBoxPanelui);
            }
        }
        if (messageBoxPanelui != null)
        {
            messageBoxPanelui.ShowSolo(title, content, "");
        }
    }

    public void ShowSoloMessage(string title, string content, Action okFun = null, string okText = "")
    {
        if (messageBoxPanelui != null)
        {
            if (!messageBoxPanelui.activeSelf)
            {
                UIManager.Instance.OpenUI(messageBoxPanelui);
            }
        }
        if (messageBoxPanelui != null)
        {
            messageBoxPanelui.ShowSolo(title, content, okFun, okText);
            messageBoxPanelui.Top();
        }
    }


    public void ShowMessage(string title, string content, System.Action okFun = null, System.Action cancel = null, string okText = "", string cancelText = "")
    {
        if (messageBoxPanelui != null)
        {
            if (!messageBoxPanelui.activeSelf)
            {
                UIManager.Instance.OpenUI(messageBoxPanelui);
                messageBoxPanelui.Top();
            }
        }

        if (messageBoxPanelui != null)
        {
            messageBoxPanelui.Show(title, content, okFun, cancel, okText, cancelText);
        }
    }
    #endregion
    #region WattingUI
    public WattingUI wattingUI
    {
        get
        {
            return Get<WattingUI>();
        }
    }


    public void OpenWattingUIAction(Action callBack = null)
    {
        wattingUI.SetAutoFailClose(callBack);
        UIManager.Instance.OpenUI(wattingUI);
    }

    public void OpenWattingUI()
    {
        wattingUI.SetAutoFailClose(null);
        UIManager.Instance.OpenUI(wattingUI);
    }

    public void CloseWattingUI()
    {
        UIManager.Instance.CloseUI(wattingUI);
    }
    #endregion
    #region tipsUI
    public TipsUI tipsUI
    {
        get
        {
            return Get<TipsUI>();
        }
    }

    public void PushTips(string str)
    {
        tipsUI.PushStr(str);
    }
    #endregion
    #region ServerUI
    public ServerNoticeUI serverNoticeUI
    {
        get
        {
            return Get<ServerNoticeUI>();
        }
    }

    public HomeUI homeUI
    {
        get
        {
            return Get<HomeUI>();
        }
    }

    public BattleUI battleUI
    {
        get
        {
            return Get<BattleUI>();
        }
    }

    public WallpaperUI wallpaperUI
    {
        get
        {
            return Get<WallpaperUI>();
        }
    }

	public RegistUI registUI
	{
		get
		{ 
			return Get<RegistUI> ();
		}
	}

	public void ShowRegistUI()
	{
		UIManager.Instance.OpenUI (registUI);
	}

    public void ShowWallpaperUI()
    {
        UIManager.Instance.OpenUI(wallpaperUI);
    }

    public void ShowServerNoticeUI()
    {
        UIManager.Instance.OpenUI(serverNoticeUI);
        //serverNoticeUI.Top();
    }

    public void ShowHomeUI()
    {
        UIManager.Instance.OpenUI(homeUI);
    }

    public void ShowBattleUI()
    {
        UIManager.Instance.OpenUI(battleUI);
    }
    #endregion

    public override void Deinitialization()
    {
        //m_uiDic.Clear();
        base.Deinitialization();
    }
}

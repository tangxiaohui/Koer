using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UIManagerCanvas
{
    private GameObject gameObject;
    public Transform transform;
    private Transform m_CameraUI;
    private Camera m_CameraUICom;
    //private Transform m_Camera3D;
    private EUICanvas m_eCanvasType = EUICanvas.EUICanvas_Normal;
    private Transform m_CanvasTransform;
    protected Canvas m_Canvas;
    protected CanvasScaler m_CanvasScaler;

    public Canvas canvas
    {
        get
        {
            return m_Canvas;
        }
    }

    public Camera MainCamera
    {
        get {
            return m_CameraUICom;
        }
    }

    public CanvasScaler canvasScaler
    {
        get
        {
            return m_CanvasScaler;
        }
    }

    public void Initalize(GameObject gameObject,EUICanvas canvasType)
    {
        this.gameObject = gameObject;
        this.gameObject.SetActive(true);
        this.transform = gameObject.transform;
        m_eCanvasType = canvasType;
        m_CameraUI = Utility.GameUtility.FindDeepChild(gameObject, "Camera/Camera_UI");
        m_CameraUICom = m_CameraUI.GetComponent<Camera>();
        switch(m_eCanvasType)
        {
            case EUICanvas.EUICanvas_Move:
                m_CameraUICom.depth = DepthUtils.UIMove;
                break;
            case EUICanvas.EUICanvas_Normal:
                m_CameraUICom.depth = DepthUtils.UI;
                break;
            case EUICanvas.EUICanvas_Top:
                m_CameraUICom.depth = DepthUtils.UITop;
                break;
            case EUICanvas.EUICanvas_Bottom:
                m_CameraUICom.depth = DepthUtils.UIBottom;
                break;
            case EUICanvas.EUICanvas_WordMap:
                m_CameraUICom.depth = DepthUtils.UIWorldMap;
                break;
            default:
                m_CameraUICom.depth = DepthUtils.UI;
                break;
        }
        m_CameraUICom.nearClipPlane = -100;
        // m_Camera3D = Utility.GameUtility.FindDeepChild(gameObject, "Camera/Camera_3D");
        m_CanvasTransform = Utility.GameUtility.FindDeepChild(gameObject, "CanvasChain");
        m_Canvas = m_CanvasTransform.gameObject.GetComponent<Canvas>();
        m_CanvasScaler = m_CanvasTransform.gameObject.GetComponent<CanvasScaler>();
        SetCulliingMask();
        SetLayer((int)m_CameraUICom.depth);
        int layer = 0;
        switch (m_eCanvasType)
        {
            case EUICanvas.EUICanvas_Move:
                layer = LayerUtils.UIMove;
                break;
            case EUICanvas.EUICanvas_Normal:
                layer = LayerUtils.UI;
                break;
            case EUICanvas.EUICanvas_Top:
                layer = LayerUtils.UITop;
                break;
            case EUICanvas.EUICanvas_Bottom:
                layer = LayerUtils.UIBottom;
                break;
            case EUICanvas.EUICanvas_WordMap:
                layer = LayerUtils.UIWorldMap;
                break;
        }
        Utility.GameUtility.ChangeChildLayer(transform, layer);
    }


    public void SetLayer(int index)
    {
        m_Canvas.sortingOrder = index;
    }

    public void SetCulliingMask()
    {
        int cullingMax = 0;
        switch(m_eCanvasType)
        {
            case EUICanvas.EUICanvas_Move:
                cullingMax = 1 << LayerUtils.UIMove;
                break;
            case EUICanvas.EUICanvas_Top:
                cullingMax = 1 << LayerUtils.UITop;
                break;
            case EUICanvas.EUICanvas_Normal:
                cullingMax = 1 << LayerUtils.UI;//| 1 << LayerUtils.UIModel;
                break;
            case EUICanvas.EUICanvas_Bottom:
                cullingMax = 1 << LayerUtils.UIBottom;
                break;
            case EUICanvas.EUICanvas_WordMap:
                cullingMax = 1 << LayerUtils.UIWorldMap;
                break;
        }
        m_CameraUICom.cullingMask = cullingMax;
    }


    public void Deinitialization()
    {

    }

    public void SetParent(Transform t)
    {
        this.transform.SetParent(t);
        Reset(t);
    }

    public void Reset(Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }

    public void SetUI(Transform t, bool isAutoReset)
    {
        t.SetParent(m_CanvasTransform);
        if (isAutoReset)
            Reset(t);  
    }

    public Vector3 WorldToScreenPoint(Vector3 ve3)
    {
        ve3 = m_CameraUICom.WorldToScreenPoint(ve3);
        return ve3;
    }

    public Vector3 ScreenToWorldPoint(Vector3 ve3)
    {
        ve3 = m_CameraUICom.ScreenToWorldPoint(ve3);
        return ve3;
    }
}

public class UIPreLoad
{
    public string uiPath;
    public string luaPath;
    public EUIType eType;
    public bool useBack;
    public EUICanvas canvas;
    public bool isAutoReset;
    public bool preLoad;
    public object obj = null;
  //  DealLuaObject(string uiPath, string luaPath, EUIType eType, bool useBack, EUICanvas canvas, bool isAutoReset, bool preLoad = false)
}

public class UIManager : SingleInstance<UIManager>
{
    private Dictionary<EUICanvas, UIManagerCanvas> m_canvasDic = new Dictionary<EUICanvas, UIManagerCanvas>();
    private Dictionary<string, UIBase> m_uiDic = new Dictionary<string, UIBase>();
    Dictionary<EUIType, bool> _haveFullUI = new Dictionary<EUIType, bool>();
    private GameObject gameObject = null;
    private Transform tranform = null;
    private UnityEngine.EventSystems.EventSystem eventSystem = null;
    protected readonly float m_ScreenScale = 0.67f;
   // private Camera BattleCamera;
    //UGUI 根节点  
    public const string GameUIRootName = "Default/UI/UGUIRoot";

    public float XScale;
    public float YScale;
    public readonly Vector2 BASE_RESOLUTION = new Vector2(750, 1334);

    protected List<UIPreLoad> m_preUIList = new List<UIPreLoad>();
    protected List<UIPreLoad> m_preCompeleteUIList = new List<UIPreLoad>();

    public static void ResetNode(Transform tran)
    {
        tran.localPosition = Vector3.zero;
        tran.localScale = Vector3.one;
    }

    public override void Initalize()
    {
        base.Initalize();
        if(gameObject == null)
        {
            gameObject = ResourceLoad.Load(GameUIRootName);
            gameObject.name = "UGUIRoot";
            tranform = gameObject.transform;
            eventSystem = Utility.GameUtility.FindDeepChild<EventSystem>(gameObject, "EventSystem");
            eventSystem.enabled = true;
            ResetNode(tranform);
            InitalizeCanvas();
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }

    protected void InitalizeCanvas()
    {
        GameObject canvaspre = Utility.GameUtility.FindDeepChild(gameObject, "CanvasChainTemplate").gameObject;
        canvaspre.SetActive(false);
        SetCanvas(canvaspre, EUICanvas.EUICanvas_Normal);
        SetCanvas(canvaspre, EUICanvas.EUICanvas_Top);
        SetCanvas(canvaspre, EUICanvas.EUICanvas_Bottom);
        SetCanvas(canvaspre, EUICanvas.EUICanvas_Move);
        SetCanvas(canvaspre, EUICanvas.EUICanvas_WordMap);
    }

    protected  void SetCanvas(GameObject canvaspre, EUICanvas eCanvas)
    {
        if(!m_canvasDic.ContainsKey(eCanvas))
        {
            GameObject go = GameObject.Instantiate(canvaspre);
            go.name = eCanvas.ToString();
            go.transform.SetParent(tranform);
            ResetNode(go.transform);
            UIManagerCanvas data = new UIManagerCanvas();
            data.Initalize(go,eCanvas);
            data.SetParent(tranform);
            m_canvasDic[eCanvas] = data;
        }
    }

    private void ChangeBattleView(bool isShow)
    {
        if (m_canvasDic[EUICanvas.EUICanvas_Move].transform.gameObject.activeSelf == isShow)
            return;
        m_canvasDic[EUICanvas.EUICanvas_Move].transform.gameObject.SetActive(isShow);
    }

    public void RegisterLuaObject(string luaPath, int index, string uiPath, bool isAutoReset, int eType, bool useBack,bool preLoad)
    {
        DealLuaObject(uiPath, luaPath, (EUIType)eType, useBack, (EUICanvas)index, isAutoReset,preLoad);
    }

    public void RegisterLuaObject(string luaPath, int index, string uiPath)
    {
       DealLuaObject(uiPath, luaPath, EUIType.EUIType_None, false, (EUICanvas)index, true);
    }

    public void RegisterLuaObject(string luaPath, int index, string uiPath, bool isAutoReset , EUIType eType)
    {
        DealLuaObject(uiPath, luaPath, eType, false, (EUICanvas)index, isAutoReset);
    }

    public void RegisterLuaObject(string luaPath, int index, string uiPath, bool isAutoReset, EUIType eType,bool useBack)
    {
        DealLuaObject(uiPath, luaPath, eType, useBack, (EUICanvas)index, isAutoReset);
    }


    public void RegisterLuaObject(string luaPath, int index, string uiPath, bool isAutoReset, int eType)
    {
        DealLuaObject(uiPath, luaPath, (EUIType)eType, false, (EUICanvas)index, isAutoReset);
    }

    public void RegisterLuaObject(string luaPath, int index, string uiPath, bool isAutoReset, int eType, bool useBack)
    {
       DealLuaObject(uiPath,luaPath,(EUIType)eType,useBack,(EUICanvas)index, isAutoReset);
    }

    public void RegisterLuaObject(string luaPath,int index,string uiPath,bool isAutoReset)
    {
        DealLuaObject(uiPath, luaPath, EUIType.EUIType_None, false, (EUICanvas)index, isAutoReset);
    }

    public void SetCanvas(UIBase ui, EUICanvas eType,bool isAutoReset = true)
    {
        UIManagerCanvas canvas = null;
        if (m_canvasDic.TryGetValue(eType, out canvas))
        {
            canvas.SetUI(ui.transform, isAutoReset);
        }
    }

    public UIManagerCanvas GetCanvas(int type)
    {
        return GetCanvas((EUICanvas)type);
    }

    public Transform GetCanvasTransform(int type)
    {
        return GetCanvas((EUICanvas)type).canvas.transform;
    }

    public Transform GetCanvasTransform(EUICanvas type)
    {
        return GetCanvas(type).canvas.transform;
    }


    public UIManagerCanvas GetCanvas(EUICanvas eType)
    {
        if (m_canvasDic.ContainsKey(eType))
        {
            return m_canvasDic[eType];
        }
        else
        {
            return null;
        }
    }

    protected void OnUICompelete(string str,object obj)
    {
        int i = m_preUIList.Count - 1;
        for (; i > -1; --i)
        {
            if(m_preUIList[i].uiPath.Equals(str))
            {
                m_preUIList[i].obj = obj;
                m_preCompeleteUIList.Add(m_preUIList[i]);
                m_preUIList.RemoveAt(i);
                return;
            }
        }
    }

    protected bool isPreLoad(string str)
    {
        int i = m_preUIList.Count - 1;
        for (; i > -1; --i)
        {
            if (m_preUIList[i].uiPath.Equals(str))
            {
                return true;
            }
        }
        return false;
    }

    public bool isPreLoadCompelete()
    {
        return m_preUIList.Count < 1;
    }

    public void InitalizePreLoadUI()
    {
        for(int i = 0; i < m_preCompeleteUIList.Count; ++i)
        {
            UIPreLoad data = m_preCompeleteUIList[i];
            DealLuaObject(data.uiPath, data.luaPath, data.eType, data.useBack, data.canvas, data.isAutoReset, false,data.obj);
        }
        m_preCompeleteUIList.Clear();
    }
    protected void DealLuaObject(string uiPath, string luaPath, EUIType eType, bool useBack, EUICanvas canvas, bool isAutoReset, bool preLoad = false, object tempObj = null)
    {
        string luaFileName = System.IO.Path.GetFileName(uiPath);
        if (m_uiDic.ContainsKey(luaFileName))
        {
            return;
        }

        if (preLoad)
        {
            if (!isPreLoad(uiPath))
            {
                Res.ResourcesManager.Instance.AsyncLoadResource<GameObject>(uiPath, Res.ResourceType.UI, OnUICompelete);
                UIPreLoad UIPreLoadData = new UIPreLoad();
                UIPreLoadData.uiPath = uiPath;
                UIPreLoadData.luaPath = luaPath;
                UIPreLoadData.eType = eType;
                UIPreLoadData.useBack = useBack;
                UIPreLoadData.canvas = canvas;
                UIPreLoadData.isAutoReset = isAutoReset;
                UIPreLoadData.preLoad = preLoad;
                m_preUIList.Add(UIPreLoadData);
            }
            return;
        }

        GameObject obj = null;
        if (tempObj != null)
        {
            obj = tempObj as GameObject;
        }
        else
        {
            obj = Res.ResourcesManager.Instance.SyncGetResource<GameObject>(uiPath, Res.ResourceType.UI);
        }
        string name = obj.name.Replace("(Clone)", string.Empty);
        obj.name = name;
        UIBase luaBase = obj.AddComponent<UIBase>();
        luaBase.eUIType = eType;
        luaBase.eUICanvas = canvas;
        luaBase.useBack = useBack;
        m_uiDic[luaFileName] = luaBase;
        RegisterUI(luaBase, canvas, isAutoReset);
    }

    public bool isInitalize(string uiPath)
    {
        string luaFileName = System.IO.Path.GetFileName(uiPath);
        return m_uiDic.ContainsKey(luaFileName);
    }

    protected void RegisterUI(UIBase luaBase, EUICanvas eType, bool isAutoReset)
    {
        UIManagerCanvas canvas = null;
        if(m_canvasDic.TryGetValue(eType,out canvas))
        {
            canvas.SetUI(luaBase.transform, isAutoReset);
            if(luaBase != null)
            {
                if (luaBase.useBack)
                {
                    luaBase.SetOverrideSorting(true);
                }
                luaBase.order = canvas.canvas.sortingOrder;
            }
        }
    }

    void UpdateFullDic(UIBase ui)
    {
        if ((_haveFullUI.ContainsKey(ui.eUIType) && _haveFullUI[ui.eUIType]) == false)
            return;
        if (ui.useBack == false && ui.eUIType < EUIType.EUIType_NoneBase)
        {
            bool found = false;
            foreach (var item in m_uiDic.Values)
            {
                if (item.eUIType == ui.eUIType && ui.useBack == false && item.GetInstanceID() != ui.GetInstanceID())
                {
                    found = true;
                }
            }
            if (found == false)
            {
                _haveFullUI[ui.eUIType] = false;
            }
        }
    }

    public void CloseUI(string luaPath)
    {
        UIBase ui = null;
        if (m_uiDic.TryGetValue(luaPath,out ui))
        {
            CloseUI(ui);
        }
    }


    public UIBase GetUI(string uiname)
    {
        UIBase ui = null;
        m_uiDic.TryGetValue(uiname, out ui);
        return ui;
    }

    public int GetTopBarSibling()
    {
        if (m_uiDic.ContainsKey("TopMenuBarUI"))
        {
            return m_uiDic["TopMenuBarUI"].transform.GetSiblingIndex();
        }
        else
        {
            return 0;
        }
    }

    public void CloseUI(UIBase ui)
    {
        if(ui != null)
        {
            if (ui.activeSelf)
                ui.CloseUI();
            UpdateFullDic(ui);
        }
    }

    public void CloseUIByType(int eType)
    {
        foreach (var item in m_uiDic.Values)
        {
            if (item.eUIType == (EUIType)eType)
            {
                item.CloseUI();
            }
        }
        _haveFullUI[(EUIType)eType] = false;
    }

    public void CloseUseBackPanel()
    {
        foreach(var item in m_uiDic.Values)
        {
            if(item.useBack)
            {
                item.CloseUI();
            }
        }
    }

    public void OpenUI(UIBase ui)
    {
        int DepthUtilsStart = DepthUtils.UI + 2;
        if (ui != null)
        {
            foreach (var item in m_uiDic.Values)
            {
                if(item.eUIType > EUIType.EUIType_None)
                {
                    if(item.eUIType == ui.eUIType)
                    {
                        item.CloseUI();
                    }
                }

                if(item.activeSelf && item.useBack)
                {
                    item.SetOrderLayer(DepthUtilsStart);
                    DepthUtilsStart += 2;
                }
            }

            if (ui.useBack)
            {
                ui.SetOrderLayer(DepthUtilsStart);
            }

            if (!ui.activeSelf)
            {
                ui.OpenUI();
            }
        }
    }

    public void OpenUI(string luaPath)
    {
        UIBase ui = null;

        if (m_uiDic.TryGetValue(luaPath, out ui))
        {
            EUIType eType = ui.eUIType + 9;
            if (ui.eUIType != EUIType.EUIType_None)
            {
                foreach (var item in m_uiDic.Values)
                {
                    if (item.eUIType > EUIType.EUIType_None)
                    {
                        if (item.eUIType != eType)///不是base UI
                        {
                            if (item.eUIType != ui.eUIType || (item.useBack == false && ui.useBack == false && item.eUIType != EUIType.EUIType_LoginBase))
                            {
                                item.CloseUI();
                                if (item.useBack == false)///关闭了其他类型的不带黑底的UI
                                {
                                    EUIType itemBase = item.eUIType + 9;
                                    if (itemBase != eType && _haveFullUI.ContainsKey(item.eUIType) && _haveFullUI[item.eUIType])
                                    {
                                        _haveFullUI[item.eUIType] = false;
                                    }
                                }
                            }
                        }
                        else///是base UI
                        {
                            if (ui.useBack == false)///没用黑底的就关闭base UI
                            {
                                item.CloseUI();
                            }
                        }
                    }
                }
            }

            if (ui.eUIType != EUIType.EUIType_None && ui.eUIType < EUIType.EUIType_FightBase)
            {

                foreach (var item in m_uiDic.Values)
                {
                    if (item.eUIType > EUIType.EUIType_None)
                    {
                        if (!item.activeSelf && item.eUIType == eType && ui.useBack)
                        {
                            if(_haveFullUI.ContainsKey(ui.eUIType) == false || _haveFullUI[ui.eUIType] == false)
                            {
                                item.OpenUI();
                                break;
                            }
                        }
                    }
                }
            }
            if (ui.eUIType != EUIType.EUIType_LoginBase && eType != EUIType.EUIType_LoginBase && ui.eUIType != EUIType.EUIType_None && eType != EUIType.EUIType_None)
            {
                if (ui.eUIType == EUIType.EUIType_FightBase || (eType == EUIType.EUIType_FightBase) && ui.useBack)
                {
                    ChangeBattleView(true);
                }
                else
                {
                    ChangeBattleView(false);
                } 
            }
            if (ui.useBack == false && ui.eUIType < EUIType.EUIType_NoneBase)
            {
                _haveFullUI[ui.eUIType] = true;
            }

            int DepthUtilsStart = DepthUtils.UI + 2;
            foreach(var item in m_uiDic.Values)
            {
                if(item != ui && item.useBack && ui.eUICanvas == EUICanvas.EUICanvas_Normal)
                {
                    item.SetOrderLayer(DepthUtilsStart);
                    DepthUtilsStart += 2;
                }
            }

            if (ui.useBack)
            {
                if (ui.eUICanvas == EUICanvas.EUICanvas_Normal)
                {
                    ui.SetOrderLayer(DepthUtilsStart);
                }
                else
                {
                    ui.Top();
                }
            }
            ui.OpenUI();

            Utility.Event.EventDispatcher.Dispatch(Utility.Event.EventType.UIEnumEvent, (int)eType);
        }
    }

    public bool isOpenUI(string uiname)
    {
        if (string.IsNullOrEmpty(uiname))
        {
            return false;
        }
        UIBase ui = null;
        if (m_uiDic.TryGetValue(uiname,out ui))
        {
            return ui.activeSelf;
        }
        return false;
    }

    public  void ClearAllUI()
    {
        foreach (var item in m_uiDic.Values)
        {
            item.Deinitialization();
        }
        _haveFullUI.Clear();
        m_uiDic.Clear();
    }

    public override void Deinitialization()
    {
        foreach(var item in m_uiDic.Values)
        {
            item.Deinitialization();
        }
        m_uiDic.Clear();

        foreach (var item in m_canvasDic.Values)
        {
            item.Deinitialization();
        }
        m_canvasDic.Clear();
        base.Deinitialization();
    }

    public Vector3 WorldToScreenPoint(int index, Vector3 vec)
    {
        EUICanvas eCanvas = (EUICanvas)index;
        UIManagerCanvas canvas = null;
        if (m_canvasDic.TryGetValue(eCanvas, out canvas))
        {
            return canvas.WorldToScreenPoint(vec);
        }
        return vec;
    }

    public Vector3 WorldToScreenPoint(EUICanvas eCanvas , Vector3 vec)
    {
        UIManagerCanvas canvas = null;
        if (m_canvasDic.TryGetValue(eCanvas, out canvas))
        {
            return canvas.WorldToScreenPoint(vec);
        }
        return vec;
    }

    public Vector3 ScreenToWorldPoint(int index, Vector3 vec)
    {
        EUICanvas eCanvas = (EUICanvas)index;
        UIManagerCanvas canvas = null;
        if (m_canvasDic.TryGetValue(eCanvas, out canvas))
        {
            return canvas.ScreenToWorldPoint(vec);
        }
        return vec;
    }

    public Vector3 ScreenToWorldPoint(EUICanvas eCanvas, Vector3 vec)
    {
        UIManagerCanvas canvas = null;
        if (m_canvasDic.TryGetValue(eCanvas, out canvas))
        {
            return canvas.ScreenToWorldPoint(vec);
        }
        return vec;
    }

    //比UI宽度多的就出现黑边
    //比UI宽度少的就适配 by 王鹏
    public void AdaptationUI(UIBase ui)
    {
        if (ui != null)
        {
            RectTransform rect = ui.gameObject.GetComponent<RectTransform>();
            AdaptationUI(rect, ui.eUICanvas);
        }
    }

    protected void AdaptationUI(RectTransform rect, EUICanvas eEUICanvas)
    {
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.transform.localScale = Vector3.one;
            UIManagerCanvas canvas = GetCanvas(eEUICanvas);
            //真实宽度
            float RealScreenScale = Screen.width / (float)Screen.height;
            float ReadWidth = canvas.canvasScaler.referenceResolution.y * RealScreenScale;

            //实际宽度
            float width = canvas.canvasScaler.referenceResolution.y * m_ScreenScale;
            float height = canvas.canvasScaler.referenceResolution.y;
            float offsetX = 0;
            if (width > ReadWidth)
            {
                width = ReadWidth;
            }
            else
            {
                offsetX = (ReadWidth - width) * 0.5f;
            }
            rect.offsetMax = new Vector2(-offsetX, 0);
            rect.offsetMin = new Vector2(offsetX, 0);
            //rect.sizeDelta = new Vector2(width, 0);
        }
    }

    public void AdaptationUI(GameObject go, EUICanvas eEUICanvas)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        AdaptationUI(rect, eEUICanvas);
    }
}

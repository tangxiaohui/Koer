using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class UIBase : Base
{

    protected List<GameObject> m_bgList = new List<GameObject>();
    protected Dictionary<string, GameObject> m_ParticalDic = new Dictionary<string, GameObject>();
    protected Canvas m_Canavas = null;
    protected GraphicRaycaster m_GraphicRaycaster = null;
    public EUIType eUIType = EUIType.EUIType_None;
    public EUICanvas eUICanvas = EUICanvas.EUICanvas_Normal;
    protected int m_order = 0;
    public int order
    {
        get
        {
            return m_order;
        }
        set
        {
            m_order = value;
        }
    }
    protected bool m_useBack = false;
    public bool useBack
    {
        get
        {
            return m_useBack;
        }
        set
        {
            m_useBack = value;
            if(value)
            {
                m_Canavas = Utility.GameUtility.GetOrAddComponent<Canvas>(gameObject);
                m_Canavas.overrideSorting = true;
                m_GraphicRaycaster = Utility.GameUtility.GetOrAddComponent<GraphicRaycaster>(gameObject);
            }
        }
    }

    public bool isGame = false;
    protected RectTransform m_rectTranform = null;
    public RectTransform rectTranform
    {
        get
        {
            if(m_rectTranform == null)
            {
                m_rectTranform = gameObject.GetComponent<RectTransform>();
            }
            return m_rectTranform;
        }
    }
    public bool activeSelf
    {
        get
        {
            return gameObject.activeSelf;
        }
    }

    public virtual void AddBgUI(string path)
    {
        Res.ResourcesManager.Instance.AsyncLoadResource<GameObject>(path, Res.ResourceType.UI,(resName,obj)=>{
        });    
    }

    public virtual void SetOrderLayer(int index)
    {
        if(m_Canavas != null)
        {
            m_Canavas.sortingOrder = index;
            m_order = index;
        }
    }

    public virtual void Initalize()
    {
        int layer = 0;
        switch (eUICanvas)
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
        if (useBack)
        {
            AddBack();
            if (m_Canavas != null)
            {
                m_Canavas.overrideSorting = true;
            }
        }
    }

    public void SetOverrideSorting(bool b)
    {
        if(m_Canavas != null)
        {
            m_Canavas.overrideSorting = true;
        }
    }

    public virtual void AddBack()
    {
        GameObject go =   Res.ResourcesManager.Instance.SyncGetResource<GameObject>("CommomBack", Res.ResourceType.UI);
        go.transform.SetParent(transform);
        Utility.GameUtility.ResetTransform(go.transform);
        go.transform.SetAsFirstSibling();
        UGUIEventListener listerner = UGUIEventListener.Get(go);
        Graphic graphic = go.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
        }
        listerner.onClick = delegate (GameObject obj)
        {
            CloseUI();
        };
    }

    public virtual void CloseUI()
    {
        gameObject.SetActive(false);
        for(int i = 0; i < m_bgList.Count; ++i)
        {
            m_bgList[i].SetActive(false);
        }
        OnDisable();
    }

    public virtual void OpenUI()
    {
        gameObject.SetActive(true);
        for (int i = 0; i < m_bgList.Count; ++i)
        {
            m_bgList[i].SetActive(true);
        }
        //Utility.Event.EventDispatcher.Dispatch(Utility.Event.EventType.TriggerGuide, null, EGuideType.EGuideType_Panel,gameObject.name);
    }

    public virtual void Deinitialization()
    {
        if(m_Canavas != null)
        {
            m_Canavas.overrideSorting = false;
        }

        m_Canavas = null;
        m_GraphicRaycaster = null;
        GameObject.Destroy(gameObject);
        for (int i = 0; i < m_bgList.Count; ++i)
        {
            GameObject.Destroy(m_bgList[i]);
        }
        m_bgList.Clear();
    }

    #region 事件
    public void AddListClick(GameObject go, UGUIEventListener.VoidDelegate func)
    {
        if (go == null) return;
        Graphic graphic = go.GetComponent<Graphic>();
        if(graphic != null)
        {
            graphic.raycastTarget = true;
        }

        UGUIEventForList listerner = Utility.GameUtility.GetOrAddComponent<UGUIEventForList>(go);
        listerner.onClick = delegate (GameObject obj)
        {
            OnClick(obj, func);
        };
    }

    public void AddListDown(GameObject go, UGUIEventListener.VoidDelegate func)
    {
        if (go == null) return;
        Graphic graphic = go.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
        }
        UGUIEventForList listerner = Utility.GameUtility.GetOrAddComponent<UGUIEventForList>(go);
        listerner.onDown = delegate (GameObject obj)
        {
            OnClick(obj, func);
        };
    }

    public void AddListUp(GameObject go, UGUIEventListener.VoidDelegate func)
    {
        if (go == null) return;
        Graphic graphic = go.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
        }
        UGUIEventForList listerner = Utility.GameUtility.GetOrAddComponent<UGUIEventForList>(go);
        listerner.onUp = delegate (GameObject obj)
        {
            OnClick(obj, func);
        };
    }

    public void AddListPress(GameObject go, UGUIEventListener.VoidDelegate func)
    {
        if (go == null) return;
        Graphic graphic = go.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
        }
        UGUIEventForList listerner = Utility.GameUtility.GetOrAddComponent<UGUIEventForList>(go);
        listerner.onPress = delegate (GameObject obj)
        {
            OnClick(obj, func);
        };
    }

    //只添加新手引导方法,不准许添加其他方法
    public void AddGuideClick(GameObject go, UGUIEventListener.VoidDelegate func)
    {
        if (go == null || func == null) return;
        Graphic graphic = go.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
        }
        UGUIEventListener listerner = UGUIEventListener.Get(go);
        listerner.onClick += func;
    }

    //添加移除引导方法,不准许添加其他方法
    public void RemoveGuideClick(GameObject go, UGUIEventListener.VoidDelegate func)
    {
        if (go == null || func == null) return;
        Graphic graphic = go.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
        }
        UGUIEventListener listerner = UGUIEventListener.Get(go);
        listerner.onClick -= func;
    }


    public void AddClick(GameObject go, UGUIEventListener.VoidDelegate func)
    {
        if (go == null || func == null) return;
        Graphic graphic = go.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
        }
        UGUIEventListener listerner = UGUIEventListener.Get(go);
        listerner.onClick = func;
    }
    public void AddClickDown(GameObject go, UGUIEventListener.VoidDelegate func)
    {
        if (go == null || func == null) return;
        Graphic graphic = go.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
        }
        UGUIEventListener listerner = UGUIEventListener.Get(go);
        listerner.onDown = func;
    }
    public void AddClickUp(GameObject go, UGUIEventListener.VoidDelegate func)
    {
        if (go == null || func == null) return;
        Graphic graphic = go.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
        }
        UGUIEventListener listerner = UGUIEventListener.Get(go);
        listerner.onUp = func;
    }

    protected void OnClick(GameObject go, UGUIEventListener.VoidDelegate func)
    {
        if (go == null || func == null)
        {
            return;
        }
        func(go);
    }
    public void AddDrag(GameObject go)
    {
        if (go == null) return;
        Graphic graphic = go.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
        }
        UGUIEventListener listerner = UGUIEventListener.Get(go);
        listerner.onDrag = delegate (GameObject obj)
        {
            OnDrag(obj);
        };
    }

    public void AddDrag(GameObject go, UGUIEventListener.VoidDelegate func)
    {
        if (go == null || func == null) return;
        Graphic graphic = go.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
        }
        UGUIEventListener listerner = UGUIEventListener.Get(go);
        listerner.onDrag = func;
    }


    protected void OnDrag(GameObject go)
    {
        if (go == null)
        {
            return;
        }
    }

    public void AddClickDown(GameObject go)
    {
        if (go == null) return;
        Graphic graphic = go.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
        }
        UGUIEventListener listerner = UGUIEventListener.Get(go);
        listerner.onDown = delegate (GameObject obj)
        {
            OnClickDown(obj);
        };
    }

    protected void OnClickDown(GameObject go)
    {
        if (go == null)
        {
            return;
        }
    }

    public void AddClickUp(GameObject go)
    {
        if (go == null) return;
        Graphic graphic = go.GetComponent<Graphic>();
        if (graphic != null)
        {
            graphic.raycastTarget = true;
        }
        UGUIEventListener listerner = UGUIEventListener.Get(go);
        listerner.onUp = delegate (GameObject obj)
        {
            OnClickUp(obj);
        };
    }

    protected void OnClickUp(GameObject go)
    {
        if (go == null)
        {
            return;
        }
    }

    /// <summary>
    /// 删除单击事件
    /// </summary>
    /// <param name="go"></param>
    public void RemoveClick(GameObject go)
    {
        if (go == null) return;
        UGUIEventListener listerner = UGUIEventListener.Get(go);
        listerner.onClick = null;
    }

    public void RemoveDrag(GameObject go)
    {
        if (go == null) return;
        UGUIEventListener listerner = UGUIEventListener.Get(go);
        listerner.onDrag = null;
    }

    public void RemoveClickDown(GameObject go)
    {
        if (go == null) return;
        UGUIEventListener listerner = UGUIEventListener.Get(go);
        listerner.onDown = null;
    }


    public void RemoveClickUp(GameObject go)
    {
        if (go == null) return;
        UGUIEventListener listerner = UGUIEventListener.Get(go);
        listerner.onUp = null;
    }



    public void RemoveListClick(GameObject go)
    {
        if (go == null) return;
        UGUIEventForList listerner = go.GetComponent<UGUIEventForList>();
        if (listerner == null)
            return;
        listerner.onClick = null;
    }

    public void RemoveListDown(GameObject go)
    {
        if (go == null) return;
        UGUIEventForList listerner = go.GetComponent<UGUIEventForList>();
        if (listerner == null)
            return;
        listerner.onDown = null;
    }

    public void RemoveListUp(GameObject go)
    {
        if (go == null) return;
        UGUIEventForList listerner = go.GetComponent<UGUIEventForList>();
        if (listerner == null)
            return;
        listerner.onUp = null;
    }

    public void RemoveListPress(GameObject go)
    {
        if (go == null) return;
        UGUIEventForList listerner = go.GetComponent<UGUIEventForList>();
        if (listerner == null)
            return;
        listerner.onPress = null;
    }

    public void Top()
    {
        transform.SetAsLastSibling();
    }

    public void Bottom()
    {
        transform.SetAsFirstSibling();
    }

    public void AddPartical(GameObject go,string path,Vector3 localPosition, Vector3 localScale)
    {
        if(go == null || string.IsNullOrEmpty(path))
        {
            return;
        }

        Res.ResourcesManager.Instance.AsyncLoadResource<GameObject>(path, Res.ResourceType.Fx, (res,obj)=>
        {
            GameObject fxObject = obj as GameObject;
            if(fxObject != null)
            {
                fxObject.transform.SetParent(go.transform);
                GameUtility.ResetTransform(fxObject.transform);
                fxObject.transform.localPosition = localPosition;
                fxObject.transform.localScale = localScale;
                m_ParticalDic[path] = fxObject;
                Renderer[] renders = gameObject.GetComponentsInChildren<Renderer>();
                int count = renders.Length;
                for (int i = 0; i < count; i++)
                {
                    renders[i].sortingOrder = m_order + 1;
                }
            }
        });
    }


    protected void RemovePartical(string path)
    {
        if(m_ParticalDic.ContainsKey(path))
        {
            Res.ResourcesManager.Instance.UnloadResource(m_ParticalDic[path]);
            m_ParticalDic.Remove(path);
        }
    }
    #endregion
}

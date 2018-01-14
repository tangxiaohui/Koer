using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxPanelUI : UIBase
{
    protected UIText m_titleText = null;
    protected UIText m_contentText = null;
    protected UIText m_OKText = null;
    protected UIText m_CancelText = null;
    protected UIText m_SoloOKText = null;
    protected UIButton m_OKButton = null;
    protected UIButton m_CancelButton = null;
    protected UIButton m_closeButton = null;
    protected UIButton m_SoloOKButton = null;
    protected System.Action m_OkFun = null;
    protected System.Action m_CancelFun = null;
    protected bool bIsSolo = false;
    public override void Initalize()
    {
        base.Initalize();
        m_titleText = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "middleNode/title/UIText");
        m_contentText = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "middleNode/Content/Label");
        m_OKText = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "middleNode/OkBtn/UIText");
        m_CancelText = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "middleNode/NoBtn/UIText");
        m_SoloOKText = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "middleNode/SoloOkBtn/UIText");

        m_OKButton = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "middleNode/OkBtn");
        m_SoloOKButton = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "middleNode/SoloOkBtn");
        m_closeButton = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "middleNode/CloseButton");
        m_CancelButton = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "middleNode/NoBtn");

        m_OKButton.onClick.AddListener(OnClickOk);
        m_CancelButton.onClick.AddListener(OnClickCancel);
        m_closeButton.onClick.AddListener(OnClickCancel);
        m_SoloOKButton.onClick.AddListener(OnClickOk);
        CloseUI();
    }

    public override void Deinitialization()
    {
        base.Deinitialization();
        m_OkFun = null;
        m_CancelFun = null;
    }

    public void Show(string title, string content, string okText = "",string cancelText = "")
    {
        bIsSolo = false;
        isShowCancelButton(true);
        m_titleText.text = title;
        m_contentText.text = content;
        if(!string.IsNullOrEmpty(okText))
        {
            m_OKText.text = okText;
        }
        else
        {
            m_OKText.text = TextManager.Instance.GetString(TEXTS.Text_MessageBox_OK);
        }

        if (!string.IsNullOrEmpty(cancelText))
        {
            m_CancelText.text = cancelText;
        }
        else
        {
            m_CancelText.text = TextManager.Instance.GetString(TEXTS.Text_MessageBox_Cancel);
        }
    }

    public void ShowSolo(string title, string content, System.Action okFun = null, string okText = "")
    {
        bIsSolo = true;
        isShowCancelButton(false);
        m_titleText.text = title;
        m_contentText.text = content;
        m_OkFun = okFun;
        if (!string.IsNullOrEmpty(okText))
        {
            m_SoloOKText.text = okText;
        }
        else
        {
            m_SoloOKText.text = TextManager.Instance.GetString(TEXTS.Text_MessageBox_OK);
        }
    }

    public void ShowSolo(string title, string content,string okText)
    {
        bIsSolo = true;
        isShowCancelButton(false);
        m_titleText.text = title;
        m_contentText.text = content;
        m_OkFun = null;
        if (!string.IsNullOrEmpty(okText))
        {
            m_SoloOKText.text = okText;
        }
        else
        {
            m_SoloOKText.text = TextManager.Instance.GetString(TEXTS.Text_MessageBox_OK);
        }
    }

    public void OnClickOk()
    {
        if (m_OkFun != null)
        {
            m_OkFun();
        }
        CloseUI();
    }

    public override void AddBack()
    {
        GameObject go = Res.ResourcesManager.Instance.SyncGetResource<GameObject>("CommomBack", Res.ResourceType.UI);
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
            OnClickCancel();
        };
    }


    public void OnClickCancel()
    {
        if(bIsSolo)
        {
            OnClickOk();
            return;
        }

        if (m_CancelFun != null)
        {
            m_CancelFun();
        }
        CloseUI();
    }

    public override void OpenUI()
    {
        base.OpenUI();
    }
    public override void CloseUI()
    {
        base.CloseUI();
        m_OkFun = null;
        m_CancelFun = null;
    }

    protected void isShowCancelButton(bool b)
    {
        m_OKButton.gameObject.SetActive(b);
        m_CancelButton.gameObject.SetActive(b);
        m_SoloOKButton.gameObject.SetActive(!b);
    }

    public void Show(string title, string content, System.Action okFun = null, System.Action cancel = null, string okText = "", string cancelText = "")
    {
        bIsSolo = false;
        isShowCancelButton(true);
        m_titleText.text = title;
        m_contentText.text = content;
        m_OkFun = okFun;
        m_CancelFun = cancel;
        if (!string.IsNullOrEmpty(okText))
        {
            m_OKText.text = okText;
        }
        else
        {
            m_OKText.text = TextManager.Instance.GetString(TEXTS.Text_MessageBox_OK);
        }

        if (!string.IsNullOrEmpty(cancelText))
        {
            m_OKText.text = cancelText;
        }
        else
        {
            m_CancelText.text = TextManager.Instance.GetString(TEXTS.Text_MessageBox_Cancel);
        }
    }
}

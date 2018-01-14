using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerNoticeUI : UIBase
{
    private UIText m_titleText = null;
    private UIText m_contentText = null;
    public override void Initalize()
    {
        base.Initalize();
        m_titleText = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "title/UIText");
        m_contentText = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "Content/UIText");
        GameObject CloseBtn = Utility.GameUtility.FindDeepChildGameObject(gameObject, "CloseBtn");
        AddClick(CloseBtn,OnClick);
        //UIManager.Instance.OpenUI(this);
        UpdateInfo();
    }

    public void OnClick(GameObject go)
    {
        CloseUI();
    }
    public void UpdateInfo()
    {
        m_titleText.text = ClientProxy.Instance.notice_title;//TextManager.Instance.GetString(TEXTS.Text_Notice_Title);
        m_contentText.text = ClientProxy.Instance.notice_content;
    }

    public void Update()
    {

    }

    public override void OpenUI()
    {
        gameObject.SetActive(true);
        //Top();
        UpdateInfo();
    }


    public override void CloseUI()
    {
        gameObject.SetActive(false);
    }
}

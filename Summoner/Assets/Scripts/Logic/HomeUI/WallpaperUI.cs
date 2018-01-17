using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res;

public class WallpaperUI : UIBase {

    protected UIScrollView m_sroll = null;
    protected UIButton CloseBtn = null;
    protected UIButton RightBtn = null;
    protected UIButton LeftBtn = null;
    private int CurIndex = 0;

    public override void Initalize()
    {
        base.Initalize();
        m_sroll = Utility.GameUtility.FindDeepChild<UIScrollView>(gameObject, "scrollview");
        CloseBtn = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "CloseBtn");
        RightBtn = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "arrowrightimg");
        LeftBtn = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "arrowleftimg");
        AddClick(CloseBtn.gameObject, OnClickCloseBtn);
        AddClick(RightBtn.gameObject, OnClickRightBtn);
        AddClick(LeftBtn.gameObject, OnClickLeftBtn);
        ClearContentSroll(MyPlayer.Instance.data.CardList.Count);
    }

    public void OnClickCloseBtn(GameObject obj)
    {
        Debug.Log("OnClickBattleBtn!");
        UIManager.Instance.OpenUI(EUIName.HomeUI);
        CloseUI();
    }

    public void ClearContentSroll(int count)
    {
        m_sroll.ClearContent();
        m_sroll.Col = 1;
        //m_sroll.horizontal = false;
        m_sroll.InitializeItem = UpdateScoll;
        m_sroll.AutoAligningCallBack = AutoAligningCallBack;
        for (int i = 0; i < count; ++i)
        {
            m_sroll.AddItem();
        }
        m_sroll.Initialize();
    }

    protected void UpdateScoll(int index, GameObject go)
    {
        Cards cardData = Cards.Get(MyPlayer.Instance.data.CardList[index]);

        Utility.GameUtility.FindDeepChild<UIImage>(go, "card").sprite = ResourcesManager.Instance.SyncGetCardImgInAltas(MyPlayer.Instance.data.CardList[index]);
        Utility.GameUtility.FindDeepChild<UIText>(go, "name").text = cardData.Name;
        RefreshSelectBtn();
    }

    protected void AutoAligningCallBack(int index)
    {
        CurIndex = index;
        RefreshSelectBtn();
    }

    public void SetShowCard(int index)
    {
        CurIndex = index;
        m_sroll.JumpToIndex(CurIndex + 1);
        RefreshSelectBtn();
    }

    private void OnClickRightBtn(GameObject obj)
    {
        if (CurIndex - 1 > MyPlayer.Instance.data.CardList.Count) return;
        CurIndex = CurIndex + 1;
        m_sroll.JumpToIndex(CurIndex+1);
        RefreshSelectBtn();
    }

    private void OnClickLeftBtn(GameObject obj)
    {
        if (CurIndex - 1 < 0) return;
        CurIndex = CurIndex - 1;
        m_sroll.JumpToIndex(CurIndex+1);
        RefreshSelectBtn();
    }

    public void RefreshSelectBtn()
    {
        if (CurIndex == 0)
            LeftBtn.gameObject.SetActive(false);
        else
            LeftBtn.gameObject.SetActive(true);

        if (CurIndex == MyPlayer.Instance.data.CardList.Count - 1)
            RightBtn.gameObject.SetActive(false);
        else
            RightBtn.gameObject.SetActive(true);

        //Debug.Log("CurIndex: " + CurIndex.ToString());
    }

    public override void OpenUI()
    {
        base.OpenUI();
        gameObject.SetActive(true);

        Initalize();
    }

    public override void CloseUI()
    {
        base.CloseUI();
        gameObject.SetActive(false);
    }
}

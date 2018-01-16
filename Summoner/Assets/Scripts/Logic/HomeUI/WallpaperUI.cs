using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallpaperUI : UIBase {

    protected UIScrollView m_sroll = null;
    protected UIButton CloseBtn = null;

    public override void Initalize()
    {
        base.Initalize();
        m_sroll = Utility.GameUtility.FindDeepChild<UIScrollView>(gameObject, "scrollview");
        CloseBtn = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "CloseBtn");
        AddClick(CloseBtn.gameObject, OnClickBattleBtn);
        ClearContentSroll(MyPlayer.Instance.data.CardList.Count);
    }

    public void OnClickBattleBtn(GameObject obj)
    {
        Debug.Log("OnClickBattleBtn!");
        SinglePanelManger.Instance.ShowBattleUI();
        CloseUI();
    }

    public void ClearContentSroll(int count)
    {
        m_sroll.ClearContent();
        m_sroll.Col = 1;
        //m_sroll.horizontal = false;
        m_sroll.InitializeItem = UpdateScoll;
        for (int i = 0; i < count; ++i)
        {
            m_sroll.AddItem();
        }
        m_sroll.Initialize();
    }

    protected void UpdateScoll(int index, GameObject go)
    {
        Debug.Log("index:" + index);
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

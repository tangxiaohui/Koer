using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUI : UIBase {
    protected UIScrollView m_sroll = null;
    protected UIButton BattleBtn = null;

    public override void Initalize()
    {
        base.Initalize();
        m_sroll = Utility.GameUtility.FindDeepChild<UIScrollView>(gameObject, "scrollview");
        BattleBtn = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "PlayBtn");
        AddClick(BattleBtn.gameObject, OnClickBattleBtn);
        ClearContentSroll(250);
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
        m_sroll.Col = 3;
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
        //Debug.Log("index:" + index);

        SinglePanelManger.Instance.ShowWallpaperUI();
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

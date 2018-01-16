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
        ClearContentSroll(MyPlayer.Instance.data.CardList.Count);
        Cards Test = Cards.Get(1);
        Debug.Log("-----------------:" + Test.Name);
    }

    public void OnClickBattleBtn(GameObject obj)
    {
        Debug.Log("OnClickBattleBtn!");
        //UIManager.Instance.OpenUI(EUIName.BattleUI);
        //CloseUI();
        SerachEnemy();
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
        UIText name = Utility.GameUtility.FindDeepChild<UIText>(go, "name");
        name.text = MyPlayer.Instance.data.CardList[index].ToString();
        AddListClick(go, OnClickItem);
    }

    private void OnClickItem(GameObject obj)
    {
        UIManager.Instance.OpenUI(EUIName.WallpaperUI);
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

    public void SerachEnemy()
    {
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("SerachEnemy");
        Debug.Log("发送 " + protocol.GetDesc());
        NetMgr.srvConn.Send(protocol, OnSerachEnemyBack);
    }

    public void OnSerachEnemyBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        List<int> cardList = proto.GetIntList(start, ref start);
        EnemyPlayer.Instance.id = id;
        EnemyPlayer.Instance.data.CardList = cardList;
        
        if (id.Length > 0)
        {
            SinglePanelManger.Instance.PushTips("找到敌人" + EnemyPlayer.Instance.id);
            UIManager.Instance.OpenUI(EUIName.BattleUI);
            CloseUI();
        }
        else
        {
            SinglePanelManger.Instance.PushTips("没有找到敌人");
        }
    }
}

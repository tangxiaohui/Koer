using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res;

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
        if (MyPlayer.Instance.data.BattleCardList.Count < 6)
        {
            SinglePanelManger.Instance.PushTips("你还没召唤出战英雄!");
            return;
        }
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
        //Debug.Log("card:" + card.CardID.ToString());
        UIText name = Utility.GameUtility.FindDeepChild<UIText>(go, "name");
        name.text = MyPlayer.Instance.data.CardList[index].ToString();
        AddListClick(go, OnClickItem);
        go.name = index.ToString();
        Cards card = Cards.Get(MyPlayer.Instance.data.CardList[index]);
        go.transform.Find("card").GetComponent<UIImage>().sprite = ResourcesManager.Instance.SyncGetCardImgInAltas(card.CardID);

        UIButton battlebtn = Utility.GameUtility.FindDeepChild<UIButton>(go, "battlebtn");
        battlebtn.gameObject.SetActive(true);
        foreach (int id in MyPlayer.Instance.data.BattleCardList)
        {
            if (id == card.CardID)
            {
                battlebtn.gameObject.SetActive(false);
                return;
            }
        }
        AddListClick(battlebtn.gameObject, delegate(GameObject obj){
            if (MyPlayer.Instance.data.BattleCardList.Count < 6)
                MyPlayer.Instance.data.BattleCardList.Add(card.CardID);
            SetMyBattleCardList();
        });
    }

    private void OnClickItem(GameObject obj)
    {
        UIManager.Instance.OpenUI(EUIName.WallpaperUI);
        WallpaperUI wallpaperUI = (WallpaperUI)UIManager.Instance.GetUI(EUIName.WallpaperUI);
        if(wallpaperUI != null)
        {
            wallpaperUI.SetShowCard(int.Parse(obj.name));
        }
    }

    public void SetTheCardBattle(GameObject obj)
    {
        if (MyPlayer.Instance.data.BattleCardList.Count < 6)
            MyPlayer.Instance.data.BattleCardList.Add(int.Parse(obj.name));
        SetMyBattleCardList();
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

    public void SetMyBattleCardList()
    {
        if (MyPlayer.Instance.data.BattleCardList.Count < 6)
            return;
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("SetMyBattleCardList");
        Debug.Log("发送 " + protocol.GetDesc());
        protocol.AddIntList(MyPlayer.Instance.data.BattleCardList);
        NetMgr.srvConn.Send(protocol, OnSetMyBattleCardListBack);
    }

    public void OnSetMyBattleCardListBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        List<int> MyPlayerBattleCardList = proto.GetIntList(start, ref start);
        MyPlayer.Instance.data.BattleCardList = MyPlayerBattleCardList;
        if (ret == 0)
        {
            Debug.Log("我的出战卡牌数: " + MyPlayerBattleCardList.Count);
        }
        else
        {
            SinglePanelManger.Instance.PushTips("出战失败!");
        }
    }

    public void OnSerachEnemyBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        string id = proto.GetString(start, ref start);
        List<int> EnemyBattleCardList = proto.GetIntList(start, ref start);
        EnemyPlayer.Instance.id = id;
        EnemyPlayer.Instance.data = new PlayerData(null, EnemyBattleCardList);

        if (id.Length > 0)
        {
            SinglePanelManger.Instance.PushTips("找到敌人" + EnemyPlayer.Instance.id);
            Debug.Log("敌人出战卡牌数：" + EnemyBattleCardList.Count);
            UIManager.Instance.OpenUI(EUIName.BattleUI);
            CloseUI();
        }
        else
        {
            SinglePanelManger.Instance.PushTips("没有找到敌人");
        }
    }
}

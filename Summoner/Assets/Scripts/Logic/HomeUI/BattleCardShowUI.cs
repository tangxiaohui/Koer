using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res;

public class BattleCardShowUI : UIBase {
    protected UIScrollView m_sroll = null;
    protected UIButton SureBtn = null;

    public override void Initalize()
    {
        base.Initalize();
        m_sroll = Utility.GameUtility.FindDeepChild<UIScrollView>(gameObject, "scrollview");
        SureBtn = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "PlayBtn");
        AddClick(SureBtn.gameObject, OnClickSureBtn);
        ClearContentSroll(6);
    }

    public void OnClickSureBtn(GameObject obj)
    {
        SetMyBattleCardList();
        CloseUI();
        UIManager.Instance.OpenUI(EUIName.HomeUI);
    }

    public void SetMyBattleCardList()
    {
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
            Debug.Log("出战失败!");
        }
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
        name.text = "";
        go.name = index.ToString();
        UIButton battlebtn = Utility.GameUtility.FindDeepChild<UIButton>(go, "battlebtn");
        battlebtn.gameObject.SetActive(false);
        Utility.GameUtility.FindDeepChild(go, "UIPoint").gameObject.SetActive(false);
        go.transform.Find("card").GetComponent<UIImage>().sprite = null;
        if (index >= MyPlayer.Instance.data.BattleCardList.Count)
            return;
        Cards card = Cards.Get(MyPlayer.Instance.data.BattleCardList[index]);
        name.text = card.CardID.ToString();
        AddListClick(go, OnClickItem);
        go.transform.Find("card").GetComponent<UIImage>().sprite = ResourcesManager.Instance.SyncGetCardImgInAltas(card.CardID);

        battlebtn.gameObject.SetActive(true);
        UIText btntext = Utility.GameUtility.FindDeepChild<UIText>(battlebtn.gameObject, "name");
        btntext.text = "下阵";
        AddListClick(battlebtn.gameObject, delegate (GameObject obj) {
            MyPlayer.Instance.data.BattleCardList.Remove(card.CardID);
            ClearContentSroll(6);
        });
    }

    private void OnClickItem(GameObject obj)
    {
        UIManager.Instance.OpenUI(EUIName.WallpaperUI);
        WallpaperUI wallpaperUI = (WallpaperUI)UIManager.Instance.GetUI(EUIName.WallpaperUI);
        if (wallpaperUI != null)
        {
            wallpaperUI.SetShowCard(int.Parse(obj.name));
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchEnemyUI : UIBase
{
    public UIButton SearchBtn = null;
    public UIButton InvestigationBtn = null;
    public UIButton FightBtn = null;
    public UIButton CloseBtn = null;
    public UIText EnemyNameText = null;
    public UIText PowerText = null;

    public override void Initalize()
    {
        base.Initalize();
        SearchBtn = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "SearchBtn");
        InvestigationBtn = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "InvestigationBtn");
        FightBtn = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "FightBtn");
        CloseBtn = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "CloseBtn");
        EnemyNameText = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "EnemyText");
        PowerText = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "PowerText");

        AddClick(SearchBtn.gameObject, OnClickSearchBtn);
        AddClick(InvestigationBtn.gameObject, OnClickInvestigationBtn);
        AddClick(FightBtn.gameObject, OnClickFightBtn);
        AddClick(CloseBtn.gameObject, OnClickCloseBtn);
    }

    public void OnClickCloseBtn(GameObject obj)
    {
        CloseUI();
        UIManager.Instance.OpenUI(EUIName.HomeUI);
    }

    public void OnClickSearchBtn(GameObject obj)
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
        List<int> EnemyBattleCardList = proto.GetIntList(start, ref start);
        EnemyPlayer.Instance.id = id;
        EnemyPlayer.Instance.data = new PlayerData(null, EnemyBattleCardList);

        if (id.Length > 0)
        {
            SinglePanelManger.Instance.PushTips("找到敌人" + EnemyPlayer.Instance.id);
            Debug.Log("敌人出战卡牌数：" + EnemyBattleCardList.Count);
            RefreshUI();
        }
        else
        {
            SinglePanelManger.Instance.PushTips("没有找到敌人");
        }
    }

    public void RefreshUI()
    {
        EnemyNameText.text = EnemyPlayer.Instance.id.ToString();
        PowerText.text = "战力值：88888888888";
    }

    public void OnClickInvestigationBtn(GameObject obj)
    {
        CloseUI();
        UIManager.Instance.OpenUI(EUIName.BattleUI);
        BattleUI battleUI = (BattleUI)UIManager.Instance.GetUI(EUIName.BattleUI);
        battleUI.RefreshBattleUI(BattleUI.BattleType.Investigation);
    }

    public void OnClickFightBtn(GameObject obj)
    {
        CloseUI();
        UIManager.Instance.OpenUI(EUIName.BattleUI);
        BattleUI battleUI = (BattleUI)UIManager.Instance.GetUI(EUIName.BattleUI);
        battleUI.RefreshBattleUI(BattleUI.BattleType.Fight);
    } 

    public override void CloseUI()
    {
        base.CloseUI();
    }

    public override void OpenUI()
    {
        base.OpenUI();

        Initalize();
    }
}

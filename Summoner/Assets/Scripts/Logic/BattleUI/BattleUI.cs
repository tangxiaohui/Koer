using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res;

public class BattleUI : UIBase
{
    public enum BattleType
    {
        Investigation,
        Fight
    };
    public BattleType BatType = BattleType.Investigation;
    public UIText TimeText = null;
    public UIButton FightBtn = null;
    public UIButton LeaveBtn = null;
    private GameObject ownerCardPanel = null;
    private UIScrollView m_sroll = null;
    protected UIButton CloseBtn = null;
    private Dictionary<int, UIImage> HpImgDic = new Dictionary<int, UIImage>();

    public override void Initalize()
    {
        base.Initalize();
        BattleManager.Instance.InitBattleData();
        ownerCardPanel = ResourcesManager.Instance.SyncGetResource(ClassType.GameObject, "BattleUI/FashionCards", ResourceType.UI) as GameObject;
        ownerCardPanel.transform.SetParent(UIManager.Instance.GetCanvas(EUICanvas.EUICanvas_Normal).transform);
        ownerCardPanel.SetActive(true);
        ownerCardPanel.GetComponent<BattleCardsPanel>().Initalize();
        CloseBtn = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "CloseBtn");
        m_sroll = Utility.GameUtility.FindDeepChild<UIScrollView>(gameObject, "scrollview");
        ClearContentSroll(EnemyPlayer.Instance.data.BattleCardList.Count);
        TimeText = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "TimeText");
        FightBtn = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "FightBtn");
        LeaveBtn = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "LeaveBtn");
        AddClick(CloseBtn.gameObject, OnClickCloseBtn);
        AddClick(FightBtn.gameObject, OnClickFightBtn);
        AddClick(LeaveBtn.gameObject, OnClickLeaveBtn);

        BattleManager.Instance.OnFightOpComplete += UpdateHpImg;
    }

    public void OnClickFightBtn(GameObject obj)
    {
        RefreshBattleUI(BattleType.Fight);
    }

    public void OnClickLeaveBtn(GameObject obj)
    {
        UIManager.Instance.OpenUI(EUIName.HomeUI);
        CloseUI();
    }

    public void RefreshBattleUI(BattleType battleType)
    {
        if (battleType == BattleType.Investigation)
        {
            TimeText.gameObject.SetActive(true);
            FightBtn.gameObject.SetActive(true);
            LeaveBtn.gameObject.SetActive(true);
            StartCoroutine("StartTime");
        }
        else if (battleType == BattleType.Fight)
        {
            TimeText.gameObject.SetActive(false);
            FightBtn.gameObject.SetActive(false);
            LeaveBtn.gameObject.SetActive(false);
        }
    }

    IEnumerator StartTime()
    {
        int time = 18;
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
            TimeText.text = string.Format("还有{0}秒战斗开始！", time);
        }
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
        Cards card = Cards.Get(EnemyPlayer.Instance.data.BattleCardList[index]);
        go.name = card.CardID.ToString();
        go.transform.Find("card").GetComponent<UIImage>().sprite = ResourcesManager.Instance.SyncGetCardImgInAltas(card.CardID);
        AddListClick(go, OnClickCard);

        UIImage m_Hp = Utility.GameUtility.FindDeepChild<UIImage>(go, "HpNode/Hp");
        UIImage m_Process = Utility.GameUtility.FindDeepChild<UIImage>(go, "HpNode/HpProcess");
        UIText m_NameText = Utility.GameUtility.FindDeepChild<UIText>(go, "HpNode/Name");
        UIText m_LevelText = Utility.GameUtility.FindDeepChild<UIText>(go, "HpNode/Level");
        m_Hp.fillAmount = 1.0f;

        BattleCard battleCard = BattleManager.Instance.EnemyCardDic[card.CardID];
        m_Hp.fillAmount = battleCard.HpPercent();
        m_NameText.text = battleCard.BaseData.Name;

        if (!HpImgDic.ContainsKey(card.CardID))
            HpImgDic.Add(card.CardID, m_Hp);
    }

    public void UpdateHpImg()
    {
        BattleCard battleCard = BattleManager.Instance.GetCurrentEnemyFightCard();
        HpImgDic[battleCard.Id].fillAmount = battleCard.HpPercent();
    }

    public void OnClickCard(GameObject obj)
    {
        Debug.Log("AAAAAAAAAAAAAA");
        BattleManager.Instance.FightOp(int.Parse(obj.name));
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
        StopCoroutine("StartTime");
    }
}

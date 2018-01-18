using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res;

public class BattleCardsPanel : MonoBehaviour {
    protected UIFashionCard m_sroll = null;
    private Dictionary<int, GameObject> HpImgDic = new Dictionary<int, GameObject>();

    void Start () {
        Initalize();
    }

    public void Initalize()
    {
        m_sroll = Utility.GameUtility.FindDeepChild<UIFashionCard>(gameObject, "Cards");
        m_sroll.ClearContent();
        m_sroll.InitUnChose = InitUnChose;
        m_sroll.InitChosed = InitChosed;
        m_sroll.InitializeItem = InitCardItem;
        m_sroll.AddItem(MyPlayer.Instance.data.BattleCardList.Count);

        BattleManager.Instance.OnFightOpComplete += UpdateHpImg;
    }

    public void InitCardItem(int index, GameObject item)
    {
        UIText name = Utility.GameUtility.FindDeepChild<UIText>(item, "Name");
        UIImage m_Hp = Utility.GameUtility.FindDeepChild<UIImage>(item, "HpNode/Hp");
        UIImage m_Process = Utility.GameUtility.FindDeepChild<UIImage>(item, "HpNode/HpProcess");
        UIText m_NameText = Utility.GameUtility.FindDeepChild<UIText>(item, "HpNode/Name");
        UIText m_LevelText = Utility.GameUtility.FindDeepChild<UIText>(item, "HpNode/Level");
        m_Hp.fillAmount = 1.0f;
        Cards card = Cards.Get(MyPlayer.Instance.data.BattleCardList[index]);

        Utility.GameUtility.FindDeepChild<UIImage>(item, "pic").sprite = ResourcesManager.Instance.SyncGetCardImgInAltas(card.CardID);
        item.name = card.CardID.ToString();
        name.text = card.CardID.ToString();
        BattleCard battleCard = BattleManager.Instance.MyPlayerCardDic[card.CardID];
        m_Hp.fillAmount = battleCard.HpPercent();
        m_NameText.text = battleCard.BaseData.Name;

        if (!HpImgDic.ContainsKey(card.CardID))
            HpImgDic.Add(card.CardID, item);
    }

    public void UpdateHpImg()
    {
        BattleCard battleCard = BattleManager.Instance.GetCurrentMyFightCard();
        Utility.GameUtility.FindDeepChild<UIImage>(HpImgDic[battleCard.Id], "HpNode/Hp").fillAmount = battleCard.HpPercent();
    }

    public void InitUnChose(GameObject obj)
    {
        UIText name = Utility.GameUtility.FindDeepChild<UIText>(obj, "Name");
        name.color = Color.gray;
        Transform mask = Utility.GameUtility.FindDeepChild(obj, "mask");
        mask.gameObject.SetActive(true);
    }

    public void InitChosed(GameObject obj)
    {
        UIText name = Utility.GameUtility.FindDeepChild<UIText>(obj, "Name");
        name.color = Color.white;
        Transform mask = Utility.GameUtility.FindDeepChild(obj, "mask");
        mask.gameObject.SetActive(false);
        BattleManager.Instance.MyFightCardID = int.Parse(name.text);
    }

    public void RefreshCards()
    {
        m_sroll.ClearContent();
        m_sroll.AddItem(MyPlayer.Instance.data.BattleCardList.Count);
    }

    private void RefreshCurrCardInfo()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res;

public class BattleCardsPanel : MonoBehaviour {
    protected UIFashionCard m_sroll = null;
    private List<GameObject> HpImgDic = new List<GameObject>();

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
        HpImgDic.Clear();
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

        HpImgDic.Add(item);
    }

    public void UpdateHpImg()
    {
        BattleCard battleCard = BattleManager.Instance.GetCurrentMyFightCard();
        foreach(GameObject i in HpImgDic)
        {
            if(battleCard.Id == int.Parse(i.name))
            {
                Utility.GameUtility.FindDeepChild<UIImage>(i, "HpNode/Hp").fillAmount = battleCard.HpPercent();
                break;
            }
        }
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

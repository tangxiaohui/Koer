using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCardsPanel : MonoBehaviour {
    protected UIFashionCard m_sroll = null;

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
        m_sroll.AddItem(MyPlayer.Instance.data.CardList.Count);
    }

    public void InitCardItem(int index, GameObject item)
    {
        UIText name = Utility.GameUtility.FindDeepChild<UIText>(item, "Name");
        name.text = MyPlayer.Instance.data.CardList[index].ToString();
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
    }

    public void RefreshCards()
    {
        m_sroll.ClearContent();
        m_sroll.AddItem(MyPlayer.Instance.data.CardList.Count);
    }

    private void RefreshCurrCardInfo()
    {

    }
}

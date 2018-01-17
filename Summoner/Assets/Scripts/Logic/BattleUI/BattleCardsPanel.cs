using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res;

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
        m_sroll.AddItem(MyPlayer.Instance.data.BattleCardList.Count);

        foreach (int i in MyPlayer.Instance.data.BattleCardList)
            Debug.Log(i);
    }

    public void InitCardItem(int index, GameObject item)
    {
        UIText name = Utility.GameUtility.FindDeepChild<UIText>(item, "Name");
        name.text = MyPlayer.Instance.data.BattleCardList[index].ToString();
        Utility.GameUtility.FindDeepChild<UIImage>(item, "pic").sprite = ResourcesManager.Instance.SyncGetCardImgInAltas(MyPlayer.Instance.data.BattleCardList[index]);
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
        m_sroll.AddItem(MyPlayer.Instance.data.BattleCardList.Count);
    }

    private void RefreshCurrCardInfo()
    {

    }
}

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
        m_sroll.AddItem(108);
    }

    public void InitUnChose(GameObject obj)
    {

    }

    public void InitChosed(GameObject obj)
    {

    }
}

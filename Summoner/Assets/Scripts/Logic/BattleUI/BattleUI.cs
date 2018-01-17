using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res;

public class BattleUI : UIBase
{
    private GameObject ownerCardPanel = null;
    private UIScrollView m_sroll = null;
    public override void Initalize()
    {
        base.Initalize();

        ownerCardPanel = ResourcesManager.Instance.SyncGetResource(ClassType.GameObject, "BattleUI/FashionCards", ResourceType.UI) as GameObject;
        ownerCardPanel.transform.SetParent(UIManager.Instance.GetCanvas(EUICanvas.EUICanvas_Normal).transform);
        ownerCardPanel.SetActive(true);
        ownerCardPanel.GetComponent<BattleCardsPanel>().Initalize();

        m_sroll = Utility.GameUtility.FindDeepChild<UIScrollView>(gameObject, "scrollview");
        ClearContentSroll(EnemyPlayer.Instance.data.BattleCardList.Count);
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
        go.name = index.ToString();
        Cards card = Cards.Get(EnemyPlayer.Instance.data.BattleCardList[index]);
        //Debug.Log("card:" + card.CardID.ToString());
        go.transform.Find("card").GetComponent<UIImage>().sprite = ResourcesManager.Instance.SyncGetCardImgInAltas(card.CardID);
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

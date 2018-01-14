using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res;

public class BattleUI : UIBase
{
    private GameObject ownerCardPanel = null;
    public override void Initalize()
    {
        base.Initalize();

        ownerCardPanel = ResourcesManager.Instance.SyncGetResource(ClassType.GameObject, "BattleUI/FashionCards", ResourceType.UI) as GameObject;
        ownerCardPanel.transform.SetParent(UIManager.Instance.GetCanvas(EUICanvas.EUICanvas_Normal).transform);
        ownerCardPanel.SetActive(false);
    }

    public override void OpenUI()
    {
        base.OpenUI();
        gameObject.SetActive(true);
        ownerCardPanel.SetActive(true);
    }

    public override void CloseUI()
    {
        base.CloseUI();

        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResigsterSucceesUI : UIBase
{
    public UIText accountText = null;
    public UIText passwordText = null;
    public UIText contentText = null;
    public UIButton BackBtn = null;

    public override void Initalize()
    {
        base.Initalize();
        accountText = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "accountText");
        passwordText = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "passwordText");
        contentText = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "contentText");
        BackBtn = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "BackBtn");
        AddClick(BackBtn.gameObject, BackLoginUI);
    }

    public void BackLoginUI(GameObject obj)
    {
        UIManager.Instance.OpenUI(EUIName.AccountUI);
    }

    public override void CloseUI()
    {
        base.CloseUI();
        gameObject.SetActive(false);
    }

    public void UpdateInfo(string account, string password, string content)
    {
        accountText.text = account;
        passwordText.text = password;
        contentText.text = content;
    }

    public override void OpenUI()
    {
        base.OpenUI();
        gameObject.SetActive(true);
        Initalize();
    }
}

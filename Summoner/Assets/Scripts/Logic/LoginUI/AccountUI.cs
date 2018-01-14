using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountUI : UIBase
{
    public UIInputField AccountText = null;
    public UIInputField PasswordText = null;
    public UIText FogetPassWord = null;
    public UIButton SureButton = null;
    public UIScrollView AccountList = null;

    private void Start()
    {
        AccountText = Utility.GameUtility.FindDeepChild<UIInputField>(gameObject, "AccountInputField");
        PasswordText = Utility.GameUtility.FindDeepChild<UIInputField>(gameObject, "PassWordInputField");
        FogetPassWord = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "FogetPassWord");
        SureButton = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "SureButton");
        AddClick(SureButton.gameObject, OnClickPopBtn);

        AccountList = Utility.GameUtility.FindDeepChild<UIScrollView>(gameObject, "Account");
    }

    private void OnClickPopBtn(GameObject obj)
    {
        Debug.Log("OnClickPopBtn!");
        SinglePanelManger.Instance.ShowHomeUI();
        CloseUI();
    }

    public override void OpenUI()
    {
        base.OpenUI();
        gameObject.SetActive(true);
    }

    public override void CloseUI()
    {
        base.CloseUI();

        gameObject.SetActive(false);
    }
}

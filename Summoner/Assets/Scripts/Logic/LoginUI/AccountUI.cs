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
	public UIButton RegisterButton = null;

    private void Start()
    {
        AccountText = Utility.GameUtility.FindDeepChild<UIInputField>(gameObject, "AccountInputField");
        PasswordText = Utility.GameUtility.FindDeepChild<UIInputField>(gameObject, "PassWordInputField");
        FogetPassWord = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "FogetPassWord");
        SureButton = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "SureButton");
        AddClick(SureButton.gameObject, OnClickPopBtn);

		RegisterButton = Utility.GameUtility.FindDeepChild<UIButton> (gameObject, "RegisterButton");
		AddClick (RegisterButton.gameObject, OnClickRegisterButton);

        AccountList = Utility.GameUtility.FindDeepChild<UIScrollView>(gameObject, "Account");
    }

    private void OnClickPopBtn(GameObject obj)
    {
        Debug.Log("OnClickPopBtn!");
        SinglePanelManger.Instance.ShowHomeUI();
        CloseUI();
    }

	private void OnClickRegisterButton(GameObject obj)
	{
		SinglePanelManger.Instance.ShowRegistUI();
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

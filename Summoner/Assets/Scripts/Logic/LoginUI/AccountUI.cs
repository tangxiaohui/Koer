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

    public override void Initalize()
    {
        base.Initalize();
        AccountText = Utility.GameUtility.FindDeepChild<UIInputField>(gameObject, "AccountInputField");
        PasswordText = Utility.GameUtility.FindDeepChild<UIInputField>(gameObject, "PassWordInputField");
        FogetPassWord = Utility.GameUtility.FindDeepChild<UIText>(gameObject, "FogetPassWord");
        SureButton = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "SureButton");
        AddClick(SureButton.gameObject, OnClickSureButton);

        RegisterButton = Utility.GameUtility.FindDeepChild<UIButton>(gameObject, "RegisterButton");
        AddClick(RegisterButton.gameObject, OnClickRegisterButton);

        AccountList = Utility.GameUtility.FindDeepChild<UIScrollView>(gameObject, "Account");
    }

    private void OnClickSureButton(GameObject obj)
    {
        Debug.Log("OnClickPopBtn!");
        //用户名密码为空
        if (AccountText.text == "" || PasswordText.text == "")
        {
            SinglePanelManger.Instance.PushTips(TextManager.Instance.GetString(TEXTS.Text_Tips_AcOrPwIsNull));
            Debug.Log("用户名密码不能为空!");
            return;
        }
        //连接服务器
        if (NetMgr.srvConn.status != Connection.Status.Connected)
        {
            string host = "127.0.0.1";
            int port = 1234;
            NetMgr.srvConn.proto = new ProtocolBytes();
            if (!NetMgr.srvConn.Connect(host, port))
            {
                SinglePanelManger.Instance.PushTips(TextManager.Instance.GetString(TEXTS.Text_Tips_ConnectServerFail));
                Debug.Log("连接服务器失败!");
            }
        }
        //发送
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Login");
        protocol.AddString(AccountText.text);
        protocol.AddString(PasswordText.text);
        Debug.Log("发送 " + protocol.GetDesc());
        NetMgr.srvConn.Send(protocol, OnLoginBack);
    }

    public void OnLoginBack(ProtocolBase protocol)
    {
        ProtocolBytes proto = (ProtocolBytes)protocol;
        int start = 0;
        string protoName = proto.GetString(start, ref start);
        int ret = proto.GetInt(start, ref start);
        if (ret == 0)
        {
            SinglePanelManger.Instance.PushTips(TextManager.Instance.GetString(TEXTS.Text_LoginSuccess));
            Debug.Log("登录成功");
            CloseUI();
            UIManager.Instance.OpenUI(EUIName.HomeUI);
        }
        else
        {
            SinglePanelManger.Instance.PushTips(TextManager.Instance.GetString(TEXTS.Text_LoginSuccess));
            Debug.Log("登录失败");
        }
    }

    private void OnClickRegisterButton(GameObject obj)
	{
        UIManager.Instance.OpenUI(EUIName.RegistUI);
		CloseUI();
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

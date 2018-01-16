using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegistUI : UIBase {


	public UIInputField AccountText = null;
	public UIInputField PasswordText = null;
	public UIInputField RepeatePassInputField = null;
	public UIButton BackBtn = null;
	public UIButton SureBtn = null;

	void Start () 
	{
		AccountText = Utility.GameUtility.FindDeepChild<UIInputField>(gameObject, "AccountInputField");
		PasswordText = Utility.GameUtility.FindDeepChild<UIInputField> (gameObject, "PassWordInputField");
		RepeatePassInputField = Utility.GameUtility.FindDeepChild<UIInputField> (gameObject, "RepeatPassWordInputField");
		AccountText.contentType = UnityEngine.UI.InputField.ContentType.Custom;
		PasswordText.contentType = UnityEngine.UI.InputField.ContentType.Password;
		RepeatePassInputField.contentType = UnityEngine.UI.InputField.ContentType.Password;
		BackBtn = Utility.GameUtility.FindDeepChild<UIButton> (gameObject, "BackBtn");
		AddClick (BackBtn.gameObject, OnClickBackBtn);
		SureBtn = Utility.GameUtility.FindDeepChild<UIButton> (gameObject, "SureButton");
		AddClick (SureBtn.gameObject, OnClickSureBtn);
	}

	private void OnClickBackBtn(GameObject obj)
	{
		
	}	

	private void OnClickSureBtn(GameObject obj)
	{
        //用户名密码为空
        if (AccountText.text == "" || PasswordText.text == "")
        {
            SinglePanelManger.Instance.PushTips(TextManager.Instance.GetString(TEXTS.Text_Tips_AcOrPwIsNull));
            Debug.Log("用户名密码不能为空!");
            return;
        }
        //两次密码不同
        if (PasswordText.text != RepeatePassInputField.text)
        {
            SinglePanelManger.Instance.PushTips(TextManager.Instance.GetString(TEXTS.Text_Tips_TwoPwIsUnEqual));
            Debug.Log("两次输入的密码不同！");
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
                SinglePanelManger.Instance.PushTips(TextManager.Instance.GetString(TEXTS.Text_Tips_AcOrPwIsNull));
                Debug.Log("连接服务器失败!");
            }
        }
        //发送
        ProtocolBytes protocol = new ProtocolBytes();
        protocol.AddString("Register");
        protocol.AddString(AccountText.text);
        protocol.AddString(PasswordText.text);
        Debug.Log("发送 " + protocol.GetDesc());
        NetMgr.srvConn.Send(protocol, OnRegisterBack);
    }
	public void OnRegisterBack(ProtocolBase protocol)
	{
		ProtocolBytes proto = (ProtocolBytes)protocol;
		int start = 0;
		string protoName = proto.GetString (start, ref start);
		int ret = proto.GetInt (start, ref start);
		if (ret == 0) {
			Debug.Log ("注册成功");
            UIManager.Instance.OpenUI(EUIName.ResigsterSucceesUI);
            ResigsterSucceesUI ui = (ResigsterSucceesUI)UIManager.Instance.GetUI(EUIName.ResigsterSucceesUI);
            ui.UpdateInfo("用户名：" + AccountText.text,"密码：" + PasswordText.text, "AAAAAAAAAAAAAAAAAAAAA");
			CloseUI ();
		} else {
            SinglePanelManger.Instance.PushTips(TextManager.Instance.GetString(TEXTS.Text_RegistFail));
			Debug.Log ("注册失败");
		}
	}
}

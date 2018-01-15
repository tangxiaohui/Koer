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
		AccountText.contentType = UnityEngine.UI.InputField.ContentType.IntegerNumber;
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
		if (AccountText.text == "" || PasswordText.text == "" || RepeatePassInputField.text == "")
		{
			Debug.Log ("用户名密码不能为空");
			return;
		}

		if (NetMgr.srvConn.status != Connection.Status.Connected) {
			Debug.Log ("网络未连接");
			return;
		}

		ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("Register");
		protocol.AddString (AccountText.text);
		protocol.AddString (PasswordText.text);
		Debug.Log ("发送 " + protocol.GetDesc ());
		NetMgr.srvConn.Send (protocol, OnRegisterBack);
	}
	public void OnRegisterBack(ProtocolBase protocol)
	{
		ProtocolBytes proto = (ProtocolBytes)protocol;
		int start = 0;
		string protoName = proto.GetString (start, ref start);
		int ret = proto.GetInt (start, ref start);
		if (ret == 0) {
			Debug.Log ("注册成功");
			CloseUI ();
		} else {
			Debug.Log ("注册失败");
		}
	}
}

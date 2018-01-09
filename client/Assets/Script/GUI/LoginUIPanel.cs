using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using KBEngine;

public class LoginUIPanel : GUIBase
{
	public LoginUIPanel(string prefabName, string uiName)
		: base(prefabName, uiName)
	{
	}

	protected override void OnStart()
	{
		base.OnStart();

		Debug.Log("login ui panel is open.");

		GObject loginBtn = GetChild("login");

		loginBtn.onClick.Add(OnLoginClick);

		if(PlayerPrefs.HasKey("chess_usen"))
		{
			GTextField useName = GetChild("use_name").asTextField;

			useName.text = PlayerPrefs.GetString("chess_usen");
		}

		if (PlayerPrefs.HasKey("chess_passw"))
		{
			GTextField passWord = GetChild("passward").asTextField;

			passWord.text = PlayerPrefs.GetString("chess_passw");
		}
	}

	private void OnLoginClick(EventContext ec)
	{
		GTextField useName = GetChild("use_name").asTextField;
		GTextField passWord = GetChild("passward").asTextField;

		string useNameValue = useName.text;
		string passWordValue = passWord.text;

		if(string.IsNullOrEmpty(useNameValue) || string.IsNullOrEmpty(passWordValue))
		{
			Debug.Log("帐号密码为空.");
			MessageUIPanel messagePanel = GUIManager.ShowOrLoadView<MessageUIPanel>("Command", "MessageUIPanel");
			messagePanel.ShowMessage("帐号密码为空");
			return;
		}

		Debug.Log(string.Format("entry game useName:{0}  passWord:{1}", useNameValue, passWordValue));

		UnityEngine.PlayerPrefs.SetString("chess_passw", passWordValue);
		UnityEngine.PlayerPrefs.SetString("chess_usen", useNameValue);

		KBEngine.Event.fireIn("login", useNameValue, passWordValue, System.Text.Encoding.UTF8.GetBytes("ChessSoldier"));
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class MessageUIPanel : GUIBase
{
	public MessageUIPanel(string prefabName, string uiName)
		: base(prefabName, uiName)
	{
	}

	protected override void OnStart()
	{
		base.OnStart();
	}

	public void ShowMessage(string message)
	{
		GTextField messageText = GetChild("n0").asTextField;
		Transition dongxiao = ViewComponent.GetTransition("message");

		messageText.text = message;

		dongxiao.Play();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FairyGUI;
using KBEngine;

public class ReNameUIPanel : GUIBase
{
	public ReNameUIPanel(string prefabName, string uiName)
		: base(prefabName, uiName)
	{
	}

	protected override void OnStart()
	{
		base.OnStart();

		GObject createBtn = GetChild("createname_btn");

		createBtn.onClick.Add(OnReNameClick);
	}

	private void OnReNameClick(EventContext content)
	{
		GTextField nameText = GetChild("name").asTextField;

		string nameValue = nameText.text;

		if(string.IsNullOrEmpty(nameValue))
		{
			return;
		}

		Account account = KBEngineApp.app.player() as Account;

		account.baseCall("ReName", nameValue);
	}
}

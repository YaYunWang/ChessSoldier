using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using KBEngine;

public class MainUIPanel : GUIBase
{
	public MainUIPanel(string prefabName, string uiName)
		: base(prefabName, uiName)
	{
	}

	protected override void OnStart()
	{
		base.OnStart();

		Account account = KBEngineApp.app.player() as Account;

		GTextField roleTypeText = GetChild("role_type").asTextField;
		GTextField nameText = GetChild("name").asTextField;

		roleTypeText.text = account.RoleType.ToString();
		nameText.text = account.RoleName;
	}
}

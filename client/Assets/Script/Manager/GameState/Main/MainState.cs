using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;

public class MainState : GameStateBase
{
	public MainState(GameStateManager mgr)
        : base(mgr)
    {
	}

	public override void Enter()
	{
		Dbg.DEBUG_MSG("enter main state.");

		Account account = KBEngineApp.app.player() as Account;

		if(account.RoleType <= 0)
		{
			GUIManager.ShowOrLoadView<ReNameUIPanel>("Login", "ReNameUIPanel");
		}
		else
		{
			GUIManager.ShowOrLoadView<MainUIPanel>("Main", "MainUIPanel");
		}
	}

	public void ChangeMainState()
	{
		GUIManager.DestroyView("ReNameUIPanel");

		GUIManager.ShowOrLoadView<MainUIPanel>("Main", "MainUIPanel");
	}

	public override void Leave()
	{
	}

	public override void Update()
	{
	}

	public override void LateUpdate()
	{

	}
}

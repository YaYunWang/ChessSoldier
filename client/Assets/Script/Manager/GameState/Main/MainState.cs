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

		GUIManager.ShowOrLoadView<ReNameUIPanel>("Login", "ReNameUIPanel");
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

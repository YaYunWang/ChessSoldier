using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;

public class BattleFieldState : GameStateBase
{
	public BattleFieldState(GameStateManager mgr)
        : base(mgr)
    {
	}

	public override void Enter()
	{
		SceneManager.ChangeScene(1, OnSceneLoadComplete);
	}

	private void OnSceneLoadComplete()
	{
		Debug.Log("entry battle field.");

		Account account = KBEngineApp.app.player() as Account;

		account.baseCall("reqHasEnteredBattlefiled");
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

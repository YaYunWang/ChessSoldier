using UnityEngine;
using System.Collections;
using KBEngine;

public class LoginState : GameStateBase
{
    public LoginState(GameStateManager mgr)
        : base(mgr)
    {
    }

    public override void Enter()
    {
		Debug.Log("entry login");
		//PanelManager.Instance.OpenPanel("LoginUIPanel");

		KBEngine.Event.registerOut("AccountCreate", this, "OnAccountCreate");

		GUIManager.ShowOrLoadView<LoginUIPanel>("Login", "LoginUIPanel");
	}

	public void OnAccountCreate(Account account)
	{
		GameStateManager.Instance.ChangeGameState(GameStateManager.GameState.Main);
	}

	public override void Leave()
    {
		GUIManager.DestroyAllView();
    }

    public override void Update()
    {
    }

    public override void LateUpdate()
    {

    }
}

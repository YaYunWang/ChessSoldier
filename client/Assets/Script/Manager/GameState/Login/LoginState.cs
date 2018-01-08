using UnityEngine;
using System.Collections;

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

		KBEngine.Event.registerOut("CreateName", this, "OnCreateName");
		KBEngine.Event.registerOut("AccountInit", this, "OnAccountInit");
	}

	public void OnCreateName()
	{
		//PanelManager.Instance.ClosePanel("LoginUIPanel");

		//PanelManager.Instance.OpenPanel("CreateNameUIPanel");
	}

	public void OnAccountInit(string name)
	{
		Debug.Log("创建名字完成：" + name);
		//PanelManager.Instance.ClosePanel("LoginUIPanel");

		//GameStateManager.Instance.ChangeGameState(GameStateManager.GameState.World);
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

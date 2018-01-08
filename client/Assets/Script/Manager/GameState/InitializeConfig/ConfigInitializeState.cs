using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ConfigInitializeState : GameStateBase
{
    public ConfigInitializeState(GameStateManager mgr)
        : base(mgr)
    {
    }

    public override void Enter()
    {
        ConfigManager.CreateInstance();

        GameStateManager.Instance.ChangeGameState(GameStateManager.GameState.Login);

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

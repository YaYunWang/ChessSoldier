using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameStateBase
{
    protected GameStateManager.GameState CurrGameState = GameStateManager.GameState.None;
    protected GameStateManager m_gameStateManager;

    public GameStateManager.GameState GetCurrGameState
    {
        get
        {
            return this.CurrGameState;
        }
    }
    public GameStateBase(GameStateManager mgr)
    {
        m_gameStateManager = mgr;
    }

    public abstract void Enter();

    public abstract void Leave();

    public abstract void Update();

	public abstract void LateUpdate();
}
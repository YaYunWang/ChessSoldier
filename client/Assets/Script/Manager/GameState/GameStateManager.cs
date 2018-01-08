using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStateManager : ManagerTemplate<GameStateManager>
{
	public enum GameState
	{
		None = -1,
		ConfigInitialize = 0,
		Login = 1,
		World = 2,
		Count
	}
	private GameStateBase[] m_states;
	private GameState m_activeState = GameState.None;
	private GameState m_preState = GameState.None;

    protected override void InitManager()
	{
		InitializeStates();
	}

	public void Update()
	{
		if (m_activeState != GameState.None)
		{
			if(m_states[(int)m_activeState] != null)
				m_states[(int)m_activeState].Update();
		}
	}

	public void LateUpdate()
	{
		if (m_activeState != GameState.None)
			m_states[(int)m_activeState].LateUpdate();
	}

	public void UnInitialize()
	{
		if (m_activeState != GameState.None)
		{
			m_states[(int)m_activeState].Leave();
		}

		for (int n = 0; n < (int)GameState.Count; ++n)
		{
			m_states[n] = null;
		}
		m_activeState = GameState.None;
		m_states = null;
	}

	private void InitializeStates()
	{
		m_states = new GameStateBase[(int)GameState.Count];
		m_states[(int)GameState.ConfigInitialize] = new ConfigInitializeState(this);
		m_states[(int)GameState.Login] = new LoginState(this);

		ChangeGameState(GameState.ConfigInitialize);
    }

	public GameState GetPreState()
	{
		return m_preState;
	}

	public GameState GetActiveState()
	{
		return m_activeState;
	}

    public void ChangeGameState(GameState state)
    {
        if (m_activeState != GameState.None)
        {
            m_states[(int)m_activeState].Leave();
        }

        m_preState = m_activeState;
        m_activeState = state;

        m_states[(int)m_activeState].Enter();
    }
}

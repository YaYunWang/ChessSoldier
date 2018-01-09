using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FairyGUI;

public class Main : KBEMain
{

	// Use this for initialization
	void Awake()
	{
		GameObject.DontDestroyOnLoad(gameObject);
#if SHOW_FPS
		gameObject.SafeAddComponent<FPSDisplay>();
#endif

		InitGameManager();
	}

	private void InitGameManager()
	{
		AssetBundleManager.CreateInstance();
		GUIManager.CreateInstance();

		GameEventManager.CreateInstance();
		AudioManager.CreateInstance();
		GameStateManager.CreateInstance();
	}

	// Update is called once per frame
	void Update()
	{

	}
}

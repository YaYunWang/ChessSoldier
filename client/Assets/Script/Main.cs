using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

	// Use this for initialization
	void Start()
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
		GameEventManager.CreateInstance();
		AudioManager.CreateInstance();
		GameStateManager.CreateInstance();
	}

	// Update is called once per frame
	void Update()
	{

	}
}

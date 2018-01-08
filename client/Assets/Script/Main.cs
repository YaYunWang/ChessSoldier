using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FairyGUI;

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

		AssetBundle commandab = AssetBundleManager.LoadAsset("command.bundle");
		UIPackage.AddPackage(commandab);

		StartCoroutine(loadpanel());

		//GObject loginBtn = loginView.GetChild("login");
		//loginBtn.onClick.Add(OnLoginClick);
	}

	private IEnumerator loadpanel()
	{
		yield return null;
		AssetBundle loginab = AssetBundleManager.LoadAsset("login.bundle");
		UIPackage.AddPackage(loginab);
		GComponent loginView = UIPackage.CreateObject("Login", "LoginUIPanel").asCom;
		loginView.fairyBatching = true;
		loginView.SetSize(GRoot.inst.width, GRoot.inst.height);
		loginView.AddRelation(GRoot.inst, RelationType.Size);
		GRoot.inst.AddChild(loginView);

		yield return new WaitForSeconds(1);

		AssetBundle loadingab = AssetBundleManager.LoadAsset("loading.bundle");
		UIPackage.AddPackage(loadingab);
		GComponent loadingView = UIPackage.CreateObject("Loading", "LoadingUIPanel").asCom;
		loadingView.fairyBatching = true;
		loadingView.SetSize(GRoot.inst.width, GRoot.inst.height);
		loadingView.AddRelation(GRoot.inst, RelationType.Size);
		GRoot.inst.AddChild(loadingView);
	}

	private void OnLoginClick(EventContext ec)
	{
		Debug.Log("login");
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

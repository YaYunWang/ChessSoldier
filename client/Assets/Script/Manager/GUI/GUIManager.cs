using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FairyGUI;

class GUIAssetBundle
{
	public string PackageName;
	public AssetBundle AB;

	private int reference = 0;

	public GUIAssetBundle(string packageName)
	{
		PackageName = packageName;
		AB = AssetBundleManager.LoadAsset(string.Format("ui/{0}.bundle", PackageName.ToLower())); ;

		reference = 0;
	}

	public void ReferenceCount()
	{
		if(reference == 0)
		{
			UIPackage.AddPackage(AB);
		}
		reference++;
	}

	public void IncReferenceCount()
	{
		--reference;

		if(reference <= 0)
		{
			UIPackage.RemovePackage(PackageName);
		}
	}

	public int GetReferenceCount()
	{
		return reference;
	}
}

public class GUIManager : ManagerTemplate<GUIManager>
{
	private static AssetBundle MainAssetBundle = null;

	private static Dictionary<string, GUIBase> uiViewDic = new Dictionary<string, GUIBase>();
	private static List<GUIBase> uiViewList = new List<GUIBase>();

	private static Dictionary<string, GUIAssetBundle> uiAssetDic = new Dictionary<string, GUIAssetBundle>();

	protected override void InitManager()
	{
		MainAssetBundle = AssetBundleManager.LoadAsset("ui/command.bundle");
		UIPackage.AddPackage(MainAssetBundle);
	}

	public static void ReferenceAssetBundle(string packageName)
	{
		if (packageName.Equals("Command"))
			return;

		if(uiAssetDic.ContainsKey(packageName))
		{
			uiAssetDic[packageName].ReferenceCount();
		}
		else
		{
			GUIAssetBundle guiAB = new GUIAssetBundle(packageName);
			guiAB.ReferenceCount();

			uiAssetDic.Add(packageName, guiAB);
		}
	}

	public static void IncReferenceAssetBundle(string packageName)
	{
		if (packageName.Equals("Command"))
			return;

		if (uiAssetDic.ContainsKey(packageName))
		{
			GUIAssetBundle guiAB = uiAssetDic[packageName];

			guiAB.IncReferenceCount();

			if(guiAB.GetReferenceCount() <= 0)
			{
				uiAssetDic.Remove(packageName);
			}
		}
	}

	private void Update()
	{
		for(int idx = 0; idx < uiViewList.Count; idx++)
		{
			GUIBase view = uiViewList[idx];

			UnityEngine.Profiling.Profiler.BeginSample("ui update ==" + view.UIName);
			view.OnUpdate(Time.deltaTime);
			UnityEngine.Profiling.Profiler.EndSample();
		}
	}

	public static GUIBase GetView(string uiName)
	{
		for (int n = 0; n < uiViewList.Count; ++n)
		{
			if (uiViewList[n].UIName == uiName)
				return uiViewList[n];
		}

		return null;
	}

	public static T GetView<T>(string uiName) where T : GUIBase
	{
		for (int n = 0; n < uiViewList.Count; ++n)
		{
			if (uiViewList[n].UIName == uiName)
				return uiViewList[n] as T;
		}

		return null;
	}

	public static bool IsViewExist(string uiName)
	{
		int count = uiViewList.Count;
		for (int i = 0; i < count; ++i)
		{
			if (uiViewList[i].UIName == uiName)
				return true;
		}

		return false;
	}

	public static T ShowOrLoadView<T>(string packageName, string uiName) where T : GUIBase
	{
		GUIBase gui = GetView<T>(uiName);
		if(null != gui)
		{
			return gui as T;
		}

		gui = CreateView<T>(packageName, uiName);

		return gui as T;
	}

	public static void DestroyView(string uiName)
	{
		for (int n = 0; n < uiViewList.Count; ++n)
		{
			if (uiViewList[n].UIName == uiName)
			{
				GUIBase view = uiViewList[n];
				if (uiViewDic.ContainsKey(uiName))
				{
					uiViewDic.Remove(uiName);
				}
				if (uiViewList.Contains(view))
				{
					uiViewList.Remove(view);
				}

				view.OnDestory();
				view = null;
				break;
			}
		}
	}

	public static void DestroyAllView()
	{
		foreach (KeyValuePair<string, GUIBase> pair in uiViewDic)
		{
			pair.Value.OnDestory();
		}
		uiViewDic.Clear();
		uiViewList.Clear();
	}

	private static T CreateView<T>(string packageName, string uiName) where T : GUIBase
	{
		GUIBase view = System.Activator.CreateInstance(typeof(T), new System.Object[2] { packageName, uiName }) as GUIBase;
		if (view == null)
		{
			view = new GUIBase(packageName, uiName);
		}

		view.EngineBuild();

		uiViewList.Add(view);
		uiViewDic.Add(uiName, view);

		return view as T;
	}
}

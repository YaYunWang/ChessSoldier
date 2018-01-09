using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using FairyGUI;

public class GUIBase
{
	public string UIName = string.Empty;
	public string PackageName = string.Empty;

	protected GComponent ViewComponent = null;

	public GUIBase(string packageName, string uiName)
	{
		PackageName = packageName;
		UIName = uiName;
	}

	protected virtual void OnStart()
	{
	}

	public virtual void OnUpdate(float dt)
	{
	}

	public virtual void OnDestory()
	{
	}

	public void EngineBuild()
	{
		GUIManager.ReferenceAssetBundle(PackageName);

		ViewComponent = UIPackage.CreateObject(PackageName, UIName).asCom;

		ViewComponent.fairyBatching = true;
		ViewComponent.SetSize(GRoot.inst.width, GRoot.inst.height);
		ViewComponent.AddRelation(GRoot.inst, RelationType.Size);
		GRoot.inst.AddChild(ViewComponent);

		OnStart();
	}

	protected GObject GetChild(string name)
	{
		if (ViewComponent == null)
			return null;

		return ViewComponent.GetChild(name);
	}
}

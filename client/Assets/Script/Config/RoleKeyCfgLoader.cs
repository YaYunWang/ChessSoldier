using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoleKeyCfgLoader : ConfigLoaderBase
{
	private Dictionary<int, RoleKeyCfg> m_data = new Dictionary<int, RoleKeyCfg>();

	protected override void OnLoad()
	{
		if (!ReadConfig<RoleKeyCfg>("Xml/RoleKeyCfg.xml", OnReadRow))
			return;
	}

	protected override void OnUnload()
	{
	}

	private void OnReadRow(RoleKeyCfg row)
	{
		m_data[row.ID] = row;
	}

	public RoleKeyCfg GetConfig(int id)
	{
		RoleKeyCfg config;
		m_data.TryGetValue(id, out config);
		return config;
	}
}

public class RoleKeyCfg
{
	public int ID;
	public int Sex;
	public string BornBody;
	public string ShowBone;
	public string ShowBody;
	public string ShowWeapon;
	public string Controller;
}

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class NpcCfg
{
    public int ID;
    public string Name;
    public string ModelName;
    public string Controller;
}

public class NpcCfgLoader : ConfigLoaderBase {

    private Dictionary<int, NpcCfg> m_data = new Dictionary<int, NpcCfg>();

    protected override void OnLoad()
    {
        if (!ReadConfig<NpcCfg>("Exported/NpcCfg.xml", OnReadRow))
            return;
    }

    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(NpcCfg row)
    {
        m_data[row.ID] = row;
    }

    public NpcCfg GetConfig(int key)
    {
        NpcCfg config = null;
        m_data.TryGetValue(key, out config);
        return config;
    }

    public IEnumerable<int> GetKeys()
    {
        return m_data.Keys;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneCfgLoader : ConfigLoaderBase
{
    private Dictionary<int, SceneCfg> m_data = new Dictionary<int, SceneCfg>();

    protected override void OnLoad()
    {
        if (!ReadConfig<SceneCfg>("Xml/SceneCfg.xml", OnReadRow))
            return;
    }

    protected override void OnUnload()
    {
    }

    private void OnReadRow(SceneCfg row)
    {
        m_data[row.ID] = row;
    }

    public SceneCfg GetConfig(int id)
    {
        SceneCfg config;
        m_data.TryGetValue(id, out config);
        return config;
    }

    public string GetSceneAssetName(int id)
    {
        SceneCfg cfg = GetConfig(id);
        if (cfg == null)
            return "";
        return cfg.AssetName;
    }
}
public class SceneCfg
{
    public int ID;
    public string AssetName;
}
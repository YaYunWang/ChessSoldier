using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class AudioCfg
{
    public int ID;
    public string AssetName;
    public bool Loop;
    public float Volume;
    public int Priority;
    public int FadeIn;
    public int FadeOut;
}

public class AudioCfgLoader : ConfigLoaderBase
{


    private Dictionary<int, AudioCfg> m_data = new Dictionary<int, AudioCfg>();

    protected override void OnLoad()
    {
        if (!ReadConfig<AudioCfg>("Xml/AudioCfg.xml", OnReadRow))
            return;
    }

    protected override void OnUnload()
    {
    }

    private void OnReadRow(AudioCfg row)
    {
        m_data[row.ID] = row;
    }

    public AudioCfg GetConfig(int id)
    {
        AudioCfg config;
        m_data.TryGetValue(id, out config);
        return config;
    }
}

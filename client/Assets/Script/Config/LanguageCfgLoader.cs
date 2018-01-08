using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LanguageCfgLoader : ConfigLoaderBase
{
    private Dictionary<int, LanguageCfg> m_data = new Dictionary<int, LanguageCfg>();
    protected override void OnLoad()
    {
        //if (!ReadConfig<LanguageCfg>("Xml/LanguageCfg.xml", OnReadRow))
        //    return;
    }
    protected override void OnUnload()
    {
    }

    private void OnReadRow(LanguageCfg row)
    {
        m_data[row.ID] = row;
    }

    public string GetLanguage(int id)
    {
        return m_data.ContainsKey(id) ? m_data[id].China : id.ToString();
    }
}
public class LanguageCfg
{
    public int ID;
    public string China;
    public string Traditional;
    public string Engilsh;
}
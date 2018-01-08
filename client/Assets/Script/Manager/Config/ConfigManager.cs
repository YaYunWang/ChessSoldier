using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigManager : ManagerTemplate<ConfigManager>
{
    private static Dictionary<System.Type, ConfigLoaderBase> m_configLoaders = new Dictionary<System.Type, ConfigLoaderBase>();
    public static T Get<T>() where T : ConfigLoaderBase
    {
        ConfigLoaderBase loader = null;
        if (!m_configLoaders.TryGetValue(typeof(T), out loader))
            return null;

        return loader as T;
    }

    protected override void InitManager()
    {
        var types = System.Reflection.Assembly.GetCallingAssembly().GetTypes();

        for (int i = 0; i < types.Length; i++)
        {
            var type = types[i];
            if (type.IsSubclassOf(typeof(ConfigLoaderBase)))
            {
                var loader = System.Activator.CreateInstance(type) as ConfigLoaderBase;
                if (loader == null)
                    continue;

                m_configLoaders.Add(types[i], loader);
            }
        }

        foreach (var loader in m_configLoaders)
        {
            loader.Value.Load();
        }
    }
}
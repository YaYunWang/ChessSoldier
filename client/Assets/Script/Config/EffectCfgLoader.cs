using UnityEngine;
using System.Collections.Generic;
using System;

public enum EffectPriority
{
    Low = 0,        // 常驻低优先级特效，达到限制时会被顶替
    Meidum = 1,     // 参与同屏限制，会顶替Low
    High = 2,       // High不参与同屏限制，不计入特效总数
}

public class EffectCfg
{
    // 配置表数据
    public int ID;
    public string OnBeginPlay;
    public string OnEndPlay;
    public string AssetName;
    public string BindPoint;
    public int Delay;
    public int Lifetime;
    public bool FollowPosition;
    public bool FollowRotation;
    public bool FollowScale;
    public string LocalPosition;
    public string LocalRotation;
    public string LocalScale;
    public int FadeOutTime;
    public int Audio;
    public int Priority;

    // 运行时数据
    public int[] OnBeginPlayArray;
    public int[] OnEndPlayArray;
    public Vector3 LocalPositionVec3;
    public Vector3 LocalRotationVec3;
    public Quaternion LocalRotationQuaternion;
    public Vector3 LocalScaleVec3;
}


public class EffectCfgLoader : ConfigLoaderBase
{
    private Dictionary<int, EffectCfg> m_data = new Dictionary<int, EffectCfg>();

    protected override void OnLoad()
    {
        //if (!ReadConfig<EffectCfg>("Xml/EffectCfg.xml", OnReadRow))
        //    return;
    }

    protected override void OnUnload()
    {
        m_data.Clear();
    }

    private void OnReadRow(EffectCfg row)
    {
        InitRuntimeData(row);
        m_data[row.ID] = row;
    }

    private void InitRuntimeData(EffectCfg row)
    {
        row.OnBeginPlayArray = ConfigParseUtil.ParseIntArray(row.OnBeginPlay);
        row.OnEndPlayArray = ConfigParseUtil.ParseIntArray(row.OnEndPlay);
        row.LocalPositionVec3 = ConfigParseUtil.ParseVec3(row.LocalPosition);
        row.LocalRotationVec3 = ConfigParseUtil.ParseVec3(row.LocalRotation);
        row.LocalRotationQuaternion = Quaternion.Euler(row.LocalRotationVec3);

        row.LocalScaleVec3 = ConfigParseUtil.ParseVec3(row.LocalScale);
        if (row.LocalScaleVec3 == Vector3.zero)
            row.LocalScaleVec3 = Vector3.one;
    }

    public EffectCfg GetConfig(int id)
    {
        EffectCfg config;
        m_data.TryGetValue(id, out config);
        return config;
    }
}

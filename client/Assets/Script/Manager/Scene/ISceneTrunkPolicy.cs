using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ISceneTrunkPolicy
{
    void Setup(SceneTrunkMgr sceneTrunkmgr);
    void CalcSignificantIndicies(int index, Vector3 trunkLocalPos, ref List<int> output);
    bool CanUnloadInsignificant(Vector3 trunkLocalPos);

}

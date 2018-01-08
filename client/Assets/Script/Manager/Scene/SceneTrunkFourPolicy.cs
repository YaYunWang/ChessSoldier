using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneTrunkFourPolicy : ISceneTrunkPolicy
{
    public float preloadThreshold = 10;
    SceneTrunkMgr sceneTrunkMgr;

    public void Setup(SceneTrunkMgr sceneTrunkmgr)
    {
        this.sceneTrunkMgr = sceneTrunkmgr;
    }

    public void CalcSignificantIndicies(int index, Vector3 trunkLocalPos, ref List<int> output)
    {
        float halfSize = sceneTrunkMgr.trunkSize * .5f;
        int i = -1;

        bool left = false;

        if (trunkLocalPos.x >= halfSize)
        {
            left = false;
            i = sceneTrunkMgr.GetRightTrunkIndex(index);
            if (i != -1)
                output.Add(i);
        }
        else
        {
            left = true;
            i = sceneTrunkMgr.GetLeftTrunkIndex(index);
            if (i != -1)
                output.Add(i);
        }

        if (trunkLocalPos.z >= halfSize)
        {
            i = sceneTrunkMgr.GetTopTrunkIndex(index);
            if (i != -1)
            {
                output.Add(i);

                if (left)
                {
                    i = sceneTrunkMgr.GetLeftTrunkIndex(i);
                    if (i != -1)
                        output.Add(i);
                }
                else
                {
                    i = sceneTrunkMgr.GetRightTrunkIndex(i);
                    if (i != -1)
                        output.Add(i);
                }
            }
        }
        else
        {
            i = sceneTrunkMgr.GetBottomTrunkIndex(index);
            if (i != -1)
            {
                output.Add(i);

                if (left)
                {
                    i = sceneTrunkMgr.GetLeftTrunkIndex(i);
                    if (i != -1)
                        output.Add(i);
                }
                else
                {
                    i = sceneTrunkMgr.GetRightTrunkIndex(i);
                    if (i != -1)
                        output.Add(i);
                }
            }

        }
    }

    public bool CanUnloadInsignificant(Vector3 trunkLocalPos)
    {
        float halfSize = sceneTrunkMgr.trunkSize * .5f;
        bool xNotInThreshold = false;
        bool zNotInThreshold = false;

        if (trunkLocalPos.x < halfSize - preloadThreshold || trunkLocalPos.x > halfSize + preloadThreshold)
        {
            xNotInThreshold = true;
        }

        if (trunkLocalPos.z < halfSize - preloadThreshold || trunkLocalPos.z > halfSize + preloadThreshold)
        {
            zNotInThreshold = true;
        }

        return xNotInThreshold && zNotInThreshold;
    }
}

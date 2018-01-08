using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneTrunkNinePolicy : ISceneTrunkPolicy
{
    public float preloadThreshold = 20;
    public float unloadThreshold = 40;
    SceneTrunkMgr sceneTrunkMgr;

    public void Setup(SceneTrunkMgr sceneTrunkmgr)
    {
        this.sceneTrunkMgr = sceneTrunkmgr;
    }

    public void CalcSignificantIndicies(int index, Vector3 trunkLocalPos, ref List<int> output)
    {
        int top = sceneTrunkMgr.GetTopTrunkIndex(index);
        if (top != -1)
            output.Add(top);
        int right = sceneTrunkMgr.GetRightTrunkIndex(index);
        if (right != -1)
            output.Add(right);
        int bottom = sceneTrunkMgr.GetBottomTrunkIndex(index);
        if (bottom != -1)
            output.Add(bottom);
        int left = sceneTrunkMgr.GetLeftTrunkIndex(index);
        if (left != -1)
            output.Add(left);

        int i = -1;

        if (left != -1)
        {
            i = sceneTrunkMgr.GetTopTrunkIndex(left);
            if (i != -1)
                output.Add(i);

            i = sceneTrunkMgr.GetBottomTrunkIndex(left);
            if (i != -1)
                output.Add(i);
        }

        if (right != -1)
        {
            i = sceneTrunkMgr.GetTopTrunkIndex(right);
            if (i != -1)
                output.Add(i);

            i = sceneTrunkMgr.GetBottomTrunkIndex(right);
            if (i != -1)
                output.Add(i);
        }

        if (trunkLocalPos.x < preloadThreshold)
        {
            if (left != -1)
            {
                i = sceneTrunkMgr.GetLeftTrunkIndex(left);
                if (i != -1)
                {
                    output.Add(i);
                }
            }
        }
        else if (trunkLocalPos.x > sceneTrunkMgr.trunkSize - preloadThreshold)
        {
            if (right != -1)
            {
                i = sceneTrunkMgr.GetRightTrunkIndex(right);
                if (i != -1)
                {
                    output.Add(i);
                }
            }
        }

        if (trunkLocalPos.z < preloadThreshold)
        {
            if (bottom != -1)
            {
                i = sceneTrunkMgr.GetBottomTrunkIndex(bottom);
                if (i != -1)
                {
                    output.Add(i);
                }
            }
        }
        else if (trunkLocalPos.z > sceneTrunkMgr.trunkSize - preloadThreshold)
        {
            if (top != -1)
            {
                i = sceneTrunkMgr.GetTopTrunkIndex(top);
                if (i != -1)
                {
                    output.Add(i);
                }
            }
        }
    }

    public bool CanUnloadInsignificant(Vector3 trunkLocalPos)
    {
        bool xNotInThreshold = false;
        bool zNotInThreshold = false;

        if (trunkLocalPos.x > unloadThreshold && trunkLocalPos.x < sceneTrunkMgr.trunkSize - unloadThreshold)
        {
            xNotInThreshold = true;
        }

        if (trunkLocalPos.z > unloadThreshold && trunkLocalPos.z < sceneTrunkMgr.trunkSize - unloadThreshold)
        {
            zNotInThreshold = true;
        }

        return xNotInThreshold && zNotInThreshold;
    }
}

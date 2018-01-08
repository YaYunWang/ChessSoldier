using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialObject : MonoBehaviour
{
    bool mInstancing = false;
    Material mMat;
    MaterialPropertyBlock mMatPropBlock;
    Renderer mRenderer;

    public void Awake()
    {
        mRenderer = GetComponent<Renderer>();

        var sharedMat = mRenderer.sharedMaterial;
        mInstancing = sharedMat.enableInstancing;

        if (SystemInfo.supportsInstancing && mInstancing)
        {
            mInstancing = true;
            mMatPropBlock = new MaterialPropertyBlock();
            mRenderer.GetPropertyBlock(mMatPropBlock);
        }
        else
        {
            mInstancing = false;
            mMat = mRenderer.material;
        }
    }

    public Color GetColor(int propName)
    {
        if (mInstancing)
        {
            if (mMatPropBlock == null)
                return Color.black;

            return mMatPropBlock.GetVector(propName);
        }
        else
        {
            if (mMat == null)
                return Color.black;

            return mMat.GetColor(propName);
        }
    }

    public void SetColor(int propName, Color color)
    {
        if (mInstancing)
        {
            if (mMatPropBlock != null)
                mMatPropBlock.SetVector(propName, color);
        }
        else
        {
            if (mMat != null)
                mMat.SetColor(propName, color);
        }
    }

    public Vector4 GetVector(int propName)
    {
        if (mInstancing)
        {
            if (mMatPropBlock == null)
                return Vector4.zero;

            return mMatPropBlock.GetVector(propName);
        }
        else
        {
            if (mMat == null)
                return Vector4.zero;

            return mMat.GetVector(propName);
        }
    }

    public void SetVector(int propName, Color color)
    {
        if (mInstancing)
        {
            if (mMatPropBlock != null)
                mMatPropBlock.SetVector(propName, color);
        }
        else
        {
            if (mMat != null)
                mMat.SetVector(propName, color);
        }
    }

    public float GetFloat(int propName)
    {
        if (mInstancing)
        {
            if (mMatPropBlock == null)
                return 0;

            return mMatPropBlock.GetFloat(propName);
        }
        else
        {
            if (mMat == null)
                return 0;

            return mMat.GetFloat(propName);
        }
    }

    public void SetFloat(int propName, float val)
    {
        if (mInstancing)
        {
            if (mMatPropBlock != null)
                mMatPropBlock.SetFloat(propName, val);
        }
        else
        {
            if (mMat != null)
                mMat.SetFloat(propName, val);
        }
    }

    public void Flush()
    {
        if (mInstancing && mMatPropBlock != null)
        {
            mRenderer.SetPropertyBlock(mMatPropBlock);
        }
    }
}
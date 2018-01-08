using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSheetAnimation : MonoBehaviour
{
    // 延迟播放（单位秒）
    public float delay = 0;
    // 横向平铺个数
    public int xTiles = 1;
    // 纵向平铺个数
    public int yTiles = 1;
    // 总帧数，-1为默认
    public int frameCount = -1;
    // 帧率，每秒播放的帧数
    public int frameRate = 30;
    // 循环次数
    public int cycles = 1;

	public string texturePropName = "_MainTex";

    private int frameIndex = 0;
    private int playCount = 0;
    private float elapsed = 0;

    private MaterialObject material;

    private int texturePropID;
    private int realFrameCount = -1;
    private float timePerFrame = 0;
    private bool isPlaying = false;
    private bool needDelay = false;

    Vector4 textureST;

    private void Awake()
    {
        material = gameObject.SafeAddComponent<MaterialObject>();

        if (frameCount <= 0)
        {
            realFrameCount = xTiles * yTiles;
        }
        else
        {
            realFrameCount = frameCount;
        }

        timePerFrame = (1.0f / frameRate);

        string textureSTPropName = texturePropName + "_ST";
        texturePropID = Shader.PropertyToID(textureSTPropName);
        textureST = material.GetVector(texturePropID);
		textureST.x = 1.0f / xTiles;
		textureST.y = 1.0f / yTiles;
    }

    private void OnEnable()
    {
        frameIndex = 0;
        playCount = 0;
        elapsed = 0;
        isPlaying = true;
        needDelay = delay > 0;

        RefreshMaterial();
    }

    private void RefreshMaterial()
    {
        int row = (int)(frameIndex / xTiles);
        int col = frameIndex % xTiles;

        textureST.z = (float)col / xTiles;
        textureST.w = (float)(yTiles - 1 - row) / yTiles;
        material.SetVector(texturePropID, textureST);
		material.Flush ();
    }

    private void Update()
    {
        if (!isPlaying) return;

        elapsed += Time.deltaTime;

        if (needDelay)
        {
            if (elapsed < delay)
            {
                return;
            }

            needDelay = false;
            elapsed -= delay;
        }

        int nextFrameIndex = Mathf.FloorToInt((elapsed - realFrameCount * timePerFrame * playCount) / timePerFrame);

        if (nextFrameIndex >= realFrameCount)
        {
            playCount++;

			if (cycles >= 0 && playCount >= cycles)
            {
                isPlaying = false;
                nextFrameIndex = realFrameCount - 1;
            }
            else
            {
                nextFrameIndex = nextFrameIndex % realFrameCount;
            }
        }

        if (frameIndex != nextFrameIndex)
        {
            frameIndex = nextFrameIndex;
            RefreshMaterial();
        }
    }
}

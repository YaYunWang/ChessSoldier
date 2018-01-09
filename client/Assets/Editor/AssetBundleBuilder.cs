using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public static class AssetBundleBuilder
{
    [MenuItem("Assets/AssetBundles/Build AssetBundles")]
    public static void BuildAssetBundles()
    {
		string platformName = ResourceUtil.GetPlatformName();

		string outputPath = Path.Combine(ResourceUtil.AssetBundlesOutputPath, platformName);
		if("Windows" == platformName)
		{
			outputPath = Path.Combine("Assets/StreamingAssets/", platformName);
		}

		if (!Directory.Exists(outputPath))
			Directory.CreateDirectory(outputPath);

		BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }
}

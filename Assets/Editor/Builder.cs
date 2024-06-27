using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Builder : Editor
{
    [MenuItem("AssetBundles/Build")]
    public static void Build()
    {
        BuildPipeline.BuildAssetBundles("Assets/Bundles/Output/Windows", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
        BuildPipeline.BuildAssetBundles("Assets/Bundles/Output/Android", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
    }
}
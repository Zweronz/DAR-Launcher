using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Builder : Editor
{
    [MenuItem("AssetBundles/Build")]
    public static void Build()
    {
        BuildPipeline.BuildAssetBundles("BundleOutput/Windows", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        BuildPipeline.BuildAssetBundles("BundleOutput/Android", BuildAssetBundleOptions.None, BuildTarget.Android);

        File.Delete(Directory.GetParent(Application.dataPath) + "/BundleOutput/Windows/Windows");
        File.Delete(Directory.GetParent(Application.dataPath) + "/BundleOutput/Windows/Windows.manifest");

        File.Delete(Directory.GetParent(Application.dataPath) + "/BundleOutput/Android/Android");
        File.Delete(Directory.GetParent(Application.dataPath) + "/BundleOutput/Android/Android.manifest");
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Bundles
{
    public const string BundlePath = "Bundles";

#if UNITY_STANDALONE_WIN
    public const string BundlePostfix = "Windows";
#elif UNITY_ANDROID
    public const string BundlePostfix = "Android";
#endif

    public const string BGMPath = "BGM";

    private static Dictionary<string, AssetBundle> cachedBundles = new Dictionary<string, AssetBundle>();


    private static AssetBundle Load(string game)
    {
        return AssetBundle.LoadFromFile(Path.Combine(GithubController.DownloadedDataPath, BundlePath, BundlePostfix, game).Replace("\\", "/"));
    }

    public static T Load<T>(string game, string path) where T : Object
    {
        if (!cachedBundles.ContainsKey(game))
        {
            cachedBundles.Add(game, Load(game));
        }

        return cachedBundles[game].LoadAsset<T>(path);
    }

    public static AudioClip LoadBGM(string game)
    {
        return Load<AudioClip>(game, BGMPath);
    }
}

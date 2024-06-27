using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class GithubController
{
    public const string LauncherDataRepository = "DAR-Launcher-Data";

    public const string LauncherDataMain = "DAR-Launcher-Data-main";

    public const string Author = "Zweronz";

    public const string APIURL = "https://api.github.com/repos/";

    public const string APIPostfix = "releases/latest";

    public const string DefaultURL = "https://github.com";

    public const string ZipPostfix = "archive/refs/heads/main.zip";

    private static string DataPath
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, "LauncherData").Replace("\\", "/");
        }
    }

    public static string DownloadedDataPath
    {
        get
        {
            return Path.Combine(DataPath, LauncherDataMain).Replace("\\", "/");
        }
    }

    private static string DataZipPath
    {
        get
        {
            return Path.Combine(DefaultURL, Author, LauncherDataRepository, ZipPostfix).Replace("\\", "/");
        }
    }

    private static string LauncherDataURL => GetPath(LauncherDataRepository);

    private static bool downloading;

    private static JObject LauncherData;

    private static Dictionary<string, JObject> CachedData = new Dictionary<string, JObject>();

    private static string GetPath(string repository)
    {
        return Path.Combine(APIURL, Author, repository, APIPostfix).Replace("\\", "/");
    }

    public static void RedownloadData(Action onFinish)
    {
        if (downloading)
        {
            return;
        }

        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }

        //Routiner.Coroutine
        //(
            //RefreshLauncherData
            //(()=>
            //    {
                    //if (!PlayerPrefs.HasKey("Launcher ID") || PlayerPrefs.GetString("Launcher ID") != ((long)LauncherData["id"]).ToString())
                    //{
                        Routiner.Coroutine(Download(DataZipPath, DataPath, onFinish));
                    //}
                //}
            //)
        //);
    }

    private static IEnumerator Download(string url, string outputPath, Action onFinish)
    {
        downloading = true;

        UnityWebRequest downloadRequest = new UnityWebRequest(url)
        {
            downloadHandler = new DownloadHandlerBuffer()
        };

        yield return downloadRequest.SendWebRequest();

        if (downloadRequest.result == UnityWebRequest.Result.ConnectionError || downloadRequest.result == UnityWebRequest.Result.DataProcessingError || downloadRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(downloadRequest.error);
        }
        else
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(downloadRequest.downloadHandler.data);
                new FastZip().ExtractZip(stream, outputPath, FastZip.Overwrite.Always, null, null, null, false, true);

                onFinish();
            }
        }

        downloading = false;
    }

    private static IEnumerator RefreshLauncherData(Action onFinish)
    {
        Debug.LogError(LauncherDataURL);
        UnityWebRequest downloadRequest = new UnityWebRequest(LauncherDataURL);
        yield return downloadRequest.SendWebRequest();

        if (downloadRequest.result == UnityWebRequest.Result.ConnectionError || downloadRequest.result == UnityWebRequest.Result.DataProcessingError || downloadRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(downloadRequest.error);
        }
        else
        {
            Debug.LogError(downloadRequest.downloadHandler);
            LauncherData = JObject.Parse(downloadRequest.downloadHandler.text);
            onFinish();
        }
    }

    private static IEnumerator Refresh(string repo, Action onFinish)
    {
        UnityWebRequest downloadRequest = new UnityWebRequest(LauncherDataURL);
        yield return downloadRequest.SendWebRequest();

        if (downloadRequest.result == UnityWebRequest.Result.ConnectionError || downloadRequest.result == UnityWebRequest.Result.DataProcessingError || downloadRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(downloadRequest.error);
        }
        else
        {
            if (!CachedData.ContainsKey(repo))
            {
                CachedData.Add(repo, JObject.Parse(downloadRequest.downloadHandler.text));
            }
            else
            {
                CachedData[repo] = JObject.Parse(downloadRequest.downloadHandler.text);
            }

            onFinish();
        }
    }
}
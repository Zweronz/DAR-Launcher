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
using System.Linq;

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

    private static string GamePath
    {
        get
        {
            string path = Path.Combine(Application.persistentDataPath, "Games").Replace("\\", "/");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
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

    public static GameStatus currentStatus;

    public static string currentGamePath, currentExecutablePath;

    public static float downloadProgress;

    public enum GameStatus
    {
        NotDownloaded,
        UpdateNeeded,
        Unsynced,
        Downloaded
    }

    private static string GetPath(string repository)
    {
        return Path.Combine(APIURL, Author, repository, APIPostfix).Replace("\\", "/");
    }

    private static string GetGamePath(string game)
    {
        string path = Path.Combine(GamePath, game).Replace("\\", "/");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        return Path.Combine(GamePath, game).Replace("\\", "/");
    }

    public static void RedownloadData(Action onFinish)
    {
        if (downloading)
        {
            return;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            onFinish();
            return;
        }

        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }

        Routiner.Coroutine
        (
            RefreshLauncherData
            (()=>
                {
                    if (!Directory.Exists(DownloadedDataPath) || !PlayerPrefs.HasKey("Launcher ID") || PlayerPrefs.GetString("Launcher ID") != ((long)LauncherData["id"]).ToString())
                    {
                        Routiner.Coroutine(Download(DataZipPath, DataPath, onFinish));
                        PlayerPrefs.SetString("Launcher ID", ((long)LauncherData["id"]).ToString());
                    }
                    else if (Directory.Exists(DownloadedDataPath))
                    {
                        onFinish();
                    }
                }
            )
        );
    }

    public static void RefreshGame(string repo, Action onFinish)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            onFinish();
            return;
        }

        Routiner.Coroutine
        (
            Refresh
            (repo, ()=>
                {
                    foreach (JToken token in (JArray)CachedData[repo]["assets"])
                    {
                        string tokenString = (string)token["name"];

                        if (tokenString.EndsWith(".zip") && !tokenString.Contains("Debug"))
                        {
                            currentGamePath = (string)token["browser_download_url"];
                        }
                    }

                    string id = ((long)CachedData[repo]["id"]).ToString();
                    
                    string path = GetGamePath(Launcher.gameList.entries.Find(x => x.GetValue("repo") == repo).id);
                    string[] paths = Directory.GetDirectories(path);

                    if (paths.Length > 0)
                    {
                        path = Directory.GetDirectories(path).First();

                        List<string> executables = Directory.GetFiles(path).ToList().FindAll(x => x.EndsWith(".exe"));

                        if (Directory.Exists(path) && executables.Count != 0)
                        {
                            currentStatus = PlayerPrefs.GetString("Current ID_" + repo) == id ? GameStatus.Downloaded : GameStatus.UpdateNeeded;
                            currentExecutablePath = executables.First();
                        }
                        else
                        {
                            currentStatus = GameStatus.NotDownloaded;
                        }
                    }
                    else
                    {
                        currentStatus = GameStatus.NotDownloaded;
                    }

                    PlayerPrefs.SetString("Current ID_" + repo, id);

                    onFinish();
                }
            )
        );
    }

    public static void DownloadGame(string repo, Action onFinish)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            onFinish();
            return;
        }

        string path = GetGamePath(Launcher.gameList.entries.Find(x => x.GetValue("repo") == repo).GetValue("name"));
        string[] paths = Directory.GetDirectories(path);

        if (paths.Length > 0)
        {
            new DirectoryInfo(paths.First()).Delete();
        }

        Routiner.Coroutine
        (
            Download(currentGamePath, GetGamePath(Launcher.gameList.entries.Find(x => x.GetValue("repo") == repo).id), onFinish)
        );
    }

    private static IEnumerator Download(string url, string outputPath, Action onFinish)
    {
        downloading = true;

        UnityWebRequest downloadRequest = new UnityWebRequest(url)
        {
            downloadHandler = new DownloadHandlerBuffer()
        };

        downloadRequest.SendWebRequest();

        while (!downloadRequest.isDone)
        {
            downloadProgress = downloadRequest.downloadProgress;
            yield return null;
        }

        if (downloadRequest.result == UnityWebRequest.Result.ConnectionError || downloadRequest.result == UnityWebRequest.Result.DataProcessingError || downloadRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(downloadRequest.error);
        }
        else
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (Directory.Exists(DownloadedDataPath))
                {
                    new DirectoryInfo(DownloadedDataPath).Delete(true);
                }

                stream.Write(downloadRequest.downloadHandler.data);
                new FastZip().ExtractZip(stream, outputPath, FastZip.Overwrite.Always, null, null, null, false, true);

                onFinish();
            }
        }

        downloading = false;
    }

    private static IEnumerator RefreshLauncherData(Action onFinish)
    {
        UnityWebRequest downloadRequest = new UnityWebRequest(LauncherDataURL)
        {
            downloadHandler = new DownloadHandlerBuffer()
        };

        downloadRequest.SendWebRequest();

        while (!downloadRequest.isDone)
        {
            downloadProgress = downloadRequest.downloadProgress;
            yield return null;
        }

        if (downloadRequest.result == UnityWebRequest.Result.ConnectionError || downloadRequest.result == UnityWebRequest.Result.DataProcessingError || downloadRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(downloadRequest.error);
        }
        else
        {
            LauncherData = JObject.Parse(downloadRequest.downloadHandler.text);
            onFinish();
        }
    }

    private static IEnumerator Refresh(string repo, Action onFinish)
    {
        UnityWebRequest downloadRequest = new UnityWebRequest(GetPath(repo))
        {
            downloadHandler = new DownloadHandlerBuffer()
        };

        downloadRequest.SendWebRequest();

        while (!downloadRequest.isDone)
        {
            downloadProgress = downloadRequest.downloadProgress;
            yield return null;
        }

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
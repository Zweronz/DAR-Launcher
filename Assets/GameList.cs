using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameList
{
    public List<GameListEntry> entries = new List<GameListEntry>();

    public GameList(List<GameListEntry> entries)
    {
        this.entries = entries;
    }

    public GameListEntry GetEntry(string id)
    {
        return entries.Find(x => x.id == id);
    }
}

public class GameListEntry
{
    public string id;

    private Dictionary<string, string> values;

    public GameListEntry(string id, Dictionary<string, string> values)
    {
        this.id = id;
        this.values = values;
    }

    public string GetValue(string key)
    {
        if (!values.ContainsKey(key))
        {
            return "";
        }

        return values[key];
    }
}

public static class GameListParser
{
    public const string GameListPath = "Game List.txt";

    private static string FullGameListPath
    {
        get
        {
            return Path.Combine(GithubController.DownloadedDataPath, GameListPath).Replace("\\", "/");
        }
    }

    public static GameList Parse()
    {
        List<GameListEntry> entries = new List<GameListEntry>();
        string[] content = File.ReadAllLines(FullGameListPath);

        foreach (string line in content)
        {
            string[] idAndValues = line.Split(':');
            string[] entryValues = idAndValues[1].Split(',');

            Dictionary<string, string> values = new Dictionary<string, string>();

            foreach (string value in entryValues)
            {
                string[] keyAndValue = value.Split('=');
                values.Add(keyAndValue[0], keyAndValue[1]);
            }

            entries.Add(new GameListEntry(idAndValues[0], values));
        }

        return new GameList(entries);
    }
}
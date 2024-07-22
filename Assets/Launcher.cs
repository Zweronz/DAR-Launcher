using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class Launcher : MonoBehaviour
{
    public static GameList gameList;

    public GameObject button;

    public UIGrid grid;

    public UIPanel mainPanel, gamePanel;

    public UITexture background, icon, gameButtonTexture;

    public UILabel gameLabel, gameButtonLabel;

    public UIScrollView scrollView;

    public SimpleButton gameButton, backButton;

    public UIProgressBar gameDownloadBar;

    public GameObject fetching;

    private AudioSource source;

    private float panelTransparency = 1f;

    private bool switching, switchingBack;

    void Start()
    {
        source = GetComponent<AudioSource>();
        backButton.onClick = SwitchBack;

        GithubController.RedownloadData(()=>
        {
            gameList = GameListParser.Parse();
            fetching.SetActive(false);

            Sync();
        });
    }

    void Update()
    {
        gameDownloadBar.value = GithubController.downloadProgress;

        if (switching)
        {
            if (panelTransparency > 0f)
            {
                panelTransparency -= Time.deltaTime * 3f;
            }
            else
            {
                switching = false;
            }
        }
        else if (switchingBack)
        {
            if (panelTransparency < 1f)
            {
                panelTransparency += Time.deltaTime * 3f;
            }
            else
            {
                switchingBack = false;
            }
        }

        source.volume = 1f - panelTransparency;

        mainPanel.alpha = panelTransparency;
        gamePanel.alpha = 1f - panelTransparency;

        mainPanel.gameObject.SetActive(mainPanel.alpha > 0f);
        gamePanel.gameObject.SetActive(gamePanel.alpha > 0f);
    }

    private void Sync()
    {
        foreach (GameListEntry entry in gameList.entries)
        {
            GameButton gameButton = Instantiate(button, grid.transform).GetComponent<GameButton>();
            string bundle = entry.GetValue("overrideBundle");

            if (bundle == "")
            {
                bundle = entry.id;
            }

            gameButton.GetComponent<UIDragScrollView>().scrollView = scrollView;

            gameButton.label.text = entry.GetValue("name").Replace("\n", Environment.NewLine);
            gameButton.label.trueTypeFont = Bundles.Load<Font>("fonts", entry.GetValue("font"));

            gameButton.texture.mainTexture = Bundles.LoadIcon(bundle);
            gameButton.background.mainTexture = Bundles.LoadBackground(bundle);

            gameButton.onClick = ()=> { Switch(entry); };
        }

        grid.Reposition();
    }

    public void Switch(GameListEntry entry)
    {
        fetching.SetActive(true);

        GithubController.RefreshGame(entry.GetValue("repo"), ()=>
        {
            fetching.SetActive(false);
            switching = true;

            string bundle = entry.GetValue("overrideBundle");

            if (bundle == "")
            {
                bundle = entry.id;
            }

            source.clip = Bundles.LoadBGM(bundle);
            source.Play();

            background.mainTexture = Bundles.LoadBackground(bundle);
            icon.mainTexture = Bundles.LoadIcon(bundle);

            gameLabel.text = entry.GetValue("name");
            RefreshButton(entry, gameLabel.trueTypeFont = Bundles.Load<Font>("fonts", entry.GetValue("font")));
        });
    }

    private void RefreshButton(GameListEntry entry, Font font)
    {
        gameButtonLabel.trueTypeFont = font;

        switch (GithubController.currentStatus)
        {
            case GithubController.GameStatus.NotDownloaded:
                gameButtonTexture.color = Color.red;
                gameButtonLabel.text = "Download";
                break;

            case GithubController.GameStatus.Downloaded:
                gameButtonTexture.color = Color.green;
                gameButtonLabel.text = "Launch";
                gameDownloadBar.gameObject.SetActive(false);
                break;

            case GithubController.GameStatus.UpdateNeeded:
                gameButtonTexture.color = Color.blue;
                gameButtonLabel.text = "Update";
                break;
        }

        gameButton.onClick = ()=>
        {
            LaunchGame(entry);
        };
    }

    private void LaunchGame(GameListEntry entry)
    {
        switch (GithubController.currentStatus)
        {
            case GithubController.GameStatus.UpdateNeeded:
            case GithubController.GameStatus.NotDownloaded:
                GithubController.DownloadGame(entry.GetValue("repo"), ()=>
                {
                    GithubController.RefreshGame(entry.GetValue("repo"), () => 
                    {
                        RefreshButton(entry, Bundles.Load<Font>("fonts", entry.GetValue("font")));
                    });
                });
                break;

            case GithubController.GameStatus.Downloaded:
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo(GithubController.currentExecutablePath);

                process.Start();
                break;
        }
    }

    public void SwitchBack()
    {
        switchingBack = true;
    }
}
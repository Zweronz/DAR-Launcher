using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Launcher : MonoBehaviour
{
    public static GameList gameList;

    public GameObject button;

    public UIGrid grid;

    public UIPanel mainPanel, gamePanel;

    public UITexture background, icon;

    public UILabel gameLabel;

    public UIScrollView scrollView;

    private AudioSource source;

    private float panelTransparency = 1f;

    private bool switching, switchingBack;

    void Start()
    {
        source = GetComponent<AudioSource>();

        GithubController.RedownloadData(()=>
        {
            gameList = GameListParser.Parse();
            Sync();
        });
    }

    void Update()
    {
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

        if (Input.GetKeyDown(KeyCode.I))
        {
            SwitchBack();
        }
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
        gameLabel.trueTypeFont = Bundles.Load<Font>("fonts", entry.GetValue("font"));
    }

    public void SwitchBack()
    {
        switchingBack = true;
    }
}
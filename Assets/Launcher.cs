using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    void Start()
    {
        GithubController.RedownloadData(()=>
        {
            GetComponent<AudioSource>().PlayOneShot(Bundles.LoadBGM("call_of_mini_dino_hunter"));
        });
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButton : MonoBehaviour
{
    public UITexture texture, background;

    public UILabel label;

    public Action onClick;

    public void OnClick()
    {
        if (onClick != null)
        {
            onClick();
        }
    }
}
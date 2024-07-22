using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleButton : MonoBehaviour
{
    public Action onClick;

    public Texture clickedTexture;

    private Texture originalTexture;

    private UITexture texture;

    void Awake()
    {
        originalTexture = (texture = GetComponent<UITexture>()).mainTexture;
    }

    public void OnClick()
    {
        if (onClick != null)
        {
            onClick();
        }

        texture.mainTexture = originalTexture;
    }

    public void OnPress()
    {
        texture.mainTexture = clickedTexture;
    }
}

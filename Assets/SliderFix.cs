using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderFix : MonoBehaviour
{
    public UITexture foreground;

    public UIProgressBar progressBar;

    void LateUpdate()
    {
        //too lazy to make the pivots correct
        foreground.transform.localPosition = new Vector3(Mathf.Lerp(10, 0f, progressBar.value), 0f, 0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampAnchor : MonoBehaviour
{
    public UIWidget widget;

    void LateUpdate()
    {
        widget.SetDimensions((int)Mathf.Clamp(widget.localSize.x, 1920f, 9999f), (int)widget.localSize.y);
    }
}

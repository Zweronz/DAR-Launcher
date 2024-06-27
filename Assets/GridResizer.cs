using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridResizer : MonoBehaviour
{
    public int minimumElements, maximumElements;

    public UIPanel background;

    private UIGrid mGrid;

    private int lastSize;

    [Header("ignore if this isn't a category grid")]

    private Vector2 startingSize;

    private void Start()
    {
        mGrid = GetComponent<UIGrid>();
        startingSize = new Vector2(mGrid.cellWidth, mGrid.cellHeight);
    }

    public void Resize()
    {
        lastSize = mGrid.maxPerLine;
        mGrid.maxPerLine = Mathf.Clamp(-(int)((mGrid.maxPerLine / startingSize.x) - (background.width / startingSize.y)), minimumElements, maximumElements);
        
        if (lastSize != mGrid.maxPerLine)
        {
            mGrid.Reposition();
        }
    }

    private void Update()
    {
        Resize();
    }
}

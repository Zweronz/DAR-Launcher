using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScaler : MonoBehaviour
{
    private UIGrid mGrid;

    public UIPanel background;

    private Vector2 startSize;

    private Vector2 lastSize;

    void Start()
    {
        mGrid = GetComponent<UIGrid>();
        
        startSize = new Vector2(mGrid.cellWidth, mGrid.cellHeight);
    }

    void Update()
    {
        List<Transform> children = Utility.GetChildListFixed(transform);

        float mult = background.width / startSize.x / mGrid.maxPerLine;
        lastSize = new Vector2(mGrid.cellWidth, mGrid.cellHeight);

        for (int i = 0; i < children.Count; i++)
        {
            children[i].GetChild(0).localScale = Vector3.one * mult;
        }

        Vector2 newSize = new Vector2(mGrid.cellWidth = startSize.x * mult, mGrid.cellHeight = startSize.y * mult);

        if (newSize != lastSize)
        {
            mGrid.Reposition();
        }
    }
}

public class Utility
{
    public static List<Transform> GetChildListFixed(Transform root, bool recursive = false)
    {
        List<Transform> finalTransforms = new();

        for (int i = 0;; i++)
        {
            Transform child = null;

            try
            {
                child = root.GetChild(i);
            }
            catch
            {
                break;
            }

            finalTransforms.Add(child);
        }

        return finalTransforms;
    }
}
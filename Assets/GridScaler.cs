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

        mult = Mathf.Clamp(mult, 0f, 0.768f);

        for (int i = 0; i < children.Count; i++)
        {
            children[i].GetChild(0).localScale = Vector3.one * mult;
        }

        Vector2 newSize = new Vector2(mGrid.cellWidth = startSize.x * mult, mGrid.cellHeight = startSize.y * mult);

        mGrid.cellWidth = Mathf.Clamp(mGrid.cellWidth, 0f, 384f);
        mGrid.cellHeight = Mathf.Clamp(mGrid.cellHeight, 0f, 268.8f);

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
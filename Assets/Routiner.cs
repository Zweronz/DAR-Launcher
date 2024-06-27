using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Routiner : MonoBehaviour
{
    private static Routiner objectInstance;

    private new static Routiner gameObject
    {
        get
        {
            if (objectInstance == null)
            {
                objectInstance = new GameObject("Routiner").AddComponent<Routiner>();
                DontDestroyOnLoad(objectInstance);
            }
            return objectInstance;
        }
    }

    public static void Coroutine(IEnumerator routine)
    {
        gameObject.StartCoroutine(routine);
    }

    public static void CancelCoroutine(IEnumerator routine)
    {
        gameObject.StopCoroutine(routine);
    }
}

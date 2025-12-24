using UnityEngine;
using System.Collections.Generic;

public class RuntimeSingletons : MonoBehaviour
{
    static readonly List<GameObject> kept = new List<GameObject>();

    public static void Mark(GameObject go)
    {
        if (!go) return;
        if (!kept.Contains(go)) kept.Add(go);
        Object.DontDestroyOnLoad(go);
    }

    public static void DestroyAll()
    {
        for (int i = kept.Count - 1; i >= 0; i--)
        {
            if (kept[i]) Object.Destroy(kept[i]);
        }
        kept.Clear();
    }
}

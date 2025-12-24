using System.Collections.Generic;
using UnityEngine;

public class HomePenManager : MonoBehaviour
{
    public static HomePenManager I;

    public Transform slotsRoot;

    readonly List<Transform> slots = new List<Transform>();
    readonly List<GameObject> stored = new List<GameObject>();

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        RuntimeSingletons.Mark(gameObject);

        RebuildSlotsFromRoot();
    }

    public int StoredCount => stored.Count;
    public int SlotCount => slots.Count;

    public void RebuildSlotsFromRoot()
    {
        slots.Clear();
        if (!slotsRoot) return;

        for (int i = 0; i < slotsRoot.childCount; i++)
            slots.Add(slotsRoot.GetChild(i));

        RepackAll();
    }

    public bool HasFreeSlot()
    {
        return stored.Count < slots.Count;
    }

    public void StoreChicken(GameObject chicken)
    {
        if (!chicken) return;
        if (!HasFreeSlot()) return;

        if (!stored.Contains(chicken))
            stored.Add(chicken);

        chicken.SetActive(true);
        chicken.transform.SetParent(transform, true);

        var col = chicken.GetComponent<Collider2D>();
        if (col) col.enabled = false;

        RepackAll();
    }

    public void RepackAll()
    {
        for (int i = 0; i < stored.Count; i++)
        {
            if (!stored[i]) continue;
            if (i < slots.Count && slots[i])
                stored[i].transform.position = slots[i].position;
        }
    }

    public int ReturnAllToFarmhouse()
    {
        int n = stored.Count;
        for (int i = 0; i < stored.Count; i++)
            if (stored[i]) stored[i].SetActive(false);

        stored.Clear();
        return n;
    }

    public int RemoveChickens(int count)
    {
        int removed = Mathf.Min(count, stored.Count);
        for (int i = 0; i < removed; i++)
        {
            var c = stored[0];
            stored.RemoveAt(0);
            if (c) c.SetActive(false);
        }
        RepackAll();
        return removed;
    }
}

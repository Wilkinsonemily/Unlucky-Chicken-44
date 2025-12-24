using System.Collections.Generic;
using UnityEngine;

public class SeedSpawner : MonoBehaviour
{
    public static SeedSpawner I;

    public GameObject seedPrefab;
    public Transform spawnPointsRoot;

    public int minSeeds = 3;
    public int maxSeeds = 7;

    readonly List<Transform> spawnPoints = new List<Transform>();
    readonly List<GameObject> alive = new List<GameObject>();

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    void Start()
    {
        RebuildSpawnPoints();
        SpawnNewDay();
    }


    public void RebuildSpawnPoints()
    {
        spawnPoints.Clear();

        if (spawnPointsRoot == null) return;

        for (int i = 0; i < spawnPointsRoot.childCount; i++)
        {
            var t = spawnPointsRoot.GetChild(i);
            if (t != null) spawnPoints.Add(t);
        }
    }

    public void SpawnNewDay()
    {
        for (int i = 0; i < alive.Count; i++)
            if (alive[i]) Destroy(alive[i]);
        alive.Clear();

        if (seedPrefab == null)
        {
            return;
        }

        if (spawnPointsRoot == null)
        {
            return;
        }

        if (spawnPoints.Count == 0)
            RebuildSpawnPoints();

        if (spawnPoints.Count == 0)
        {
            return;
        }

        int count = Random.Range(minSeeds, maxSeeds + 1);
        count = Mathf.Clamp(count, 0, spawnPoints.Count);

        var temp = new List<Transform>(spawnPoints);
        for (int i = 0; i < temp.Count; i++)
        {
            int j = Random.Range(i, temp.Count);
            (temp[i], temp[j]) = (temp[j], temp[i]);
        }

        for (int k = 0; k < count; k++)
        {
            Vector3 pos = temp[k].position;
            pos.z = 0f;
            var go = Instantiate(seedPrefab, pos, Quaternion.identity);
            alive.Add(go);
        }

    }
}

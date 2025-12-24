using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenPopulationManager : MonoBehaviour
{
    public static ChickenPopulationManager I;

    public List<ChickenDailyRoutine> chickens = new List<ChickenDailyRoutine>();
    public int minOutside = 4;
    public int maxOutside = 8;

    bool subscribed;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        RuntimeSingletons.Mark(gameObject);

    }

    IEnumerator Start()
    {
        while (DayNightManager.I == null) yield return null;
        Subscribe();
        HandleMorning();
    }

    void OnDisable()
    {
        Unsubscribe();
    }

    void Subscribe()
    {
        if (subscribed) return;
        DayNightManager.I.OnDayStarted.AddListener(HandleMorning);
        DayNightManager.I.OnNightStarted.AddListener(HandleNight);
        subscribed = true;
    }

    void Unsubscribe()
    {
        if (!subscribed) return;
        if (DayNightManager.I != null)
        {
            DayNightManager.I.OnDayStarted.RemoveListener(HandleMorning);
            DayNightManager.I.OnNightStarted.RemoveListener(HandleNight);
        }
        subscribed = false;
    }

    public void HandleMorning()
    {
        foreach (var c in chickens)
            if (c != null) c.GoInside();

        if (chickens.Count == 0) return;

        int count = Random.Range(minOutside, maxOutside + 1);
        count = Mathf.Min(count, chickens.Count);

        var temp = new List<ChickenDailyRoutine>(chickens);
        for (int i = 0; i < temp.Count; i++)
        {
            int j = Random.Range(i, temp.Count);
            var x = temp[i];
            temp[i] = temp[j];
            temp[j] = x;
        }

        for (int k = 0; k < count; k++)
            if (temp[k] != null) temp[k].GoOutside();

        ToastUI.Say($"{count} chickens went outside.");
    }

    public void HandleNight()
    {
        foreach (var c in chickens)
            if (c != null) c.GoInside();
    }
}

using UnityEngine;
using System.Collections;

public class PlayerEnergy : MonoBehaviour
{
    public int maxEnergy = 100;

    public float baseNightDrainPerSecond = 2f;

    public float nightRampPerSecond = 0.25f;
    public float nightRampMaxMultiplier = 3f;

    public int captureDrain = 5;
    public int seedCollectDrain = 2;

    public Transform faintSpawnPoint;

    public int Energy { get; private set; }

    bool fainting;
    float drainAcc;
    float nightTimer;

    void Start()
    {
        Energy = maxEnergy;
        HUD.I?.RefreshAll();
    }

    void Update()
    {
        if (fainting) return;
        if (DayNightManager.I == null) return;

        if (DayNightManager.I.IsNight)
        {
            nightTimer += Time.deltaTime;

            float mult = 1f + nightTimer * nightRampPerSecond;
            mult = Mathf.Min(mult, nightRampMaxMultiplier);

            float drainPerSec = baseNightDrainPerSecond * mult;

            drainAcc += drainPerSec * Time.deltaTime;
            int d = Mathf.FloorToInt(drainAcc);
            if (d > 0)
            {
                drainAcc -= d;
                Drain(d, "exhaustion");
            }
        }
        else
        {
            nightTimer = 0f;
            drainAcc = 0f;
        }

        if (Energy <= 0 && !fainting)
        {
            fainting = true;
            StartCoroutine(FaintRoutine("exhaustion"));
        }
    }

    public void DrainForCapture()
    {
        Drain(captureDrain, "exhaustion");
    }

    public void DrainForSeedCollect()
    {
        Drain(seedCollectDrain, "exhaustion");
    }

    public void Drain(int amount, string faintReasonIfZero)
    {
        if (amount <= 0) return;

        Energy = Mathf.Max(0, Energy - amount);
        HUD.I?.RefreshAll();

        if (Energy <= 0 && !fainting)
        {
            fainting = true;
            StartCoroutine(FaintRoutine(faintReasonIfZero));
        }
    }

    public void RestoreFull()
    {
        Energy = maxEnergy;
        drainAcc = 0f;
        nightTimer = 0f;
        HUD.I?.RefreshAll();
    }

    IEnumerator FaintRoutine(string reason)
    {
        ToastUI.Say($"You fainted {reason}.");

        yield return new WaitForSecondsRealtime(1.2f);

        if (DayNightManager.I != null)
            yield return DayNightManager.I.FadeToBlackOnly();

        GameManager.I.ApplyFaintPenaltyNoNextDay(reason);

        RestoreFull();

        if (faintSpawnPoint != null)
            transform.position = faintSpawnPoint.position;

        yield return new WaitForSecondsRealtime(0.15f);

        if (DayNightManager.I != null)
            yield return DayNightManager.I.FadeFromBlackOnly();

        DayNightManager.I?.SleepToMorning();

        fainting = false;
    }
    public void ForceSetEnergy(int e)
    {
        Energy = Mathf.Clamp(e, 0, maxEnergy);
        HUD.I?.RefreshAll();
    }
}

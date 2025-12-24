using UnityEngine;
using System.Collections;

public class Bed : MonoBehaviour, IInteractable2D
{
    bool sleeping;

    public void Interact(GameObject interactor)
    {
        if (sleeping) return;

        if (DayNightManager.I == null)
        {
            ToastUI.Say("DayNightManager not found.");
            return;
        }

        if (!DayNightManager.I.IsNight)
        {
            ToastUI.Say("It's not night yet.");
            return;
        }

        sleeping = true;
        ToastUI.Say("You go to sleep...");
        StartCoroutine(SleepRoutine(interactor));
    }

    IEnumerator SleepRoutine(GameObject player)
    {
        yield return DayNightManager.I.SleepWithFade();

        var pe = player.GetComponent<PlayerEnergy>();
        if (pe != null)
        {
            var m = pe.GetType().GetMethod("OnSlept");
            if (m != null) m.Invoke(pe, null);
            else pe.RestoreFull();
        }

        GameManager.I?.NextDayPaid();
        SeedSpawner.I?.SpawnNewDay();

        sleeping = false;
    }
}

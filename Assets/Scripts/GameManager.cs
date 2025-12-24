using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    public int Day { get; set; } = 1;
    public int pendingWage = 0;

    public int faintCost = 200;
    public int chickenPenalty = 2;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    RuntimeSingletons.Mark(gameObject);

    }

    public void AddPendingWage(int amount)
    {
        if (amount <= 0) return;
        pendingWage += amount;
        HUD.I?.RefreshAll();
    }

    public void NextDayPaid()
    {
        if (pendingWage > 0 && PlayerInventory.I != null)
        {
            PlayerInventory.I.AddCoins(pendingWage);
            ToastUI.Say($"+{pendingWage} wages received!");
            pendingWage = 0;
        }

        Day += 1;
        HUD.I?.RefreshAll();
    }

    public void ApplyFaintPenaltyNoNextDay(string faintReason)
    {
        if (PlayerInventory.I == null)
        {
            ToastUI.Say("Penalty failed: PlayerInventory missing.");
            return;
        }

        if (pendingWage > 0)
        {
            int loss = Mathf.Min(pendingWage, faintCost);
            pendingWage -= loss;
            ToastUI.Say($"You fainted ({faintReason})... {loss} was taken from your wages.");
            HUD.I?.RefreshAll();
            return;
        }

        if (PlayerInventory.I.Coins > 0)
        {
            int loss = Mathf.Min(PlayerInventory.I.Coins, faintCost);
            PlayerInventory.I.LoseCoins(loss);
            ToastUI.Say($"You fainted ({faintReason})... {loss} coins were taken.");
            HUD.I?.RefreshAll();
            return;
        }

        if (HomePenManager.I != null && HomePenManager.I.StoredCount > 0)
        {
            int removed = HomePenManager.I.RemoveChickens(chickenPenalty);
            ToastUI.Say($"You fainted ({faintReason})... {removed} chickens were taken.");
            HUD.I?.RefreshAll();
            return;
        }

        string reason = $"You fainted because {faintReason}.\nYou had no wages, no coins, and no chickens to cover the cost.";
        GameOverUI.I?.Show(reason);
    }
    public void LoadFromSave(int day, int wage)
    {
        Day = Mathf.Max(1, day);
        pendingWage = Mathf.Max(0, wage);
        HUD.I?.RefreshAll();
    }
    public void SetDay(int day)
    {
        Day = Mathf.Max(1, day);
        HUD.I?.RefreshAll();
    }
    public void ForceSetDay(int d)
    {
        Day = Mathf.Max(1, d);
        HUD.I?.RefreshAll();
    }
    public void DebugSetState(int day, int wage)
    {
        Day = Mathf.Max(1, day);
        pendingWage = Mathf.Max(0, wage);
        HUD.I?.RefreshAll();
    }
    public void ForceSet(int day, int wage)
    {
        Day = day;
        pendingWage = wage;
        HUD.I?.RefreshAll();
    }
    public void LoadDayAndWage(int day, int wage)
    {
        Day = Mathf.Max(1, day);
        pendingWage = Mathf.Max(0, wage);
        HUD.I?.RefreshAll();
    }
}

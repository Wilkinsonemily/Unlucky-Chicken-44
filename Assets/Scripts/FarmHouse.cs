using UnityEngine;

public class Farmhouse : MonoBehaviour, IInteractable2D
{
    public int payPerChicken = 100;

    public void Interact(GameObject interactor)
    {
        if (HomePenManager.I == null)
        {
            ToastUI.Say("Home pen not found.");
            return;
        }

        int storedNow = HomePenManager.I.StoredCount;
        if (storedNow <= 0)
        {
            return;
        }

        int count = HomePenManager.I.ReturnAllToFarmhouse();
        if (count <= 0)
        {
            
            return;
        }

        int pay = count * payPerChicken;

        if (GameManager.I != null)
        {
            GameManager.I.AddPendingWage(pay);
            ToastUI.Say($"Returned {count} chicken(s). Wage +{pay} (paid tomorrow). Pending: {GameManager.I.pendingWage}");
        }
        else
        {
            PlayerInventory.I.AddCoins(pay);
            ToastUI.Say($"Returned {count} chicken(s). +{pay} coins.");
        }

        PlayerInventory.I?.AddReturned(count);
        HUD.I?.RefreshAll();
    }
}

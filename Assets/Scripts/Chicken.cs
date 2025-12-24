using UnityEngine;

public class Chicken : MonoBehaviour, IInteractable2D
{
    public enum BaitType { FeedOnly, SeedOnly, Either }
    public BaitType baitType = BaitType.FeedOnly;

    bool captured;

    public void Interact(GameObject interactor)
    {
        if (captured) return;
        if (PlayerInventory.I == null) return;

        if (HomePenManager.I == null)
        {
            ToastUI.Say("Home pen not found.");
            return;
        }

        if (!HomePenManager.I.HasFreeSlot())
        {
            ToastUI.Say("Your chicken pen is full.");
            return;
        }

        if (!TryConsumeBait(out string failMsg))
        {
            ToastUI.Say(failMsg);
            return;
        }

        captured = true;

        var pe = interactor ? interactor.GetComponent<PlayerEnergy>() : null;
        if (pe) pe.DrainForCapture();

        HomePenManager.I.StoreChicken(gameObject);
        ToastUI.Say("Chicken captured.");
        HUD.I?.RefreshAll();
    }

    bool TryConsumeBait(out string failMsg)
    {
        failMsg = "";

        if (baitType == BaitType.FeedOnly)
        {
            if (PlayerInventory.I.Feed <= 0) { failMsg = "This chicken doesn't like your bait"; return false; }
            return PlayerInventory.I.UseOneFeed();
        }

        if (baitType == BaitType.SeedOnly)
        {
            if (PlayerInventory.I.Seeds <= 0) { failMsg = "This chicken wants Seeds (need 1 Seed)."; return false; }
            return PlayerInventory.I.UseOneSeed();
        }

        if (PlayerInventory.I.Seeds <= 0 && PlayerInventory.I.Feed <= 0)
        {
            failMsg = "You need 1 Seed or 1 Feed.";
            return false;
        }

        return PlayerInventory.I.UseOneSeedOrFeedPreferSeed();
    }
}

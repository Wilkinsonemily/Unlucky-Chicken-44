using System.Collections;
using UnityEngine;

public class OtherFarmerNPC : MonoBehaviour, IInteractable2D
{
    [Header("Payment")]
    public int pricePerChicken = 150;

    [Header("Release")]
    public Transform releasePoint;
    public float releaseOffsetRadius = 0.6f;
    public ChickenLockedPaidAndFeed[] chickensInFarm;

    [Header("Anti Double Interact")]
    public float interactLockSeconds = 0.25f;

    int nextIndex;
    int releaseCounter;

    bool busy;
    float lastInteractTime = -999f;

    public void Interact(GameObject interactor)
    {
        if (busy) return;

        float now = Time.unscaledTime;
        if (now - lastInteractTime < interactLockSeconds) return;
        lastInteractTime = now;

        int idx = FindNextAvailableChickenIndex();
        if (idx < 0)
        {
            ToastUI.Say("No chickens to release right now.");
            return;
        }

        if (PlayerInventory.I == null)
        {
            ToastUI.Say("Inventory not found.");
            return;
        }

        if (!PlayerInventory.I.SpendCoins(pricePerChicken))
        {
            ToastUI.Say("Not enough coins to negotiate with the other farmer.");
            return;
        }

        busy = true;

        var chicken = chickensInFarm[idx];
        if (chicken == null)
        {
            busy = false;
            ToastUI.Say("Chicken missing.");
            return;
        }

        Vector3 basePos = releasePoint ? releasePoint.position : (transform.position + Vector3.right * 0.5f);
        Vector2 offset = PolarOffset(releaseCounter++, releaseOffsetRadius);
        Vector3 dropPos = basePos + new Vector3(offset.x, offset.y, 0f);

        chicken.ReleaseTo(dropPos);

        ToastUI.Say($"Paid {pricePerChicken} coins. One chicken was released.");

        StartCoroutine(UnlockAfterDelay());
    }

    IEnumerator UnlockAfterDelay()
    {
        yield return new WaitForSecondsRealtime(interactLockSeconds);
        busy = false;
    }

    int FindNextAvailableChickenIndex()
    {
        int n = chickensInFarm != null ? chickensInFarm.Length : 0;
        if (n == 0) return -1;

        for (int step = 0; step < n; step++)
        {
            int i = (nextIndex + step) % n;
            var c = chickensInFarm[i];
            if (c == null) continue;
            if (!c.gameObject.activeSelf) continue;
            if (c.captured) continue;
            if (!c.inFarm) continue;

            nextIndex = (i + 1) % n;
            return i;
        }

        return -1;
    }

    Vector2 PolarOffset(int index, float radius)
    {
        int slots = 8;
        int ringIndex = index / slots;
        int slotIndex = index % slots;

        float angle = (slotIndex / (float)slots) * Mathf.PI * 2f;
        float r = radius + ringIndex * (radius * 0.75f);

        return new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
    }
}

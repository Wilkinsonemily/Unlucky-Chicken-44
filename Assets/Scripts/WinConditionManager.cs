using UnityEngine;

public class WinConditionManager : MonoBehaviour
{
    public int targetReturned = 44;
    bool won;

    void Update()
    {
        if (won) return;
        if (PlayerInventory.I == null) return;

        if (PlayerInventory.I.ReturnedCount >= targetReturned)
        {
            won = true;

            int day = GameManager.I != null ? GameManager.I.Day : 1;
            int money = PlayerInventory.I.Coins;

            if (WinUI.I != null)
                WinUI.I.ShowWin(day, money);
            else
                Debug.LogError("WinUI not found in scene.");
        }
    }
}

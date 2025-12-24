using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD I;

    public TMP_Text coinsText;
    public TMP_Text feedText;
    public TMP_Text returnedText;
    public TMP_Text dayText;
    public TMP_Text energyText;
    public TMP_Text wageText;

    PlayerEnergy energy;

    void Awake() { I = this; }

    void Start()
    {
        energy = FindObjectOfType<PlayerEnergy>();
        RefreshAll();
    }

    public void RefreshAll()
    {
        if (PlayerInventory.I != null)
        {
            coinsText.text = PlayerInventory.I.Coins.ToString();
            feedText.text = PlayerInventory.I.Feed.ToString();
            returnedText.text = $"{PlayerInventory.I.ReturnedCount}/44";
        }

        if (GameManager.I != null)
        {
            dayText.text = $"Day {GameManager.I.Day}";
            wageText.text = $"Wage: {GameManager.I.pendingWage}";
        }

        if (energy != null)
        {
            energyText.text = $"Energy: {energy.Energy}";
        }
    }
}

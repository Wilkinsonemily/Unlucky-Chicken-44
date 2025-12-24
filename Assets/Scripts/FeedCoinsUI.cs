using UnityEngine;
using TMPro;

public class FeedCoinsUI : MonoBehaviour
{
    public static FeedCoinsUI I;

    public TMP_Text coinsText;
    public TMP_Text feedText;
    public TMP_Text seedText;
    public TMP_Text returnedText;

    void Awake()
    {
        I = this;
    }

    public void Refresh(int coins, int feed, int seeds, int returned)
    {
        if (coinsText) coinsText.text = $"{coins}";
        if (feedText) feedText.text = $"{feed}";
        if (seedText) seedText.text = $"{seeds}";
        if (returnedText) returnedText.text = $"{returned}/44";
    }
}

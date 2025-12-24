using UnityEngine;

public static class HighscoreManager
{
    const string KEY_BEST_DAY = "HS_BEST_DAY";
    const string KEY_BEST_MONEY = "HS_BEST_MONEY";

    public static int BestDay => PlayerPrefs.GetInt(KEY_BEST_DAY, 999999);
    public static int BestMoney => PlayerPrefs.GetInt(KEY_BEST_MONEY, 0);

    public static bool TrySubmit(int day, int money)
    {
        int bestDay = BestDay;
        int bestMoney = BestMoney;

        bool better =
            day < bestDay ||
            (day == bestDay && money > bestMoney);

        if (better)
        {
            PlayerPrefs.SetInt(KEY_BEST_DAY, day);
            PlayerPrefs.SetInt(KEY_BEST_MONEY, money);
            PlayerPrefs.Save();
        }

        return better;
    }

    public static string FormatBest()
    {
        if (BestDay == 999999) return "Highscore: -";
        return $"Highscore:\nDay: {BestDay}\nMoney: {BestMoney}";
    }
    public static int ComputeScore(int day, int money)
    {
        day = Mathf.Max(1, day);
        return Mathf.RoundToInt((money / (float)day) * 100f);
    }

}

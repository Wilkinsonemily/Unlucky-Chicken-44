using UnityEngine;

public static class Highscore
{
    const string BestDayKey = "HS_BestDay";
    const string BestMoneyKey = "HS_BestMoney";
    const string HasKey = "HS_Has";

    public static bool HasScore()
    {
        return PlayerPrefs.GetInt(HasKey, 0) == 1;
    }

    public static (int day, int money) Get()
    {
        int d = PlayerPrefs.GetInt(BestDayKey, 999999);
        int m = PlayerPrefs.GetInt(BestMoneyKey, 0);
        return (d, m);
    }

    public static bool TrySet(int day, int money)
    {
        if (!HasScore())
        {
            Save(day, money);
            return true;
        }

        var cur = Get();

        bool better =
            (day < cur.day) ||
            (day == cur.day && money > cur.money);

        if (better)
        {
            Save(day, money);
            return true;
        }

        return false;
    }

    static void Save(int day, int money)
    {
        PlayerPrefs.SetInt(HasKey, 1);
        PlayerPrefs.SetInt(BestDayKey, day);
        PlayerPrefs.SetInt(BestMoneyKey, money);
        PlayerPrefs.Save();
    }

    public static void Reset()
    {
        PlayerPrefs.DeleteKey(HasKey);
        PlayerPrefs.DeleteKey(BestDayKey);
        PlayerPrefs.DeleteKey(BestMoneyKey);
        PlayerPrefs.Save();
    }
}

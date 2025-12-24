using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory I;

    public int startCoins = 100;
    public int startFeed = 0;
    public int startSeeds = 0;

    public int Coins { get; private set; }
    public int Feed { get; private set; }
    public int Seeds { get; private set; }

    public int ReturnedCount { get; private set; }

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        RuntimeSingletons.Mark(gameObject);

    }

    void Start()
    {
        Coins = startCoins;
        Feed = startFeed;
        Seeds = startSeeds;
        RefreshUI();
    }

    void RefreshUI()
    {
        FeedCoinsUI.I?.Refresh(Coins, Feed, Seeds, ReturnedCount);
        HUD.I?.RefreshAll();
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        Coins += amount;
        RefreshUI();
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0) return true;
        if (Coins < amount) return false;
        Coins -= amount;
        RefreshUI();
        return true;
    }

    public void LoseCoins(int amount)
    {
        if (amount <= 0) return;
        Coins = Mathf.Max(0, Coins - amount);
        RefreshUI();
    }

    public void AddFeed(int amount)
    {
        if (amount <= 0) return;
        Feed += amount;
        RefreshUI();
    }

    public bool UseOneFeed()
    {
        if (Feed <= 0) return false;
        Feed -= 1;
        RefreshUI();
        return true;
    }

    public void AddSeeds(int amount)
    {
        if (amount <= 0) return;
        Seeds += amount;
        RefreshUI();
    }

    public bool UseOneSeed()
    {
        if (Seeds <= 0) return false;
        Seeds -= 1;
        RefreshUI();
        return true;
    }

    public bool UseOneSeedOrFeedPreferSeed()
    {
        if (Seeds > 0) { Seeds -= 1; RefreshUI(); return true; }
        if (Feed > 0) { Feed -= 1; RefreshUI(); return true; }
        return false;
    }

    public void AddReturned(int amount)
    {
        if (amount <= 0) return;
        ReturnedCount += amount;
        RefreshUI();
    }
    public void LoadFromSave(int coins, int feed, int seeds, int returned)
    {
        Coins = Mathf.Max(0, coins);
        Feed = Mathf.Max(0, feed);
        Seeds = Mathf.Max(0, seeds);
        ReturnedCount = Mathf.Max(0, returned);
        FeedCoinsUI.I?.Refresh(Coins, Feed, Seeds, ReturnedCount);
        HUD.I?.RefreshAll();
    }
    public void SetAll(int coins, int feed, int seeds, int returned)
    {
        Coins = Mathf.Max(0, coins);
        Feed = Mathf.Max(0, feed);
        Seeds = Mathf.Max(0, seeds);
        ReturnedCount = Mathf.Max(0, returned);
        RefreshUI();
    }
    public void ForceSetAll(int coins, int feed, int seeds, int returned)
    {
        Coins = Mathf.Max(0, coins);
        Feed = Mathf.Max(0, feed);
        Seeds = Mathf.Max(0, seeds);
        ReturnedCount = Mathf.Max(0, returned);
        RefreshUI();
    }
    public void DebugSetState(int coins, int feed, int seeds, int returned)
    {
        Coins = Mathf.Max(0, coins);
        Feed = Mathf.Max(0, feed);
        Seeds = Mathf.Max(0, seeds);
        ReturnedCount = Mathf.Max(0, returned);
        RefreshUI();
    }
    public void ForceSet(int coins, int feed, int seeds, int returned)
    {
        Coins = coins;
        Feed = feed;
        Seeds = seeds;
        ReturnedCount = returned;
        RefreshUI();
    }
}

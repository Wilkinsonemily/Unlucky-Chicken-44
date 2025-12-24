using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameResetter
{
    public static void HardRestartToScene(int sceneBuildIndex = 0)
    {
        Time.timeScale = 1f;

        DestroyIfExists(GameManager.I);
        DestroyIfExists(PlayerInventory.I);
        DestroyIfExists(HomePenManager.I);
        DestroyIfExists(DayNightManager.I);
        DestroyIfExists(ChickenPopulationManager.I);
        DestroyIfExists(SeedSpawner.I);
        DestroyIfExists(ToastUI.I);
        DestroyIfExists(HUD.I);
        DestroyIfExists(WinUI.I);
        DestroyIfExists(GameOverUI.I);

        SceneManager.LoadScene(sceneBuildIndex);
    }

    static void DestroyIfExists(Object o)
    {
        if (o == null) return;
        Object.Destroy(o);
    }
}

public enum GameStartMode { NewGame, Continue }

public static class GameStartContext
{
    public static GameStartMode Mode = GameStartMode.NewGame;
    public static string GameSceneName = "GameScene";
    public static void SetMode(GameStartMode mode) => Mode = mode;
}
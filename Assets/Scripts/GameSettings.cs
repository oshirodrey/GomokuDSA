public static class GameSettings
{
    public enum GameDifficulty { Easy, Normal, Hard, Extreme }
    public static GameDifficulty currentDifficulty = GameDifficulty.Normal;

    public static void SetDifficulty(GameDifficulty difficulty)
    {
        currentDifficulty = difficulty;
    }
}

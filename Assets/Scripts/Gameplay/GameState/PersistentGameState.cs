namespace Gameplay.GameState
{
    public enum GameResultState
    {
        Invalid,
        X_Won,
        O_Won,
        Draw
    }

    /// <summary>
    /// Class containing some data that needs to be passed between ServerBossRoomState and PostGameState to represent the game session's win state.
    /// </summary>
    public class PersistentGameState
    {
        public GameResultState GameResultState { get; private set; }

        public void SetGameResult(GameResultState gameResultState)
        {
            GameResultState = gameResultState;
        }

        public void Reset()
        {
            GameResultState = GameResultState.Invalid;
        }
    }
}

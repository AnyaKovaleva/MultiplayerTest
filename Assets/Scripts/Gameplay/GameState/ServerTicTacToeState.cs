using System.Collections.Generic;
using System.Threading.Tasks;
using ConnectionManagement;
using Enums;
using Gameplay.Structs;
using Infrastructure.PubSub;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;
using Utils;
using VContainer;

namespace Gameplay.GameState
{
    [RequireComponent(typeof(NetcodeHooks), typeof(NetworkTicTacToe))]
    public class ServerTicTacToeState : GameStateBehaviour
    {
        public override GameState ActiveState
        {
            get { return GameState.TicTacToe; }
        }

        [SerializeField] private NetcodeHooks _netcodeHooks;

        [SerializeField] private NetworkTicTacToe _networkTicTacToe;

        [Inject] private ConnectionManager _connectionManager;
        [Inject] private PersistentGameState _persistentGameState;

        private List<SessionPlayerData> _players;

        private GameMarkType[,] _gameFieldValues;
        private int _gridSize = 3;

        protected override void Awake()
        {
            base.Awake();
            _netcodeHooks = GetComponent<NetcodeHooks>();
            _networkTicTacToe = GetComponent<NetworkTicTacToe>();
            _netcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            _netcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;
        }

        void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                enabled = false;
                return;
            }

            _persistentGameState.Reset();
            // m_LifeStateChangedEventMessageSubscriber.Subscribe(OnLifeStateChangedEventMessage);
            //
            // NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            // NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
            NetworkManager.Singleton.SceneManager.OnSynchronizeComplete += OnSynchronizeComplete;

            _gameFieldValues = new GameMarkType[GameField.FieldSize, GameField.FieldSize];
            _gridSize = GameField.FieldSize;
            
            _networkTicTacToe.OnClientMadeMove += ValidateClientMove;

            SessionManager<SessionPlayerData>.Instance.OnSessionStarted();

            _players = new List<SessionPlayerData>();
            foreach (var clientID in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Debug.Log("client id " + clientID);
                var playerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientID);
                if (playerData.HasValue)
                {
                    _players.Add(playerData.Value);
                    Debug.Log($"player name: {playerData.Value.PlayerName} seat type: {playerData.Value.MarkType}");
                }
                else
                {
                    Debug.LogError("Cant get data of " + clientID);
                }
            }

            //X starts game
            _networkTicTacToe.CurrentPlayerTurn.Value = _players.Find(x => x.MarkType == GameMarkType.X).ClientID;
            _networkTicTacToe.CurrentSessionState.Value = NetworkTicTacToe.SessionState.Active;
        }

        void OnNetworkDespawn()
        {
            //unsubscribe from everything
            // m_LifeStateChangedEventMessageSubscriber?.Unsubscribe(OnLifeStateChangedEventMessage);
            // NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            // NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
            NetworkManager.Singleton.SceneManager.OnSynchronizeComplete -= OnSynchronizeComplete;
            _networkTicTacToe.OnClientMadeMove -= ValidateClientMove;
        }

        protected override void OnDestroy()
        {
            // m_LifeStateChangedEventMessageSubscriber?.Unsubscribe(OnLifeStateChangedEventMessage);

            if (_netcodeHooks)
            {
                _netcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
                _netcodeHooks.OnNetworkDespawnHook -= OnNetworkDespawn;
            }

            base.OnDestroy();
        }

        void OnSynchronizeComplete(ulong clientId)
        {
            Debug.Log("syncing " + clientId);
            // if (InitialSpawnDone && !PlayerServerCharacter.GetPlayerServerCharacter(clientId))
            // {
            //     //somebody joined after the initial spawn. This is a Late Join scenario. This player may have issues
            //     //(either because multiple people are late-joining at once, or because some dynamic entities are
            //     //getting spawned while joining. But that's not something we can fully address by changes in
            //     //ServerBossRoomState.
            //     SpawnPlayer(clientId, true);
            // }
        }

        private void ValidateClientMove(ulong clientId, Coord coord)
        {
            if (clientId != _networkTicTacToe.CurrentPlayerTurn.Value)
            {
                Debug.Log("not his turn! client id" + clientId);
                return;
            }

            Debug.Log($"Client {clientId} Placing Piece at {coord}");

            if (_gameFieldValues[coord.x, coord.y] != GameMarkType.NONE)
            {
                Debug.Log($@"client {clientId} tries to place mark at occupied grid");
                return;
            }

            //perhaps validate the move and tel client that if its incorrect
            GameMarkType mark = _players.Find(x => x.ClientID == _networkTicTacToe.CurrentPlayerTurn.Value).MarkType;
            _gameFieldValues[coord.x,  coord.y] = mark;
            _networkTicTacToe.UpdateGridValueClientRpc(coord, mark);

            CheckForGameOver();

            EndCurrentPlayerTurn();
        }

        private void EndCurrentPlayerTurn()
        {
            _networkTicTacToe.CurrentPlayerTurn.Value = _players[GetNextPlayerIndex()].ClientID;
        }

        private int GetNextPlayerIndex()
        {
            if (_players.Find(x => x.ClientID == _networkTicTacToe.CurrentPlayerTurn.Value).MarkType == GameMarkType.X)
            {
                return _players.FindIndex(x => x.MarkType == GameMarkType.O);
            }

            return _players.FindIndex(x => x.MarkType == GameMarkType.X);
        }

        private void CheckForGameOver()
        {
            foreach (var player in _players)
            {
                if (IsVictory(player.MarkType))
                {
                    GameOver(player.MarkType == GameMarkType.X ? GameResultState.X_Won : GameResultState.O_Won);
                    return;
                }
            }

            if (IsGameFieldFull())
            {
                GameOver(GameResultState.Draw);
            }
        }

        private bool IsVictory(GameMarkType markType)
        {
            //check rows and colons 
            for (int i = 0; i < _gridSize; i++)
            {
                if (CountLinePieces(markType, i, 0, 0, 1) == _gridSize)
                {
                    return true;
                }

                if (CountLinePieces(markType, 0, 1, 1, 0) == _gridSize)
                {
                    return true;
                }
            }

            //check diagonals
            if (CountLinePieces(markType, 0, 0, 1, 1) == _gridSize)
            {
                return true;
            }

            if (CountLinePieces(markType, 0, _gridSize - 1, 1, -1) == _gridSize)
            {
                return true;
            }

            return false;
        }

        private int CountLinePieces(GameMarkType markType, int x, int y, int dx, int dy)
        {
            int count = 0;
            for (int i = 0; i < _gridSize; i++, x += dx, y += dy)
            {
                if (_gameFieldValues[x, y] == markType)
                {
                    count++;
                }
            }

            return count;
        }

        private bool IsGameFieldFull()
        {
            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    if (_gameFieldValues[i, j] == GameMarkType.NONE)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private async void GameOver(GameResultState gameResult)
        {
            _persistentGameState.SetGameResult(gameResult);

            _networkTicTacToe.CurrentSessionState.Value = NetworkTicTacToe.SessionState.GameFinished;

            await Task.Delay(3000);
            
            SceneLoaderWrapper.Instance.LoadScene(
                "PostGame", useNetworkSceneManager: true);
        }
    }
}
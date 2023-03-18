using System;
using System.Threading.Tasks;
using ConnectionManagement;
using Enums;
using Gameplay.Structs;
using Initializers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;
using VContainer;
using VContainer.Unity;
using Views.ViewControllers;

namespace Gameplay.GameState
{
    [RequireComponent(typeof(NetcodeHooks), typeof(NetworkTicTacToe))]
    public class ClientTicTacToeState : GameStateBehaviour
    {
        [SerializeField] private GameField _gameField;

        public static ClientTicTacToeState Instance;

        public override GameState ActiveState
        {
            get { return GameState.TicTacToe; }
        }

        private NetcodeHooks _netcodeHooks;
        private NetworkTicTacToe _networkTicTacToe;
        private bool _isOurTurn = false;

        private GameHUD GameHudUI =>
            TicTacToeUIInitializer.Instance != null ? TicTacToeUIInitializer.Instance.HUD : null;

        protected override void Awake()
        {
            base.Awake();

            Instance = this;

            _netcodeHooks = GetComponent<NetcodeHooks>();
            _networkTicTacToe = GetComponent<NetworkTicTacToe>();
            _netcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            _netcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_netcodeHooks)
            {
                _netcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
                _netcodeHooks.OnNetworkDespawnHook -= OnNetworkDespawn;
            }
        }

        protected override void Start()
        {
            base.Start();
            _isOurTurn = _networkTicTacToe.CurrentPlayerTurn.Value == NetworkManager.Singleton.LocalClientId;
            UpdateUI();
        }

        void OnNetworkDespawn()
        {
            if (_networkTicTacToe)
            {
                _networkTicTacToe.CurrentPlayerTurn.OnValueChanged -= OnPlayerTurnChanged;
                _networkTicTacToe.OnServerUpdateGridValue -= SetGridEntityValue;
                _networkTicTacToe.CurrentSessionState.OnValueChanged -= OnGameSessionStateChanged;
            }
        }

        void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                enabled = false;
            }
            else
            {
                _networkTicTacToe.CurrentPlayerTurn.OnValueChanged += OnPlayerTurnChanged;
                _networkTicTacToe.OnServerUpdateGridValue += SetGridEntityValue;
                _networkTicTacToe.CurrentSessionState.OnValueChanged += OnGameSessionStateChanged;

                GetPlayerData();
            }
        }

        private async void GetPlayerData()
        {
            if (GameHudUI == null)
            {
                while (GameHudUI == null)
                {
                    await Task.Delay(100);
                }
            }

            Debug.LogWarning("GameHUD not null");
            GameMarkType? markType = _networkTicTacToe.GetMarkType(NetworkManager.Singleton.LocalClientId);
            if (markType.HasValue)
            {
                GameHudUI.PlayerSideText = $"Your side is: {markType.Value}";
            }
        }

        public void SetGridEntityValue(Coord coord, GameMarkType mark)
        {
            if (_gameField != null)
            {
                _gameField.SetGridEntityValue(coord, mark);
            }
        }

        private void OnGameSessionStateChanged(NetworkTicTacToe.SessionState previousvalue,
            NetworkTicTacToe.SessionState newvalue)
        {
            if (newvalue == NetworkTicTacToe.SessionState.GameFinished)
            {
                GameHudUI.TurnText = "Sit tight! We are calculating results";
            }
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponentInHierarchy<UIDocument>();
            builder.RegisterEntryPoint<TicTacToeUIInitializer>();
        }

        private void OnPlayerTurnChanged(ulong previousPlayerTurn, ulong newPlayerTurn)
        {
            _isOurTurn = newPlayerTurn == NetworkManager.Singleton.LocalClientId;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_gameField == null)
            {
                return;
            }

            if (_isOurTurn)
            {
                Debug.Log("yay its our turn");
                //unlock ui 
                GameHudUI.TurnText = "Make your move!";
                _gameField.EnableEmptyGridEntities();
            }
            else
            {
                //not our turn
                Debug.Log("waiting for other player to make move");
                //lock  ui and set message of who's turn it is
                GameHudUI.TurnText = "Sit tight! Your opponent is planning his move";
                _gameField.DisableAllGridEntities();
            }
        }

        public void MakeMove(Coord coord)
        {
            if (_networkTicTacToe.IsSpawned)
            {
                _networkTicTacToe.MakeMoveServerRpc(NetworkManager.Singleton.LocalClientId, coord);
            }
        }

        public void QuitGame()
        {
            //TODO: do smth about it
            Debug.Log("WantToQuit");
        }
    }
}
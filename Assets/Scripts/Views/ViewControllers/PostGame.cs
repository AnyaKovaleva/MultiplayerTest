using System;
using ConnectionManagement;
using Enums;
using Gameplay.GameState;
using Interfaces.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;
using VContainer;
using VContainer.Unity;
using Views.Views;
using SortingLayer = Enums.UI.SortingLayer;

namespace Views.ViewControllers
{
    public class PostGame : MonoBehaviour
    {
        [SerializeField] private NetcodeHooks _netcodeHooks;
        [SerializeField] private UIDocument _document;
        public SortingLayer SortingLayer { get; }

        private PostGameView _view;

        private ServerPostGameState _postGameState;


        [Inject]
        public void InjectDependenciesAndInitialize(ServerPostGameState serverPostGameState)
        {
            _postGameState = serverPostGameState;
        }

        private void InitButtonEvents()
        {
            _view.RestartButton.clicked += _postGameState.PlayAgain;
            _view.ToMainMenuButton.clicked += _postGameState.GoToMainMenu;
        }

        public void Awake()
        {
            _view = new PostGameView(_document);
            InitButtonEvents();

            _netcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            _netcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;

            if (_view == null)
            {
                Debug.LogError("view os NULL");
            }
            else if (_view.Root == null)
            {
                Debug.LogError("view ROOT os NULL");
            }

            _postGameState.NetworkPostGame.GameResultState.OnValueChanged += OnGameStateChanged;
        }

        private void OnDestroy()
        {
            _netcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
            _netcodeHooks.OnNetworkDespawnHook -= OnNetworkDespawn;
        }

        private void OnNetworkSpawn()
        {
            Debug.Log("network spawned");
            if (!NetworkManager.Singleton.IsClient)
            {
                Debug.Log("we are  server so disableing");
                enabled = false;
            }
            else
            {
                Debug.Log("doingshiton client");

                _view.WaitForHostLabel.style.display =
                    NetworkManager.Singleton.IsHost ? DisplayStyle.None : DisplayStyle.Flex;
                _view.RestartButton.style.display =
                    NetworkManager.Singleton.IsHost ? DisplayStyle.Flex : DisplayStyle.None;
                SetPostGameUI(_postGameState.NetworkPostGame.GameResultState.Value);
            }
        }

        public void OnNetworkDespawn()
        {
            if (_postGameState != null)
            {
                if (_postGameState.NetworkPostGame != null)
                    _postGameState.NetworkPostGame.GameResultState.OnValueChanged -= OnGameStateChanged;
            }
        }

        void OnGameStateChanged(GameResultState previousValue, GameResultState newValue)
        {
            Debug.Log("log game state changed to " + newValue);
            SetPostGameUI(newValue);
        }

        void SetPostGameUI(GameResultState winState)
        {
            if (_view == null)
            {
                Debug.LogError("trying to set postgame UI but _view is null");
            }

            switch (winState)
            {
                case GameResultState.Draw:
                    _view.GameResultLabel.style.color = Color.white;
                    _view.GameResultLabel.text = "Its a DRAW!";
                    return;
                case GameResultState.Invalid:
                    Debug.LogWarning("PostGameUI encountered Invalid WinState");
                    return;
            }

            try
            {
                var playerData = _postGameState.GetPlayerDataServerRpc(NetworkManager.Singleton.LocalClientId);
                if (playerData == null)
                {
                    Debug.LogError("client datafrom server is null");
                    return;
                }

                GameMarkType ourMark = playerData.Value.MarkType;
                GameMarkType winMark = winState == GameResultState.X_Won ? GameMarkType.X : GameMarkType.O;
                bool weWon = ourMark == winMark;

                string color = weWon ? "green" : "red";
                string message = weWon
                    ? "<color=green>Congratulations! You've WON!!!"
                    : "<color=red>Oh no... you've lost((";

                _view.GameResultLabel.text = $"{winState}\n{message}";
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
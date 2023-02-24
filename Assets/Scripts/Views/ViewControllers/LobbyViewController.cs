using System;
using ConnectionManagement;
using Infrastructure;
using Infrastructure.PubSub;
using Interfaces.UI;
using QFSW.QC;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UIElements;
using UnityServices.Auth;
using UnityServices.Lobbies;
using UnityServices.Lobbies.Messages;
using VContainer;
using Views.Views;
using Random = UnityEngine.Random;
using SortingLayer = Enums.UI.SortingLayer;

namespace Views.ViewControllers
{
    public class LobbyViewController : ViewController, IView, IDisposable
    {
        public SortingLayer SortingLayer => SortingLayer.MAIN_MENU;

        private LobbyView _view;
        private CreateLobbyView _createLobbyPopup;

        AuthenticationServiceFacade _authenticationServiceFacade;
        LobbyServiceFacade _lobbyServiceFacade;
        LocalLobbyUser _localUser;
        LocalLobby _localLobby;
        ConnectionManager _connectionManager;
        ISubscriber<ConnectStatus> _connectStatusSubscriber;
        UpdateRunner _updateRunner;
        ISubscriber<LobbyListFetchedMessage> _localLobbiesRefreshedSub;

        const string _defaultLobbyName = "no-name";

        private static LobbyViewController _instance; //to use static functions in  QuantumConsole

        [Inject]
        void InjectDependenciesAndInitialize(
            AuthenticationServiceFacade authenticationServiceFacade,
            LobbyServiceFacade lobbyServiceFacade,
            LocalLobbyUser localUser,
            LocalLobby localLobby,
            ISubscriber<ConnectStatus> connectStatusSub,
            ConnectionManager connectionManager,
            UpdateRunner updateRunner,
            ISubscriber<LobbyListFetchedMessage> localLobbiesRefreshedSub
        )
        {
            _authenticationServiceFacade = authenticationServiceFacade;
            _localUser = localUser;
            _lobbyServiceFacade = lobbyServiceFacade;
            _localLobby = localLobby;
            _connectionManager = connectionManager;
            _connectStatusSubscriber = connectStatusSub;

            _connectStatusSubscriber.Subscribe(OnConnectStatus);

            _updateRunner = updateRunner;
            _localLobbiesRefreshedSub = localLobbiesRefreshedSub;
            
            _localLobbiesRefreshedSub.Subscribe(UpdateLobbyList);
        }

        public override void InjectDependenciesAndInitialize(UIDocument document)
        {
            _view = new LobbyView(document);
            _createLobbyPopup = new CreateLobbyView(_view.Root);
            base.Initialize(_view);
            
            RegenerateName();

            _instance = this;
        }

        public override void Open()
        {
            base.Open();
            _updateRunner.Subscribe(PeriodicRefresh, 10f);
        }

        public override void Close()
        {
            base.Close();
            _updateRunner.Unsubscribe(PeriodicRefresh);
        }

        void OnConnectStatus(ConnectStatus status)
        {
            Debug.Log("connection status " + status);
            if (status is ConnectStatus.GenericDisconnect or ConnectStatus.StartClientFailed)
            {
                //UnblockUIAfterLoadingIsComplete();
            }
        }

        protected override void InitButtonEvents()
        {
            _view.CreateGameButton.clicked += CreateGame;
            _view.JoinGameButton.clicked += JoinGame;

            _createLobbyPopup.CreateLobbyButton.clicked += () =>
            {
                CreateLobbyRequest(_createLobbyPopup.LobbyNameInputField.value,
                    _createLobbyPopup.IsPrivateToggle.value);
                _createLobbyPopup.Close();
            };
        }

        private void CreateGame()
        {
            _createLobbyPopup.Open();
        }

        private void JoinGame()
        {
            QuickJoin();
        }

        private async void QuickJoin()
        {
            BlockUIWhileLoadingIsInProgress();

            bool playerIsAuthorized = await _authenticationServiceFacade.EnsurePlayerIsAuthorized();

            if (!playerIsAuthorized)
            {
                UnblockUIAfterLoadingIsComplete();
                return;
            }

            var result = await _lobbyServiceFacade.TryQuickJoinLobbyAsync();

            if (result.Success)
            {
                OnJoinedLobby(result.Lobby);
            }
            else
            {
                UnblockUIAfterLoadingIsComplete();
            }
        }

        [Command("join-with-code")]
        public static void JoinWithCodeCommand(string lobbyCode)
        {
            Debug.Log("join lobby command called");
            _instance.JoinLobbyWithCodeRequest(lobbyCode);
        }
        
        public async void JoinLobbyWithCodeRequest(string lobbyCode)
        {
            Debug.Log("lobbycode "+ lobbyCode);
            BlockUIWhileLoadingIsInProgress();

            bool playerIsAuthorized = await _authenticationServiceFacade.EnsurePlayerIsAuthorized();

            if (!playerIsAuthorized)
            {
                UnblockUIAfterLoadingIsComplete();
                return;
            }

            var result = await _lobbyServiceFacade.TryJoinLobbyAsync(null, lobbyCode);

            if (result.Success)
            {
                OnJoinedLobby(result.Lobby);
            }
            else
            {
                UnblockUIAfterLoadingIsComplete();
            }
        }

        void OnJoinedLobby(Unity.Services.Lobbies.Models.Lobby remoteLobby)
        {
            _lobbyServiceFacade.SetRemoteLobby(remoteLobby);

            Debug.Log($"Joined lobby with code: {_localLobby.LobbyCode}, Internal Relay Join Code{_localLobby.RelayJoinCode}");
            _connectionManager.StartClientLobby(_localUser.DisplayName);
        }
        
        public void RegenerateName()
        {
            _localUser.DisplayName = Random.Range(1, 1000).ToString();
            _view.PlayerNameLabel.text = _localUser.DisplayName;
        }

        public void Dispose()
        {
            Debug.LogWarning("Disposing Lobby");
            _connectStatusSubscriber?.Unsubscribe(OnConnectStatus);
            if (_updateRunner != null)
            {
                _updateRunner?.Unsubscribe(PeriodicRefresh);
            }

            _localLobbiesRefreshedSub?.Unsubscribe(UpdateLobbyList);
        }

        void UpdateLobbyList(LobbyListFetchedMessage message)
        {
            if (_view == null)
            {
                Debug.LogError("Lobby view is null but trying to send update lobby  list  event");
                return;
            }
            //EnsureNumberOfActiveUISlots(message.LocalLobbies.Count);

            string lobbies = "";
            for (var i = 0; i < message.LocalLobbies.Count; i++)
            {
                //var localLobby = message.LocalLobbies[i];
                //m_LobbyListItems[i].SetData(localLobby);
                lobbies +=
                    $"Lobby #{i}\nName:  {message.LocalLobbies[i].LobbyName}\nID:  {message.LocalLobbies[i].LobbyID}\n Players {message.LocalLobbies[i].PlayerCount}/{message.LocalLobbies[i].MaxPlayerCount}\n\n\n";
            }

            if (message.LocalLobbies.Count == 0)
            {
                //m_EmptyLobbyListLabel.enabled = true;
                lobbies = "no active lobbies";
            }
            // else
            // {
            //     m_EmptyLobbyListLabel.enabled = false;
            // }

            _view.LobbiesLabel.text = lobbies;
        }
        
        void PeriodicRefresh(float _)
        {
            Debug.Log("Refreshing");
            //this is a soft refresh without needing to lock the UI and such
            QueryLobbiesRequest(false);
        }
        
        public async void CreateLobbyRequest(string lobbyName, bool isPrivate)
        {
            // before sending request to lobby service, populate an empty lobby name, if necessary
            if (string.IsNullOrEmpty(lobbyName))
            {
                lobbyName = _defaultLobbyName;
            }

            BlockUIWhileLoadingIsInProgress();

            bool playerIsAuthorized = await _authenticationServiceFacade.EnsurePlayerIsAuthorized();

            if (!playerIsAuthorized)
            {
                UnblockUIAfterLoadingIsComplete();
                return;
            }

            var lobbyCreationAttempt = await _lobbyServiceFacade.TryCreateLobbyAsync(lobbyName, _connectionManager.MaxConnectedPlayers, isPrivate);

            if (lobbyCreationAttempt.Success)
            {
                _localUser.IsHost = true;
                _lobbyServiceFacade.SetRemoteLobby(lobbyCreationAttempt.Lobby);

                Debug.Log($"Created lobby with ID: {_localLobby.LobbyID} and code {_localLobby.LobbyCode}");
                _connectionManager.StartHostLobby(_localUser.DisplayName);
            }
            else
            {
                UnblockUIAfterLoadingIsComplete();
            }
        }
        
        public async void QueryLobbiesRequest(bool blockUI)
        {
            if (Unity.Services.Core.UnityServices.State != ServicesInitializationState.Initialized)
            {
                return;
            }

            if (blockUI)
            {
                BlockUIWhileLoadingIsInProgress();
            }

            bool playerIsAuthorized = await _authenticationServiceFacade.EnsurePlayerIsAuthorized();

            if (blockUI && !playerIsAuthorized)
            {
                UnblockUIAfterLoadingIsComplete();
                return;
            }

            await _lobbyServiceFacade.RetrieveAndPublishLobbyListAsync();

            if (blockUI)
            {
                UnblockUIAfterLoadingIsComplete();
            }
        }
        
        void BlockUIWhileLoadingIsInProgress()
        {
            Debug.Log("Should block UI");
            // m_CanvasGroup.interactable = false;
            // m_LoadingSpinner.SetActive(true);
        }

        void UnblockUIAfterLoadingIsComplete()
        {
            //this callback can happen after we've already switched to a different scene
            //in that case the canvas group would be null
            
            Debug.Log("Unblocking UI");

            // if (m_CanvasGroup != null)
            // {
            //     m_CanvasGroup.interactable = true;
            //     m_LoadingSpinner.SetActive(false);
            // }
        }
    }
}
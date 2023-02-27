using System.Collections.Generic;
using ConnectionManagement;
using Gameplay.GameState;
using Interfaces.UI;
using Unity.Multiplayer.Samples.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityServices.Lobbies;
using VContainer;
using Views.Components;
using Views.Views;
using SortingLayer = Enums.UI.SortingLayer;

namespace Views.ViewControllers
{
    public class ChooseSide : ViewController, IView
    {
        public SortingLayer SortingLayer => SortingLayer.CHOOSE_SIDE;

        public string NumPlayersText
        {
            set { _view.NumPlayersLabel.text = value; }
        }

        private ChooseSideView _view;

        [Inject] private ClientChooseSideState _chooseSide;
        [Inject] private ConnectionManager _connectionManager;
        [Inject] private LocalLobby _localLobby;

        public List<ChooseSideSeat> _seats { get; private set; } = new List<ChooseSideSeat>();

        public override void InjectDependenciesAndInitialize(UIDocument document)
        {
            _view = new ChooseSideView(document);
            base.Initialize(_view);

            _view.JoinCodeLabel.text = _localLobby.LobbyCode;
            
            _seats.Add(new ChooseSideSeat(_view.XButton, NetworkChooseSide.SeatType.X));
            _seats.Add(new ChooseSideSeat(_view.OButton, NetworkChooseSide.SeatType.O));
        }

        public void SetMessageText(string text)
        {
            _view.MessageLabel.text = text;
        }

        public void UpdateLobbyState(ClientChooseSideState.LobbyMode newLobbyMode)
        {
            _view.LobbyStateLabel.text = $"Curren lobby mode is:\n <b>{newLobbyMode}";
            // that finishes the easy bit. Next, each lobby mode might also need to configure the lobby seats and class-info box.
            bool isSeatsDisabledInThisMode = false;
            switch (newLobbyMode)
            {
                case ClientChooseSideState.LobbyMode.ChooseSeat:
                    _view.ReadyButton.text = "READY!";
                    SetMessageText("Choose your side!");
                    break;
                case ClientChooseSideState.LobbyMode.SeatChosen:
                    isSeatsDisabledInThisMode = true;
                    SetMessageText("Waiting for other players");
                   // m_ClassInfoBox.SetLockedIn(true);
                    _view.ReadyButton.text = "UNREADY";
                    break;
                case ClientChooseSideState.LobbyMode.FatalError:
                    isSeatsDisabledInThisMode = true;
                    //m_ClassInfoBox.ConfigureForNoSelection();
                    break;
                case ClientChooseSideState.LobbyMode.LobbyEnding:
                    isSeatsDisabledInThisMode = true;
                    SetMessageText("Starting the game!");
                    //m_ClassInfoBox.ConfigureForNoSelection();
                    break;
            }

            // go through all our seats and enable or disable buttons
            foreach (var seat in _seats)
            {
                // disable interaction if seat is already locked or all seats disabled
                seat.SetDisableInteractions(seat.IsLocked() || isSeatsDisabledInThisMode);
            }
        }
        protected override void InitButtonEvents()
        {
            _view.CopyCodeButton.clicked += CopyCodeToClipboard;
            _view.QuitButton.clicked += QuitLobby;
            _view.ReadyButton.clicked += PlayerReady;
        }

        private void CopyCodeToClipboard()
        {
            GUIUtility.systemCopyBuffer = _view.JoinCodeLabel.text;
        }

        private void QuitLobby()
        {
            Debug.Log("clicked quit button");
            _connectionManager.RequestShutdown();
        }

        private void PlayerReady()
        {
            _chooseSide.OnPlayerClickedReady();
        }

    }
}
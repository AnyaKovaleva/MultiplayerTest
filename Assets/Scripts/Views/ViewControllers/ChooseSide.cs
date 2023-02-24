using ConnectionManagement;
using Gameplay.GameState;
using Interfaces.UI;
using Unity.Multiplayer.Samples.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityServices.Lobbies;
using VContainer;
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
        [Inject] protected LocalLobby _localLobby;
        
        public override void InjectDependenciesAndInitialize(UIDocument document)
        {
            _view = new ChooseSideView(document);
            base.Initialize(_view);

            _view.JoinCodeLabel.text = _localLobby.LobbyCode;
        }

        public void SetMessageText(string text)
        {
            _view.MessageLabel.text = text;
        }

        public void UpdateLobbyState(ClientChooseSideState.LobbyMode newLobbyMode)
        {
            _view.LobbyStateLabel.text = $"Curren lobby mode is:\n <b>{newLobbyMode}";
        }
        protected override void InitButtonEvents()
        {
            _view.CopyCodeButton.clicked += CopyCodeToClipboard;
            _view.QuitButton.clicked += QuitLobby;
            _view.XButton.clicked += ChooseX;
            _view.OButton.clicked += ChooseO;
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
           // SceneManager.LoadScene("MainMenu");
        }

        private void ChooseX()
        {
            _chooseSide.OnPlayerClickedSeat(1);
        }

        private void ChooseO()
        {
            _chooseSide.OnPlayerClickedSeat(2);
        }

        private void PlayerReady()
        {
            _chooseSide.OnPlayerClickedReady();
        }

    }
}
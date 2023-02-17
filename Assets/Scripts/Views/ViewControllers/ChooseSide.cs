using Gameplay.GameState;
using Interfaces.UI;
using Unity.Multiplayer.Samples.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using VContainer;
using Views.Views;
using SortingLayer = Enums.UI.SortingLayer;

namespace Views.ViewControllers
{
    public class ChooseSide : ViewController, IView
    {
        public SortingLayer SortingLayer => SortingLayer.CHOOSE_SIDE;

        private ChooseSideView _view;

        [Inject] private ClientChooseSideState _chooseSide;
        
        public override void InjectDependenciesAndInitialize(UIDocument document)
        {
            _view = new ChooseSideView(document);
            base.Initialize(_view);
        }

        public void SetMessageText(string text)
        {
            _view.MessageLabel.text = text;
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
        {}

        private void QuitLobby()
        {
            Debug.Log("clicked quit button");
            SceneManager.LoadScene("MainMenu");
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
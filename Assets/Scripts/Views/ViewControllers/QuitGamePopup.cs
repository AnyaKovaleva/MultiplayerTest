using Enums.UI;
using Gameplay.GameState;
using Interfaces.UI;
using UnityEngine.UIElements;
using VContainer;
using Views.Views;

namespace Views.ViewControllers
{
    public class QuitGamePopup : ViewController, IView
    {
        public SortingLayer SortingLayer => SortingLayer.POPUPS;

        private QuitGamePopupView _view;

        [Inject] private ClientTicTacToeState _clientTicTacToe;
        
        public override void InjectDependenciesAndInitialize(UIDocument document)
        {
            _view = new QuitGamePopupView(document);
            base.Initialize(_view, ViewType.POPUP);
        }

        protected override void InitButtonEvents()
        {
            _view.CancelButton.clicked += ReturnToGame;
            _view.QuitButton.clicked += Quit;
        }

        private void Quit()
        {
            _clientTicTacToe.QuitGame();
        }

        private void ReturnToGame()
        {
            ViewsController.Return();
        }

    }
}
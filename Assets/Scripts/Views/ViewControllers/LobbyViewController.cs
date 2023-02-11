using Enums.UI;
using Interfaces.UI;
using UnityEngine.UIElements;
using VContainer;
using Views.Views;

namespace Views.ViewControllers
{
    public class LobbyViewController : ViewController, IView
    {
        public SortingLayer SortingLayer => SortingLayer.MAIN_MENU;

        private LobbyView _view;

        public override void InjectDependenciesAndInitialize(UIDocument document)
        {
            _view = new LobbyView(document);
            base.Initialize(_view);
        }

        protected override void InitButtonEvents()
        {
            _view.CreateGameButton.clicked += CreateGame;
            _view.JoinGameButton.clicked += JoinGame;
        }
        
        private void CreateGame()
        {}
        
        private void JoinGame()
        {}
    }
}
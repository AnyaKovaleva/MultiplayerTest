using Enums.UI;
using Gameplay.GameState;
using Interfaces.UI;
using UnityEngine.UIElements;
using VContainer;
using Views.Views;

namespace Views.ViewControllers
{
    public class GameHUD : ViewController, IView
    {
        public SortingLayer SortingLayer => SortingLayer.HUD;

        public string PlayerSideText
        {
            set { _view.PlayerSideLabel.text = value; }
        }
        
        public string TurnText
        {
            set { _view.TurnLabel.text = value; }
        }
        
        private GameHUDView _view;

        
        public override void InjectDependenciesAndInitialize(UIDocument document)
        {
            _view = new GameHUDView(document);
            base.Initialize(_view);
        }

        protected override void InitButtonEvents()
        {
            _view.QuitButton.clicked += () => ViewsController.Open(typeof(QuitGamePopup));
        }

    }
}
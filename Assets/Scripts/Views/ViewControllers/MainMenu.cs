using Enums.UI;
using Interfaces.UI;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using Views.Views;
using SortingLayer = Enums.UI.SortingLayer;

namespace Views.ViewControllers
{
    public class MainMenu : ViewController, IView
    {
        public SortingLayer SortingLayer => SortingLayer.MAIN_MENU;

        private MainMenuView _view;
        
        public override void InjectDependenciesAndInitialize(UIDocument document)
        {
            _view = new MainMenuView(document);
            base.Initialize(_view);
        }

        protected override void InitButtonEvents()
        {
            _view.PlayButton.clicked += OpenLobbyPanel;
            _view.ChangeProfileButton.clicked += OpenChangeProfilePanel;
        }

        public void OpenLobbyPanel()
        {
            Debug.Log("Pressing lobby");
            ViewsController.Open(typeof(LobbyViewController));
        }

        public void OpenChangeProfilePanel()
        {
            Debug.Log("Profile");
            ViewsController.Open(typeof(Profile));
        }
    }
}
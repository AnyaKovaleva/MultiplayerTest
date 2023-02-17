using System;
using System.Collections.Generic;
using Interfaces.UI;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;
using Views;
using Views.ViewControllers;

namespace Initializers
{
    public class MainMenuUIInitializer : UIInitializer
    {
        private MainMenu _mainMenuView = new MainMenu();
        private LobbyViewController _lobbyView = new LobbyViewController();
        private Profile _profileView = new Profile();

        protected override void InjectDependencies()
        {
            Inject(_mainMenuView);
            Inject(_lobbyView);
            Inject(_profileView);
        }

        protected override void InitializeViewsController()
        {
            ViewsController.Initialize(new List<IView>()
            {
                _mainMenuView,
                _lobbyView,
                _profileView
            }, new List<SortingLayerView>());
        }

        protected override void OpenStartView()
        {
            ViewsController.Open(typeof(MainMenu));
        }

        public override void Dispose()
        {
            Debug.Log("Disposing MainMenu UI Initializer");
            _lobbyView.Dispose();
            _profileView.Dispose();
        }
    }
}
using System;
using System.Collections.Generic;
using Interfaces.UI;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;
using Views;
using Views.ViewControllers;

namespace Initializers
{
    public class MainMenuUIInitializer : IStartable
    {
        private MainMenu _mainMenuView = new MainMenu();
        private LobbyViewController _lobbyView = new LobbyViewController();
        private Profile _profileView = new Profile();

        [Inject] LifetimeScope serviceScope;

        // [Inject]
        // private void Construct()
        // {
        //   //  builder.RegisterBuildCallback(InjectViews);
        // }

        public void Start()
        {
            InitializeViews();
            ViewsController.Open(typeof(MainMenu));
        }

        private void InitializeViews()
        {
            InjectDependencies();

            ViewsController.Initialize(new List<IView>()
            {
                _mainMenuView,
                _lobbyView,
                _profileView
            }, new List<SortingLayerView>());
            
        }

        private void InjectDependencies()
        {
            serviceScope.Container.Inject(_mainMenuView);
            serviceScope.Container.Inject(_lobbyView);
            serviceScope.Container.Inject(_profileView);
        }
    }
}
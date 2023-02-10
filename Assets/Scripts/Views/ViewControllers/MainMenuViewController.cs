using System;
using Enums.UI;
using Interfaces.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UnityServices.Lobbies;
using VContainer;
using Views.Views;
using SortingLayer = Enums.UI.SortingLayer;

namespace Views.ViewControllers
{
    public class MainMenuViewController :  IView
    {
        public SortingLayer SortingLayer => SortingLayer.MAIN_MENU;
        public ViewType Type => ViewType.PANEL;

        private MainMenuView _view;

        [Inject] private UIDocument _uiDocument;
        [Inject] private SomeCoolClass _someCoolClass;

        [Inject] private LocalLobby _lobby;
        
        public MainMenuViewController()
        {
            // _view = new MainMenuView(_uiDocument);
        
            // InitButtonEvents();
            // Debug.Log(_lobby.LobbyName);
            // _someCoolClass.DoStuff();
        
        }
        private void Start()
        {
            Open();
        }

        public void Open()
        {
            _someCoolClass.DoStuff();

           // Debug.Log(_lobby.LobbyName);
            _view.Root.style.display = DisplayStyle.Flex;
        }

        public void Close()
        {
            _view.Root.style.display = DisplayStyle.None;
        }

        public void InitButtonEvents()
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
            ViewsController.Open(typeof(ProfileViewController));
        }
        public void InitLocalization()
        {
            throw new System.NotImplementedException();
        }
    }
}
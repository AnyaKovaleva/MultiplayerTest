using System;
using Interfaces.UI;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;
using VContainer;
using Views.Views;
using SortingLayer = Enums.UI.SortingLayer;

namespace Views.ViewControllers
{
    public class Profile : ViewController, IView, IDisposable
    {
        public SortingLayer SortingLayer => SortingLayer.MAIN_MENU;

        private ProfileView _view;

        [Inject] private ProfileManager _profileManager;
        
        public override void InjectDependenciesAndInitialize(UIDocument document)
        {
            _view = new ProfileView(document);
            _profileManager.onProfileChanged += UpdateUsernameLabel;
            base.Initialize(_view);
        }

        protected override void InitButtonEvents()
        {
            _view.SignInButton.clicked += SignIn;
            _view.SignUpButton.clicked += SignUp;
        }

        private void SignIn()
        {
            
        }

        private void SignUp()
        {
            
        }

        private void UpdateUsernameLabel()
        {
            Debug.Log("profile updated");
            _view.Username.text = _profileManager.Profile;
        }

        public void Dispose()
        {
            _profileManager.onProfileChanged -= UpdateUsernameLabel;
        }
    }
}
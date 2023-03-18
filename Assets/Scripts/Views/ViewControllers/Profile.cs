using System;
using Interfaces.UI;
using QFSW.QC;
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

        public static Profile Instance;
        
        public override void InjectDependenciesAndInitialize(UIDocument document)
        {
            _view = new ProfileView(document);
            _profileManager.onProfileChanged += UpdateUsernameLabel;
            base.Initialize(_view);
            Instance = this;
        }

        protected override void InitButtonEvents()
        {
            _view.SignInButton.clicked += SignIn;
            _view.SignUpButton.clicked += SignUp;
        }

        [Command("get-profiles")]
        public static void GetProfiles()
        {
            foreach (var profile in Instance._profileManager.AvailableProfiles)
            {
                Debug.Log(profile);
            }
        }

        [Command("create-profile")]
        public static void CreateProfile(string profile)
        {
            Instance._profileManager.CreateProfile(profile);
        }

        [Command("profile")]
        public static void GetProfile()
        {
            Debug.Log(Instance._profileManager.Profile);
        }

        [Command("set-profile")]
        public static void SetProfile(string profile)
        {
            Instance._profileManager.Profile = profile;
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
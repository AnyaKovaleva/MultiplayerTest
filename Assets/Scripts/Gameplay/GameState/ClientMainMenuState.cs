using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces.UI;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityServices.Auth;
using UnityServices.Lobbies;
using Utils;
using VContainer;
using VContainer.Unity;
using Views;
using Views.ViewControllers;

public class SomeCoolClass
{
    public void DoStuff()
    {
        Debug.Log("Do stuff");
    }
}

namespace Gameplay.GameState
{
    /// <summary>
    /// Game Logic that runs when sitting at the MainMenu. This is likely to be "nothing", as no game has been started. But it is
    /// nonetheless important to have a game state, as the GameStateBehaviour system requires that all scenes have states.
    /// </summary>
    /// <remarks> OnNetworkSpawn() won't ever run, because there is no network connection at the main menu screen.
    /// Fortunately we know you are a client, because all players are clients when sitting at the main menu screen.
    /// </remarks>
    public class ClientMainMenuState : GameStateBehaviour
    {
        public override global::Gameplay.GameState.GameState ActiveState
        {
            get { return GameState.MainMenu; }
        }

        // [SerializeField] NameGenerationData m_NameGenerationData;
        // [SerializeField] LobbyUIMediator m_LobbyUIMediator;
        // [SerializeField] IPUIMediator m_IPUIMediator;
        // [SerializeField] Button m_LobbyButton;
        // [SerializeField] GameObject m_SignInSpinner;
        // [SerializeField] UIProfileSelector m_UIProfileSelector;
        // [SerializeField] UITooltipDetector m_UGSSetupTooltipDetector;

        [SerializeField] private UIDocument _uiDocument;

        [Inject] AuthenticationServiceFacade m_AuthServiceFacade;
        [Inject] LocalLobbyUser m_LocalUser;
        [Inject] LocalLobby m_LocalLobby;
        [Inject] ProfileManager m_ProfileManager;

        private MainMenuViewController _mainMenuView = new MainMenuViewController();
        private LobbyViewController _lobbyView = new LobbyViewController();
        private ProfileViewController _profileView = new ProfileViewController();
        
        protected override void Awake()
        {
            base.Awake();

            // m_LobbyButton.interactable = false;
            // m_LobbyUIMediator.Hide();
            //InitUI();
            //ViewsController.Open(typeof(MainMenuViewController));

            if (string.IsNullOrEmpty(Application.cloudProjectId))
            {
                OnSignInFailed();
                return;
            }
            
            // TrySignIn();
        }

        protected override void Start()
        {
            base.Start();

           // MainMenuViewController menuViewController = new MainMenuViewController();
            //menuViewController.Open();
            Wait();
        }

        private async Task Wait()
        {
            await Task.Delay(1000);
            _mainMenuView.Open();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponentInHierarchy<UIDocument>();
            builder.Register<SomeCoolClass>(Lifetime.Singleton);
            builder.RegisterInstance(_mainMenuView);

            // builder.RegisterComponent(m_LobbyUIMediator);
            // builder.RegisterComponent(m_IPUIMediator);
        }

        private void InitUI()
        {
            // ViewsController.Initialize(
            //     new List<IView>
            //         { _mainMenuView, _profileView },
            //     new List<SortingLayerView>());
        }

        private async void TrySignIn()
        {
            try
            {
                var unityAuthenticationInitOptions = new InitializationOptions();
                var profile = m_ProfileManager.Profile;
                if (profile.Length > 0)
                {
                    unityAuthenticationInitOptions.SetProfile(profile);
                }

                await m_AuthServiceFacade.InitializeAndSignInAsync(unityAuthenticationInitOptions);
                OnAuthSignIn();
                m_ProfileManager.onProfileChanged += OnProfileChanged;
            }
            catch (Exception)
            {
                OnSignInFailed();
            }
        }

        private void OnAuthSignIn()
        {
            // m_LobbyButton.interactable = true;
            // m_UGSSetupTooltipDetector.enabled = false;
            // m_SignInSpinner.SetActive(false);

            Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");

            m_LocalUser.ID = AuthenticationService.Instance.PlayerId;
            // The local LobbyUser object will be hooked into UI before the LocalLobby is populated during lobby join, so the LocalLobby must know about it already when that happens.
            m_LocalLobby.AddUser(m_LocalUser);
        }

        private void OnSignInFailed()
        {
            Debug.Log("On sign in failed");
            // if (m_LobbyButton)
            // {
            //     m_LobbyButton.interactable = false;
            //     m_UGSSetupTooltipDetector.enabled = true;
            // }
            // if (m_SignInSpinner)
            // {
            //     m_SignInSpinner.SetActive(false);
            // }
        }

        protected override void OnDestroy()
        {
            m_ProfileManager.onProfileChanged -= OnProfileChanged;
            base.OnDestroy();
        }

        async void OnProfileChanged()
        {
            // m_LobbyButton.interactable = false;
            // m_SignInSpinner.SetActive(true);
            await m_AuthServiceFacade.SwitchProfileAndReSignInAsync(m_ProfileManager.Profile);

            // m_LobbyButton.interactable = true;
            // m_SignInSpinner.SetActive(false);

            Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");

            // Updating LocalUser and LocalLobby
            m_LocalLobby.RemoveUser(m_LocalUser);
            m_LocalUser.ID = AuthenticationService.Instance.PlayerId;
            m_LocalLobby.AddUser(m_LocalUser);
        }

        public void OnStartClicked()
        {
            // m_LobbyUIMediator.ToggleJoinLobbyUI();
            // m_LobbyUIMediator.Show();
        }

        public void OnDirectIPClicked()
        {
            // m_LobbyUIMediator.Hide();
            // m_IPUIMediator.Show();
        }

        public void OnChangeProfileClicked()
        {
            // m_UIProfileSelector.Show();
        }
    }
}
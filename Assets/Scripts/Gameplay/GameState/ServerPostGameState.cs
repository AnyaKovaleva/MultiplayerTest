using ConnectionManagement;
using Enums;
using Initializers;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;
using VContainer;
using VContainer.Unity;

namespace Gameplay.GameState
{
    [RequireComponent(typeof(NetcodeHooks), typeof(NetworkPostGame))]
    public class ServerPostGameState : GameStateBehaviour
    {
        public override GameState ActiveState => GameState.PostGame;

        private NetcodeHooks _netcodeHooks;
        private NetworkPostGame _networkPostGame;

        public NetworkPostGame NetworkPostGame => _networkPostGame;

        [Inject] private ConnectionManager _connectionManager;
        [Inject] private PersistentGameState _persistentGameState;

        protected override void Awake()
        {
            base.Awake();

            _netcodeHooks = GetComponent<NetcodeHooks>();
            _networkPostGame = GetComponent<NetworkPostGame>();

            _netcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
        }


        void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                enabled = false;
            }
            else
            {
                _networkPostGame.GameResultState.Value = _persistentGameState.GameResultState;

                GameMarkType winMark = GameMarkType.NONE;
                switch (_persistentGameState.GameResultState)
                {
                    case GameResultState.X_Won: winMark = GameMarkType.X;
                        break;
                    case GameResultState.O_Won: winMark = GameMarkType.O;
                        break;
                    default:
                        winMark = GameMarkType.NONE;
                        break;
                }
                foreach (var clientID in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    Debug.Log("client id " + clientID);
                    var playerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientID);
                    if (playerData.HasValue)
                    {
                        Debug.Log($"player name: {playerData.Value.PlayerName} seat type: {playerData.Value.MarkType}");
                        _networkPostGame.Players.Add(new NetworkPostGame.PostGamePlayerState(clientID,
                            playerData.Value.PlayerName, playerData.Value.PlayerNumber, playerData.Value.MarkType,
                            playerData.Value.MarkType == winMark));
                    }
                    else
                    {
                        Debug.LogError("Cant get data of " + clientID);
                    }
                }
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDestroy()
        {
            _persistentGameState.Reset();

            base.OnDestroy();

            if (_netcodeHooks)
                _netcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            //builder.RegisterComponentInHierarchy<UIDocument>();
        }
        

        public void PlayAgain()
        {
            Debug.Log("playing again");
            SessionManager<SessionPlayerData>.Instance.OnSessionEnded();
            SceneLoaderWrapper.Instance.LoadScene("ChooseSideScene", useNetworkSceneManager: true);
        }

        public void GoToMainMenu()
        {
            Debug.Log("requesting shutdown");
            SessionManager<SessionPlayerData>.Instance.OnSessionEnded();
            _connectionManager.RequestShutdown();
        }
    }
}
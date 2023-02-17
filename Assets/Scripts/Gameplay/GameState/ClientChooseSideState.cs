using System.Collections.Generic;
using ConnectionManagement;
using Initializers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;
using VContainer;
using VContainer.Unity;
using Views.ViewControllers;

namespace Gameplay.GameState
{
    [RequireComponent(typeof(NetcodeHooks))]
    public class ClientChooseSideState : GameStateBehaviour
    {
        [SerializeField]
        NetworkChooseSide _networkSideSelection;
        public override GameState ActiveState
        {
            get { return GameState.ChooseSide; }
        }
        
        [SerializeField]
        NetcodeHooks m_NetcodeHooks;

        [Inject] private ConnectionManager _connectionManager;
        
        private ChooseSide _chooseSideUI => ChooseSideUIInitializer.Instance.ChooseSide;
        
        
        int _lastSeatSelected = -1;
        bool _hasLocalPlayerLockedIn = false;
        
        /// <summary>
        /// Conceptual modes or stages that the lobby can be in. We don't actually
        /// bother to keep track of what LobbyMode we're in at any given time; it's just
        /// an abstraction that makes it easier to configure which UI elements should
        /// be enabled/disabled in each stage of the lobby.
        /// </summary>
        public enum LobbyMode
        {
            ChooseSeat, // "Choose your seat!" stage
            SeatChosen, // "Waiting for other players!" stage
            LobbyEnding, // "Get ready! Game is starting!" stage
            FatalError, // "Fatal Error" stage
        }
        protected override void Awake()
        {
            base.Awake();
            
            m_NetcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            m_NetcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;
            
        }

        protected override void Start()
        {
            base.Start();
            // for (int i = 0; i < m_PlayerSeats.Count; ++i)
            // {
            //     m_PlayerSeats[i].Initialize(i);
            // }
            //
            ConfigureUIForLobbyMode(LobbyMode.ChooseSeat);
            UpdateCharacterSelection(NetworkChooseSide.SeatState.Inactive);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponentInHierarchy<UIDocument>();
            builder.RegisterEntryPoint<ChooseSideUIInitializer>();
        }

        void OnNetworkDespawn()
        {
            if (_networkSideSelection)
            {
                _networkSideSelection.IsLobbyClosed.OnValueChanged -= OnLobbyClosedChanged;
                _networkSideSelection.LobbyPlayers.OnListChanged -= OnLobbyPlayerStateChanged;
            }
        }

        void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsClient)
            {
                enabled = false;
            }
            else
            {
                _networkSideSelection.IsLobbyClosed.OnValueChanged += OnLobbyClosedChanged;
                _networkSideSelection.LobbyPlayers.OnListChanged += OnLobbyPlayerStateChanged;
            }
        }
        
        void UpdatePlayerCount()
        {
            int count = _networkSideSelection.LobbyPlayers.Count;
            var pstr = (count > 1) ? "players" : "player";
            Debug.Log("<b>" + count + "</b> " + pstr + " connected");
        }
        
        /// <summary>
        /// Called by the server when any of the seats in the lobby have changed. (Including ours!)
        /// </summary>
        void OnLobbyPlayerStateChanged(NetworkListEvent<NetworkChooseSide.LobbyPlayerState> changeEvent)
        {
            UpdateSeats();
            UpdatePlayerCount();
            
            // now let's find our local player in the list and update the character/info box appropriately
            int localPlayerIdx = -1;
            for (int i = 0; i < _networkSideSelection.LobbyPlayers.Count; ++i)
            {
                if (_networkSideSelection.LobbyPlayers[i].ClientId == NetworkManager.Singleton.LocalClientId)
                {
                    localPlayerIdx = i;
                    break;
                }
            }
            
            if (localPlayerIdx == -1)
            {
                // we aren't currently participating in the lobby!
                // this can happen for various reasons, such as the lobby being full and us not getting a seat.
                UpdateCharacterSelection(NetworkChooseSide.SeatState.Inactive);
            }
            else if (_networkSideSelection.LobbyPlayers[localPlayerIdx].SeatState == NetworkChooseSide.SeatState.Inactive)
            {
                // we haven't chosen a seat yet (or were kicked out of our seat by someone else)
                UpdateCharacterSelection(NetworkChooseSide.SeatState.Inactive);
                // make sure our player num is properly set in Lobby UI
                OnAssignedPlayerNumber(_networkSideSelection.LobbyPlayers[localPlayerIdx].PlayerNumber);
            }
            else
            {
                // we have a seat! Note that if our seat is LockedIn, this function will also switch the lobby mode
                UpdateCharacterSelection(_networkSideSelection.LobbyPlayers[localPlayerIdx].SeatState, _networkSideSelection.LobbyPlayers[localPlayerIdx].SeatIdx);
            }
        }
        
        /// <summary>
        /// Called when our PlayerNumber (e.g. P1, P2, etc.) has been assigned by the server
        /// </summary>
        /// <param name="playerNum"></param>
        void OnAssignedPlayerNumber(int playerNum)
        {
            Debug.Log("player was assigned a number - " + playerNum);
            //m_ClassInfoBox.OnSetPlayerNumber(playerNum);
        }
        
         public void ConfigureUIForLobbyMode(ClientChooseSideState.LobbyMode mode)
        {
            // that finishes the easy bit. Next, each lobby mode might also need to configure the lobby seats and class-info box.
            bool isSeatsDisabledInThisMode = false;
            switch (mode)
            {
                case ClientChooseSideState.LobbyMode.ChooseSeat:
                    if (_lastSeatSelected == -1)
                    {
                        // if (m_CurrentCharacterGraphics)
                        // {
                        //     m_CurrentCharacterGraphics.gameObject.SetActive(false);
                        // }
                        // m_ClassInfoBox.ConfigureForNoSelection();
                    }
                    _chooseSideUI.SetMessageText( "ChooseSeat\nREADY!");
                    break;
                case ClientChooseSideState.LobbyMode.SeatChosen:
                    isSeatsDisabledInThisMode = true;
                    //m_ClassInfoBox.SetLockedIn(true);
                    _chooseSideUI.SetMessageText( "SeatChosen\nUNREADY");
                    break;
                case ClientChooseSideState.LobbyMode.FatalError:
                    isSeatsDisabledInThisMode = true;
                    //m_ClassInfoBox.ConfigureForNoSelection();
                    _chooseSideUI.SetMessageText( "FatalError");
                    break;
                case ClientChooseSideState.LobbyMode.LobbyEnding:
                    isSeatsDisabledInThisMode = true;
                    //m_ClassInfoBox.ConfigureForNoSelection();
                    _chooseSideUI.SetMessageText( "LobbyEnding");
                    break;
            }
        }
        
         /// <summary>
        /// Internal utility that sets the character-graphics and class-info box based on
        /// our chosen seat. It also triggers a LobbyMode change when it notices that our seat-state
        /// is LockedIn.
        /// </summary>
        /// <param name="state">Our current seat state</param>
        /// <param name="seatIdx">Which seat we're sitting in, or -1 if SeatState is Inactive</param>
        public void UpdateCharacterSelection(NetworkChooseSide.SeatState state, int seatIdx = -1)
        {
            bool isNewSeat = _lastSeatSelected != seatIdx;

            _lastSeatSelected = seatIdx;
            if (state == NetworkChooseSide.SeatState.Inactive)
            {
                Debug.Log("NetworkChooseSide.SeatState.Inactive");
                // if (m_CurrentCharacterGraphics)
                // {
                //     m_CurrentCharacterGraphics.SetActive(false);
                // }
                //
                // m_ClassInfoBox.ConfigureForNoSelection();
            }
            else
            {
                if (seatIdx != -1)
                {
                    // change character preview when selecting a new seat
                    if (isNewSeat)
                    {
                        Debug.Log("is new seat");
                        // var selectedCharacterGraphics = GetCharacterGraphics(m_NetworkCharSelection.AvatarConfiguration[seatIdx]);
                        //
                        // if (m_CurrentCharacterGraphics)
                        // {
                        //     m_CurrentCharacterGraphics.SetActive(false);
                        // }
                        //
                        // selectedCharacterGraphics.SetActive(true);
                        // m_CurrentCharacterGraphics = selectedCharacterGraphics;
                        // m_CurrentCharacterGraphicsAnimator = m_CurrentCharacterGraphics.GetComponent<Animator>();
                        //
                        // m_ClassInfoBox.ConfigureForClass(m_NetworkCharSelection.AvatarConfiguration[seatIdx].CharacterClass);
                    }
                }
                if (state == NetworkChooseSide.SeatState.LockedIn && !_hasLocalPlayerLockedIn)
                {
                    // the local player has locked in their seat choice! Rearrange the UI appropriately
                    // the character should act excited
                    Debug.Log("Player lockedseat");
                   // m_CurrentCharacterGraphicsAnimator.SetTrigger(m_AnimationTriggerOnCharChosen);
                    ConfigureUIForLobbyMode(_networkSideSelection.IsLobbyClosed.Value ? LobbyMode.LobbyEnding : LobbyMode.SeatChosen);
                    _hasLocalPlayerLockedIn = true;
                }
                else if (_hasLocalPlayerLockedIn && state == NetworkChooseSide.SeatState.Active)
                {
                    // reset character seats if locked in choice was unselected
                    if (_hasLocalPlayerLockedIn)
                    {
                        Debug.Log("locked in choice was unselected");
                        ConfigureUIForLobbyMode(LobbyMode.ChooseSeat);
                       // m_ClassInfoBox.SetLockedIn(false);
                        _hasLocalPlayerLockedIn = false;
                    }
                }
                else if (state == NetworkChooseSide.SeatState.Active && isNewSeat)
                {
                    Debug.Log("Animation on side chosen");
                   // m_CurrentCharacterGraphicsAnimator.SetTrigger(m_AnimationTriggerOnCharSelect);
                }
            }
        }
         
         /// <summary>
         /// Called by the server when the lobby closes (because all players are seated and locked in)
         /// </summary>
         void OnLobbyClosedChanged(bool wasLobbyClosed, bool isLobbyClosed)
         {
             if (isLobbyClosed)
             {
                 ConfigureUIForLobbyMode(LobbyMode.LobbyEnding);
                 Debug.Log("Lobby ending");
             }
             else
             {
                 if (_lastSeatSelected == -1)
                 {
                     ConfigureUIForLobbyMode(LobbyMode.ChooseSeat);
                     Debug.Log("Choosing seat ");
                 }
                 else
                 {
                     ConfigureUIForLobbyMode(LobbyMode.SeatChosen);
                     Debug.Log("Seat chosen");
                     //m_ClassInfoBox.ConfigureForClass(m_NetworkCharSelection.AvatarConfiguration[m_LastSeatSelected].CharacterClass);
                 }
             }
         }
         
         /// <summary>
         /// Internal utility that sets the graphics for the eight lobby-seats (based on their current networked state)
         /// </summary>
         void UpdateSeats()
         {
             // Players can hop between seats -- and can even SHARE seats -- while they're choosing a class.
             // Once they have chosen their class (by "locking in" their seat), other players in that seat are kicked out.
             // But until a seat is locked in, we need to display each seat as being used by the latest player to choose it.
             // So we go through all players and figure out who should visually be shown as sitting in that seat.
             
             //TODO:  change magic number
             NetworkChooseSide.LobbyPlayerState[] curSeats = new NetworkChooseSide.LobbyPlayerState[2];
             foreach (NetworkChooseSide.LobbyPlayerState playerState in _networkSideSelection.LobbyPlayers)
             {
                 if (playerState.SeatIdx == -1 || playerState.SeatState == NetworkChooseSide.SeatState.Inactive)
                     continue; // this player isn't seated at all!
                 if (curSeats[playerState.SeatIdx].SeatState == NetworkChooseSide.SeatState.Inactive
                     || (curSeats[playerState.SeatIdx].SeatState == NetworkChooseSide.SeatState.Active && curSeats[playerState.SeatIdx].LastChangeTime < playerState.LastChangeTime))
                 {
                     // this is the best candidate to be displayed in this seat (so far)
                     Debug.Log("curr seat index " + playerState.SeatIdx + " new seat state "+ playerState.SeatState);
                     curSeats[playerState.SeatIdx] = playerState;
                 }
             }

             // now actually update the seats in the UI
             string message = "";
             //TODO:  change magic number
             for (int i = 0; i < 2; ++i)
             {
                 // m_PlayerSeats[i].SetState(curSeats[i].SeatState, curSeats[i].PlayerNumber, curSeats[i].PlayerName);
                 message +=
                     $"current seat state {curSeats[i].SeatState}, Player number = {curSeats[i].PlayerNumber}, player name - {curSeats[i].PlayerName}\n";
             }
         }
         
         /// <summary>
         /// Called directly by UI elements!
         /// </summary>
         /// <param name="seatIdx"></param>
         public void OnPlayerClickedSeat(int seatIdx)
         {
             if (_networkSideSelection.IsSpawned)
             {
                 _networkSideSelection.ChangeSeatServerRpc(NetworkManager.Singleton.LocalClientId, seatIdx, false);
             }
         }

         /// <summary>
         /// Called directly by UI elements!
         /// </summary>
         public void OnPlayerClickedReady()
         {
             if (_networkSideSelection.IsSpawned)
             {
                 // request to lock in or unlock if already locked in
                 _networkSideSelection.ChangeSeatServerRpc(NetworkManager.Singleton.LocalClientId, _lastSeatSelected, !_hasLocalPlayerLockedIn);
             }
         }
         
    }
}
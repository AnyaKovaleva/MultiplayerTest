using System;
using System.Collections;
using ConnectionManagement;
using Unity.Multiplayer.Samples.Utilities;
using Unity.Netcode;
using UnityEngine;
using Utils;
using VContainer;

namespace Gameplay.GameState
{
    [RequireComponent(typeof(NetcodeHooks), typeof(NetworkChooseSide))]

    public class ServerChooseSideState : GameStateBehaviour
    {
        public override GameState ActiveState => GameState.ChooseSide;

        [SerializeField] private NetcodeHooks _netcodeHooks;
        [SerializeField] private NetworkChooseSide _networkChooseSide;

        [Inject] private ConnectionManager _connectionManager;
        
        Coroutine _waitToEndLobbyCoroutine;

        protected override void Awake()
        {
            base.Awake();
            _networkChooseSide = GetComponent<NetworkChooseSide>();

            _netcodeHooks.OnNetworkSpawnHook += OnNetworkSpawn;
            _netcodeHooks.OnNetworkDespawnHook += OnNetworkDespawn;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_netcodeHooks)
            {
                _netcodeHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
                _netcodeHooks.OnNetworkDespawnHook -= OnNetworkDespawn;
            }
        }

        void OnClientChangedSeat(ulong clientId, NetworkChooseSide.SeatType newSeatType, bool lockedIn)
        {
            int idx = FindLobbyPlayerIdx(clientId);
            if (idx == -1)
            {
                throw new Exception($"OnClientChangedSeat: client ID {clientId} is not a lobby player and cannot change seats! Shouldn't be here!");
            }

            if (_networkChooseSide.IsLobbyClosed.Value)
            {
                // The user tried to change their class after everything was locked in... too late! Discard this choice
                return;
            }

            if (newSeatType == NetworkChooseSide.SeatType.NONE)
            {
                // we can't lock in with no seat
                lockedIn = false;
            }
            else
            {
                // see if someone has already locked-in that seat! If so, too late... discard this choice
                foreach (NetworkChooseSide.LobbyPlayerState playerInfo in _networkChooseSide.LobbyPlayers)
                {
                    if (playerInfo.ClientId != clientId && playerInfo.SeatType == newSeatType && playerInfo.SeatState == NetworkChooseSide.SeatState.LockedIn)
                    {
                        // somebody already locked this choice in. Stop!
                        // Instead of granting lock request, change this player to Inactive state.
                        _networkChooseSide.LobbyPlayers[idx] = new NetworkChooseSide.LobbyPlayerState(clientId,
                            _networkChooseSide.LobbyPlayers[idx].PlayerName,
                            _networkChooseSide.LobbyPlayers[idx].PlayerNumber,
                            NetworkChooseSide.SeatState.Inactive);

                        // then early out
                        return;
                    }
                }
            }

            _networkChooseSide.LobbyPlayers[idx] = new NetworkChooseSide.LobbyPlayerState(clientId,
                _networkChooseSide.LobbyPlayers[idx].PlayerName,
                _networkChooseSide.LobbyPlayers[idx].PlayerNumber,
                lockedIn ? NetworkChooseSide.SeatState.LockedIn : NetworkChooseSide.SeatState.Active,
                newSeatType,
                Time.time);

            if (lockedIn)
            {
                // to help the clients visually keep track of who's in what seat, we'll "kick out" any other players
                // who were also in that seat. (Those players didn't click "Ready!" fast enough, somebody else took their seat!)
                for (int i = 0; i < _networkChooseSide.LobbyPlayers.Count; ++i)
                {
                    if (_networkChooseSide.LobbyPlayers[i].SeatType == newSeatType && i != idx)
                    {
                        // change this player to Inactive state.
                        _networkChooseSide.LobbyPlayers[i] = new NetworkChooseSide.LobbyPlayerState(
                            _networkChooseSide.LobbyPlayers[i].ClientId,
                            _networkChooseSide.LobbyPlayers[i].PlayerName,
                            _networkChooseSide.LobbyPlayers[i].PlayerNumber,
                            NetworkChooseSide.SeatState.Inactive);
                    }
                }
            }

            CloseLobbyIfReady();
        }

        /// <summary>
        /// Returns the index of a client in the master LobbyPlayer list, or -1 if not found
        /// </summary>
        int FindLobbyPlayerIdx(ulong clientId)
        {
            for (int i = 0; i < _networkChooseSide.LobbyPlayers.Count; ++i)
            {
                if (_networkChooseSide.LobbyPlayers[i].ClientId == clientId)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Looks through all our connections and sees if everyone has locked in their choice;
        /// if so, we lock in the whole lobby, save state, and begin the transition to gameplay
        /// </summary>
        void CloseLobbyIfReady()
        {
            foreach (NetworkChooseSide.LobbyPlayerState playerInfo in _networkChooseSide.LobbyPlayers)
            {
                if (playerInfo.SeatState != NetworkChooseSide.SeatState.LockedIn)
                    return; // nope, at least one player isn't locked in yet!
            }

            if (_networkChooseSide.LobbyPlayers.Count != _connectionManager.MaxConnectedPlayers)
            {
                //not everyone connected
                return;
            }

            // everybody's ready at the same time! Lock it down!
            _networkChooseSide.IsLobbyClosed.Value = true;

            Debug.Log("Everyone is locked in!!!");
            
            // remember our choices so the next scene can use the info
            SaveLobbyResults();

            // Delay a few seconds to give the UI time to react, then switch scenes
            _waitToEndLobbyCoroutine = StartCoroutine(WaitToEndLobby());
        }

        /// <summary>
        /// Cancels the process of closing the lobby, so that if a new player joins, they are able to chose a character.
        /// </summary>
        void CancelCloseLobby()
        {
            if (_waitToEndLobbyCoroutine != null)
            {
                StopCoroutine(_waitToEndLobbyCoroutine);
            }
            _networkChooseSide.IsLobbyClosed.Value = false;
        }

        void SaveLobbyResults()
        {
            foreach (NetworkChooseSide.LobbyPlayerState playerInfo in _networkChooseSide.LobbyPlayers)
            {
                var playerNetworkObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(playerInfo.ClientId);

               // if (playerNetworkObject && playerNetworkObject.TryGetComponent(out PersistentPlayer persistentPlayer))
               // {
                    // pass avatar GUID to PersistentPlayer
                    // it'd be great to simplify this with something like a NetworkScriptableObjects :(

                    Debug.Log("Do smth with player prefab. ClientID is " + playerInfo.ClientId);

                    // persistentPlayer.NetworkAvatarGuidState.AvatarGuid.Value =
                    //     _networkChooseSide.AvatarConfiguration[playerInfo.SeatIdx].Guid.ToNetworkGuid();
                    //}
            }
        }

        IEnumerator WaitToEndLobby()
        {
            yield return new WaitForSeconds(3);
            SceneLoaderWrapper.Instance.LoadScene("TicTacToe", useNetworkSceneManager: true);
        }

        public void OnNetworkDespawn()
        {
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
                NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
            }
            if (_networkChooseSide)
            {
                _networkChooseSide.OnClientChangedSeat -= OnClientChangedSeat;
            }
        }

        public void OnNetworkSpawn()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                enabled = false;
            }
            else
            {
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
                _networkChooseSide.OnClientChangedSeat += OnClientChangedSeat;

                NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
            }
        }

        void OnSceneEvent(SceneEvent sceneEvent)
        {
            Debug.Log("Scene event happened "+ sceneEvent.SceneName);
            // We need to filter out the event that are not a client has finished loading the scene
            if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;
            // When the client finishes loading the Lobby Map, we will need to Seat it
            Debug.Log("need to seat new player with ID "  +sceneEvent.ClientId);
            SeatNewPlayer(sceneEvent.ClientId);
        }

        int GetAvailablePlayerNumber()
        {
            for (int possiblePlayerNumber = 0; possiblePlayerNumber < _connectionManager.MaxConnectedPlayers; ++possiblePlayerNumber)
            {
                if (IsPlayerNumberAvailable(possiblePlayerNumber))
                {
                    return possiblePlayerNumber;
                }
            }
            // we couldn't get a Player# for this person... which means the lobby is full!
            return -1;
        }

        bool IsPlayerNumberAvailable(int playerNumber)
        {
            bool found = false;
            foreach (NetworkChooseSide.LobbyPlayerState playerState in _networkChooseSide.LobbyPlayers)
            {
                if (playerState.PlayerNumber == playerNumber)
                {
                    found = true;
                    break;
                }
            }

            return !found;
        }

        void SeatNewPlayer(ulong clientId)
        {
            // If lobby is closing and waiting to start the game, cancel to allow that new player to select a character
            if (_networkChooseSide.IsLobbyClosed.Value)
            {
                CancelCloseLobby();
            }

            SessionPlayerData? sessionPlayerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(clientId);
            if (sessionPlayerData.HasValue)
            {
                var playerData = sessionPlayerData.Value;
                if (playerData.PlayerNumber == -1 || !IsPlayerNumberAvailable(playerData.PlayerNumber))
                {
                    // If no player num already assigned or if player num is no longer available, get an available one.
                    playerData.PlayerNumber = GetAvailablePlayerNumber();
                }
                if (playerData.PlayerNumber == -1)
                {
                    // Sanity check. We ran out of seats... there was no room!
                    throw new Exception($"we shouldn't be here, connection approval should have refused this connection already for client ID {clientId} and player num {playerData.PlayerNumber}");
                }

                _networkChooseSide.LobbyPlayers.Add(new NetworkChooseSide.LobbyPlayerState(clientId, playerData.PlayerName, playerData.PlayerNumber, NetworkChooseSide.SeatState.Inactive));
                SessionManager<SessionPlayerData>.Instance.SetPlayerData(clientId, playerData);
            }
        }

        void OnClientDisconnectCallback(ulong clientId)
        {
            // clear this client's PlayerNumber and any associated visuals (so other players know they're gone).
            for (int i = 0; i < _networkChooseSide.LobbyPlayers.Count; ++i)
            {
                if (_networkChooseSide.LobbyPlayers[i].ClientId == clientId)
                {
                    _networkChooseSide.LobbyPlayers.RemoveAt(i);
                    break;
                }
            }

            if (!_networkChooseSide.IsLobbyClosed.Value)
            {
                // If the lobby is not already closing, close if the remaining players are all ready
                CloseLobbyIfReady();
            }
        }
    }
}
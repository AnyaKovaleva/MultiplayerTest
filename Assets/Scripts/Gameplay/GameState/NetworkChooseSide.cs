using System;
using Enums;
using Unity.Netcode;
using Utils;

namespace Gameplay.GameState
{
    public class NetworkChooseSide : NetworkBehaviour
    {
         public enum SeatState : byte
        {
            Inactive,
            Active,
            LockedIn,
        }

         /// <summary>
        /// Describes one of the players in the lobby, and their current character-select status.
        /// </summary>
        public struct LobbyPlayerState : INetworkSerializable, IEquatable<LobbyPlayerState>
        {
            public ulong ClientId;

            private FixedPlayerName m_PlayerName; // I'm sad there's no 256Bytes fixed list :(

            public int PlayerNumber; // this player's assigned "P#". (0=P1, 1=P2, etc.)
            public GameMarkType MarkType; // the latest seat they were in. -1 means none
            public float LastChangeTime;

            public SeatState SeatState;


            public LobbyPlayerState(ulong clientId, string name, int playerNumber, SeatState state, GameMarkType markType = GameMarkType.NONE, float lastChangeTime = 0)
            {
                ClientId = clientId;
                PlayerNumber = playerNumber;
                SeatState = state;
                MarkType = markType;
                LastChangeTime = lastChangeTime;
                m_PlayerName = new FixedPlayerName();

                PlayerName = name;
            }

            public string PlayerName
            {
                get => m_PlayerName;
                private set => m_PlayerName = value;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref ClientId);
                serializer.SerializeValue(ref m_PlayerName);
                serializer.SerializeValue(ref PlayerNumber);
                serializer.SerializeValue(ref SeatState);
                serializer.SerializeValue(ref MarkType);
                serializer.SerializeValue(ref LastChangeTime);
            }

            public bool Equals(LobbyPlayerState other)
            {
                return ClientId == other.ClientId &&
                       m_PlayerName.Equals(other.m_PlayerName) &&
                       PlayerNumber == other.PlayerNumber &&
                       MarkType == other.MarkType &&
                       LastChangeTime.Equals(other.LastChangeTime) &&
                       SeatState == other.SeatState;
            }
        }

        private NetworkList<LobbyPlayerState> m_LobbyPlayers;
        
        private void Awake()
        {
            m_LobbyPlayers = new NetworkList<LobbyPlayerState>();
        }

        /// <summary>
        /// Current state of all players in the lobby.
        /// </summary>
        public NetworkList<LobbyPlayerState> LobbyPlayers => m_LobbyPlayers;
        
        /// <summary>
        /// When this becomes true, the lobby is closed and in process of terminating (switching to gameplay).
        /// </summary>
        public NetworkVariable<bool> IsLobbyClosed { get; } = new NetworkVariable<bool>(false);

        /// <summary>
        /// Server notification when a client requests a different lobby-seat, or locks in their seat choice
        /// </summary>
        public event Action<ulong, GameMarkType, bool> OnClientChangedSeat;

        /// <summary>
        /// RPC to notify the server that a client has chosen a seat.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void ChangeSeatServerRpc(ulong clientId, GameMarkType markType, bool lockedIn)
        {
            OnClientChangedSeat?.Invoke(clientId, markType, lockedIn);
        }
    }
}
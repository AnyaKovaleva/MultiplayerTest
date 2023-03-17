using System;
using ConnectionManagement;
using Enums;
using Unity.Netcode;
using UnityEngine;
using Utils;
using VContainer;

namespace Gameplay.GameState
{
    public class NetworkPostGame: NetworkBehaviour
    {
        public struct PostGamePlayerState: INetworkSerializable, IEquatable<PostGamePlayerState>
        {
            public ulong ClientId;

            private FixedPlayerName m_PlayerName; // I'm sad there's no 256Bytes fixed list :(

            public int PlayerNumber; // this player's assigned "P#". (0=P1, 1=P2, etc.)
            public GameMarkType MarkType; // the latest seat they were in. -1 means none

            public bool Won;
            
            public string PlayerName
            {
                get => m_PlayerName;
                private set => m_PlayerName = value;
            }

            public PostGamePlayerState(ulong clientId, FixedPlayerName mPlayerName, int playerNumber, GameMarkType markType, bool won)
            {
                ClientId = clientId;
                m_PlayerName = mPlayerName;
                PlayerNumber = playerNumber;
                MarkType = markType;
                Won = won;

                PlayerName = m_PlayerName;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref ClientId);
                serializer.SerializeValue(ref m_PlayerName);
                serializer.SerializeValue(ref PlayerNumber);
                serializer.SerializeValue(ref Won);
                serializer.SerializeValue(ref MarkType);
            }

            public bool Equals(PostGamePlayerState other)
            {
                return ClientId == other.ClientId &&
                       m_PlayerName.Equals(other.m_PlayerName) &&
                       PlayerNumber == other.PlayerNumber &&
                       MarkType == other.MarkType &&
                       Won == other.Won;
            }
        }
        
        private NetworkList<PostGamePlayerState> _players;
        
        private void Awake()
        {
            _players = new NetworkList<PostGamePlayerState>();
        }

        /// <summary>
        /// Current state of all players in the lobby.
        /// </summary>
        public NetworkList<PostGamePlayerState> Players => _players;

        
        public NetworkVariable<GameResultState> GameResultState = new NetworkVariable<GameResultState>();
    }
}
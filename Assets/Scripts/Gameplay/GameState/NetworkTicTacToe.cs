using System;
using System.Collections.Generic;
using ConnectionManagement;
using Enums;
using Gameplay.Structs;
using Unity.Netcode;

namespace Gameplay.GameState
{
    public class NetworkTicTacToe : NetworkBehaviour
    {
        public enum SessionState : byte
        {
            Active,
            GameFinished
        }

        /// <summary>
        /// Keeping track of who's turn it is
        /// </summary>
        public NetworkVariable<ulong> CurrentPlayerTurn { get; private set; } = new NetworkVariable<ulong>();

        /// <summary>
        /// When this is GameFinished it's time to move to PostGameSate
        /// </summary>
        public NetworkVariable<SessionState> CurrentSessionState { get; private set; } =
            new NetworkVariable<SessionState>();

        // public NetworkVariable<GameFieldState> GameFieldState { get; private set; } =
        //     new NetworkVariable<GameFieldState>();

        /// <summary>
        /// Client notification when the server updates game field grid
        /// </summary>
        public event Action<Coord, GameMarkType> OnServerUpdateGridValue; 
        
        /// <summary>
        /// RPC to notify clients that the server updates game field grid
        /// </summary>
        [ClientRpc]
        public void UpdateGridValueClientRpc(Coord coord, GameMarkType value)
        {
            OnServerUpdateGridValue?.Invoke(coord,  value);
        }

        /// <summary>
        /// Server notification when a client wants to place their mark
        /// </summary>
        public event Action<ulong, Coord> OnClientMadeMove;

        /// <summary>
        /// RPC to notify the server that a client wants to make a move
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void MakeMoveServerRpc(ulong clientId, Coord coord)
        {
            OnClientMadeMove?.Invoke(clientId, coord);
        }
    }
}
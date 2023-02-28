using Enums;
using Gameplay.GameState;
using UnityEngine;
using Utils;

namespace ConnectionManagement
{
    public struct SessionPlayerData : ISessionPlayerData
    {
        public string PlayerName;
        public int PlayerNumber;
        public bool HasCharacterSpawned;
        public GameMarkType MarkType { get; set; }

        public SessionPlayerData(ulong clientID, string name, bool isConnected = false, bool hasCharacterSpawned = false,GameMarkType markType = GameMarkType.NONE)
        {
            ClientID = clientID;
            PlayerName = name;
            PlayerNumber = -1;
            IsConnected = isConnected;
            HasCharacterSpawned = hasCharacterSpawned;
            MarkType = markType;
        }

        public void UpdateSeatType(GameMarkType markType)
        {
            MarkType = markType;
        }
        
        public bool IsConnected { get; set; }
        public ulong ClientID { get; set; }
        public void Reinitialize()
        {
            //do smth
        }
    }
}

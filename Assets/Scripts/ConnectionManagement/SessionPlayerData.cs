using UnityEngine;
using Utils;

namespace ConnectionManagement
{
    public struct SessionPlayerData : ISessionPlayerData
    {
        public string PlayerName;
        public int PlayerNumber;
        public bool HasCharacterSpawned;

        public SessionPlayerData(ulong clientID, string name, bool isConnected = false, bool hasCharacterSpawned = false)
        {
            ClientID = clientID;
            PlayerName = name;
            PlayerNumber = -1;
            IsConnected = isConnected;
            HasCharacterSpawned = hasCharacterSpawned;
        }

        public bool IsConnected { get; set; }
        public ulong ClientID { get; set; }
        public void Reinitialize()
        {
            //do smth
        }
    }
}

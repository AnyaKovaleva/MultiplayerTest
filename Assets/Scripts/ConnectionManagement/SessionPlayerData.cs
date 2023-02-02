using UnityEngine;
using Utils;

namespace ConnectionManagement
{
    public struct SessionPlayerData : ISessionPlayerData
    {
        public string PlayerName;
        public int PlayerNumber;

        public SessionPlayerData(ulong clientID, string name, bool isConnected = false)
        {
            ClientID = clientID;
            PlayerName = name;
            PlayerNumber = -1;
            IsConnected = isConnected;
        }

        public bool IsConnected { get; set; }
        public ulong ClientID { get; set; }
        public void Reinitialize()
        {
            //do smth
        }
    }
}

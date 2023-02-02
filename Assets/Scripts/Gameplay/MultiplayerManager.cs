using System;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay
{
    public class MultiplayerManager : MonoBehaviour
    {
        public static MultiplayerManager Singleton;

        public static event Action OnHostStarted;
        
        private void Awake()
        {
            Singleton = this;
        }

        [ServerRpc]
        public void HostGame()
        {
            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("Start host successful");
                GameManager.Instance.StartGame();

                foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    Debug.Log("client id: " + client.ClientId);
                }

                NetworkManager.Singleton.OnClientConnectedCallback += obj =>
                {
                    Debug.Log("Client connected");
                    foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
                    {
                        Debug.Log("client id: " + client.ClientId);
                    }
                }; 
                NetworkManager.Singleton.OnClientDisconnectCallback += obj =>
                {
                    Debug.Log("Client disconnected");
                    foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
                    {
                        Debug.Log("client id: " + client.ClientId);
                    }
                };
            }
            else
            {              
                GameManager.Instance.StartGame();
                Debug.LogError("Starting host failed");
            }
        }

        public void JoinGame()
        {
            if (NetworkManager.Singleton.StartClient())
            {
                GameManager.Instance.StartGame();
                Debug.Log("Start client successful");
            }
            else
            {
                Debug.LogError("Starting client failed");
            }
            
        }
    }
}
using System;
using Unity.Netcode;
using UnityEngine;

namespace DefaultNamespace.TicTacToe.UI
{
    public class WaitPopup : UIPanel
    {
        private void Start()
        {
            GameManager.Instance.CurrentTurn.OnValueChanged += OnCurrentTurnChanged;
        }

        private void OnCurrentTurnChanged(GameManager.PlayerType previousValue, GameManager.PlayerType newValue)
        {
            Debug.Log("turn changed to " + newValue + "I'm ");
            if (NetworkManager.Singleton.IsHost)
            {
                if (newValue == GameManager.PlayerType.HOST)
                    SetActive(false);
                else
                    SetActive(true);
            }
            else
            {
                if (newValue == GameManager.PlayerType.CLIENT)
                    SetActive(false);
                else
                    SetActive(true);
            }
        }

        private void SetActive(bool active)
        {
            Root.SetActive(active);
        }
    }
}
using System;
using TicTacToe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.TicTacToe.UI
{
    public class PickSideUI :  UIPanel
    {
        [SerializeField] private Button _xButton;
        [SerializeField] private Button _oButton;
        
        private void Start()
        {
            _xButton.onClick.AddListener((() => PickSide(TicTacToeValue.X)));
            _oButton.onClick.AddListener((() => PickSide(TicTacToeValue.O)));
        }

        private void PickSide(TicTacToeValue side)
        {
            MultiplayerManager.Singleton.HostGame();
            GameManager.Instance.SetHostSideServerRpc(side);
        }
    }
}
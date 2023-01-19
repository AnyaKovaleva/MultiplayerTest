using System;
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
            GameManager.Instance.SetPlayer1Side(side);
            
            UIManager.Instance.Open(typeof(GamePanel));
            GameManager.Instance.StartGame();
        }
    }
}
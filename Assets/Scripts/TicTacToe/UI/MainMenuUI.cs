using System;
using TicTacToe;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.TicTacToe.UI
{
    public class MainMenuUI : UIPanel
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _joinGameButton;

        private void Start()
        {
            _startButton.onClick.AddListener(StartGame);
            _joinGameButton.onClick.AddListener(JoinGame);
        }
        
        private void StartGame()
        {
            UIManager.Instance.Open(typeof(PickSideUI));
        }

        private void JoinGame()
        {
            MultiplayerManager.Singleton.JoinGame();
            UIManager.Instance.Open(typeof(GamePanel));
        }
    }
}
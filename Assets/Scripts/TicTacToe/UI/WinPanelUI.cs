using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.TicTacToe.UI
{
    public class WinPanelUI : UIPanel
    {
        [SerializeField] private TMP_Text _winText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _mainMenuButton;

        private void Start()
        {
            _restartButton.onClick.AddListener(Restart);
            _mainMenuButton.onClick.AddListener(OpenMainMenu);
        }

        public void SetWinText(string text)
        {
            _winText.text = text;
        }
        private void Restart()
        {
            UIManager.Instance.Open(typeof(GamePanel));
            GameManager.Instance.RestartCurrentGameServerRpc();
        }

        private void OpenMainMenu()
        {
            UIManager.Instance.Open(typeof(MainMenuUI));
        }
    }
}
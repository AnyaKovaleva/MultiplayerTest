using System.Collections.Generic;
using UI;
using Unity.Netcode;
using UnityEngine;

namespace Gameplay
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField] private List<Cell> _cells;

        public enum PlayerType
        {
            HOST,
            CLIENT
        }

        private NetworkVariable<int> _moveCount = new NetworkVariable<int>(0);
        public NetworkVariable<PlayerType> CurrentTurn { get; private set; } = new NetworkVariable<PlayerType>(PlayerType.HOST);

        private NetworkVariable<TicTacToeValue> _hostValue = new NetworkVariable<TicTacToeValue>(TicTacToeValue.X);
        private NetworkVariable<TicTacToeValue> _clientValue = new NetworkVariable<TicTacToeValue>(TicTacToeValue.O);

        private readonly string _waitingForPlayerMessage = "Waiting for 2nd player to join";
        
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        
        private void Start()
        {
        //    UIManager.Instance.Open(typeof(MainMenuUI));
        }
        
        public void StartGame()
        {
            Debug.Log($"I am \nServer {NetworkManager.Singleton.IsServer}\nHost {NetworkManager.Singleton.IsHost}\nClient {NetworkManager.Singleton.IsClient}");
            if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClientsList.Count < 2)
            {
                Debug.Log("no clients connected. Waiting");
                UIManager.Instance.OpenMessage(_waitingForPlayerMessage);
                NetworkManager.Singleton.OnClientConnectedCallback += obj =>
                {
                    Debug.Log("client joined");
                    UIManager.Instance.Open(typeof(GamePanel));
                };
            }
            else
            {
                UIManager.Instance.Open(typeof(GamePanel));
                if (CurrentTurn.Value == PlayerType.HOST)
                {
                    UIManager.Instance.Open(typeof(WaitPopup));
                }
            }
        }

        [ServerRpc]
        public void SetHostSideServerRpc(TicTacToeValue value)
        {
            _hostValue.Value = value;
            _clientValue.Value = _hostValue.Value == TicTacToeValue.X ? TicTacToeValue.O : TicTacToeValue.X;
        }

        [ServerRpc]
        public void RestartCurrentGameServerRpc()
        {
            StartGame();
            ResetFieldClientRpc();
        }
        
        [ClientRpc]
        private void ResetFieldClientRpc()
        {
            Debug.Log("Reseting field of " + (IsClient ? "Client" : "Host"));
            foreach (var cell in _cells)
            {
                cell.SetValueServerRpc(TicTacToeValue.EMPTY);
            }
        }

        public void EndTurn(Cell chosenCell)
        {
            if (chosenCell.Value != TicTacToeValue.EMPTY)
                return;

            chosenCell.SetValueServerRpc(CurrentTurn.Value == PlayerType.HOST ? _hostValue.Value : _clientValue.Value);
            CheckValuesServerRpc();
            ChangeTurnServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangeTurnServerRpc()
        {
            CurrentTurn.Value = CurrentTurn.Value == PlayerType.HOST ? PlayerType.CLIENT : PlayerType.HOST;
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void CheckValuesServerRpc()
        {
            _moveCount.Value++;
            TicTacToeValue playerTurn = CurrentTurn.Value == PlayerType.HOST ? _hostValue.Value : _clientValue.Value;
            if (_cells[0].Value == playerTurn && _cells[1].Value == playerTurn && _cells[2].Value == playerTurn)
                GameOverClientRpc(playerTurn);
            else if (_cells[3].Value == playerTurn && _cells[4].Value == playerTurn && _cells[5].Value == playerTurn)
                GameOverClientRpc(playerTurn);
            else if (_cells[6].Value == playerTurn && _cells[7].Value == playerTurn && _cells[8].Value == playerTurn)
                GameOverClientRpc(playerTurn);
            else if (_cells[0].Value == playerTurn && _cells[3].Value == playerTurn && _cells[6].Value == playerTurn)
                GameOverClientRpc(playerTurn);
            else if (_cells[1].Value == playerTurn && _cells[4].Value == playerTurn && _cells[7].Value == playerTurn)
                GameOverClientRpc(playerTurn);
            else if (_cells[2].Value == playerTurn && _cells[5].Value == playerTurn && _cells[8].Value == playerTurn)
                GameOverClientRpc(playerTurn);
            else if (_cells[0].Value == playerTurn && _cells[4].Value == playerTurn && _cells[8].Value == playerTurn)
                GameOverClientRpc(playerTurn);
            else if (_cells[2].Value == playerTurn && _cells[4].Value == playerTurn && _cells[6].Value == playerTurn)
                GameOverClientRpc(playerTurn);
            else if (_moveCount.Value >= 9) GameOverClientRpc(TicTacToeValue.EMPTY);
        }
        
        [ClientRpc]
        private void GameOverClientRpc(TicTacToeValue winSide)
        {
            UIManager.Instance.Open(typeof(WinPanelUI));


            if (_hostValue.Value == winSide)
            {
                UIManager.Instance.SetWinMessage("Player 1 wins!");
            }
            else if (_clientValue.Value == winSide)
            {
                UIManager.Instance.SetWinMessage("Player 2 wins!");
            }
            else
            {
                UIManager.Instance.SetWinMessage("Draw!");
            }
        }
    }
}
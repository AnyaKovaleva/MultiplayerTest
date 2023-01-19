using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.TicTacToe.UI;
using TMPro;
using UnityEngine;

namespace DefaultNamespace.TicTacToe
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private List<Cell> _cells;

        private bool _isPlayer1 = true;
        private int _moveCount = 0;

        private TicTacToeValue _player_1 = TicTacToeValue.X;
        private TicTacToeValue _player_2 = TicTacToeValue.O;

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
            foreach (var cell in _cells)
            {
                cell.Initialize();
                cell.AddOnClickAction(EndTurn);
            }

            ResetField();
            
            UIManager.Instance.Open(typeof(MainMenuUI));
        }

        public void StartGame()
        {
            ResetField();
        }

        public void SetPlayer1Side(TicTacToeValue value)
        {
            _player_1 = value;
            _player_2 = _player_1 == TicTacToeValue.X ? TicTacToeValue.O : TicTacToeValue.X;
        }

        public void RestartCurrentGame()
        {
            ResetField();
        }

        private void ResetField()
        {
            foreach (var cell in _cells)
            {
                cell.SetValue(TicTacToeValue.EMPTY);
                _isPlayer1 = true;
                _moveCount = 0;
            }
        }

        private void EndTurn(Cell chosenCell)
        {
            chosenCell.SetValue(_isPlayer1 ? _player_1 : _player_2);
            CheckValues();
            ChangeTurn();
        }

        private void ChangeTurn()
        {
            _isPlayer1 = !_isPlayer1;
        }

        private void CheckValues()
        {
            _moveCount++;
            TicTacToeValue playerTurn = _isPlayer1 ? _player_1 : _player_2;
            if (_cells[0].Value == playerTurn && _cells[1].Value == playerTurn && _cells[2].Value == playerTurn)
                GameOver(playerTurn);
            else if (_cells[3].Value == playerTurn && _cells[4].Value == playerTurn && _cells[5].Value == playerTurn)
                GameOver(playerTurn);
            else if (_cells[6].Value == playerTurn && _cells[7].Value == playerTurn && _cells[8].Value == playerTurn)
                GameOver(playerTurn);
            else if (_cells[0].Value == playerTurn && _cells[3].Value == playerTurn && _cells[6].Value == playerTurn)
                GameOver(playerTurn);
            else if (_cells[1].Value == playerTurn && _cells[4].Value == playerTurn && _cells[7].Value == playerTurn)
                GameOver(playerTurn);
            else if (_cells[2].Value == playerTurn && _cells[5].Value == playerTurn && _cells[8].Value == playerTurn)
                GameOver(playerTurn);
            else if (_cells[0].Value == playerTurn && _cells[4].Value == playerTurn && _cells[8].Value == playerTurn)
                GameOver(playerTurn);
            else if (_cells[2].Value == playerTurn && _cells[4].Value == playerTurn && _cells[6].Value == playerTurn)
                GameOver(playerTurn);
            else if (_moveCount >= 9) GameOver(TicTacToeValue.EMPTY);
        }

        private void GameOver(TicTacToeValue winSide)
        {
            UIManager.Instance.Open(typeof(WinPanelUI));


            if (_player_1 == winSide)
            {
                UIManager.Instance.SetWinMessage("Player 1 wins!");
            }
            else if (_player_2 == winSide)
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
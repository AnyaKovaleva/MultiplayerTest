using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DefaultNamespace.TicTacToe
{
    public class Cell : MonoBehaviour
    {
        public TicTacToeValue Value { get; private set; }

        private Button _button;
        private TMP_Text _textField;
        
        public void Initialize()
        {
            _button = GetComponent<Button>();
            _textField = _button.GetComponentInChildren<TMP_Text>();

            SetValue(TicTacToeValue.EMPTY);
        }
        public void AddOnClickAction(UnityAction<Cell> onClick)
        {
            _button.onClick.AddListener(() => onClick(this));
        }
        
        public void SetValue(TicTacToeValue newValue)
        {
            Value = newValue;
            UpdateText();
        }

        private void UpdateText()
        {
            if (Value == TicTacToeValue.EMPTY)
            {
                _textField.text = "";
                return;
            }

            _textField.text = Value.ToString();
        }
        
    }
}
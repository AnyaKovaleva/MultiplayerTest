using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DefaultNamespace.TicTacToe
{
    public class Cell : NetworkBehaviour
    {
        public TicTacToeValue Value => _value.Value;
        private NetworkVariable<TicTacToeValue> _value = new NetworkVariable<TicTacToeValue>(TicTacToeValue.EMPTY, NetworkVariableReadPermission.Everyone);
        
        private Button _button;
        private TMP_Text _textField;
        
        public void Initialize()
        {
            _button = GetComponent<Button>();
            _textField = _button.GetComponentInChildren<TMP_Text>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Debug.Log("cell spawned");
            Initialize();
            _button.onClick.AddListener(() => GameManager.Instance.EndTurn(this));
            _value.OnValueChanged += OnValueChanged;
            _value.Value = TicTacToeValue.EMPTY;
        }

        private void OnValueChanged(TicTacToeValue previousvalue, TicTacToeValue newvalue)
        {
            if (newvalue == TicTacToeValue.EMPTY)
            {
                _textField.text = "";
                return;
            }

            _textField.text = newvalue.ToString();
        }

        public void AddOnClickAction(UnityAction<Cell> onClick)
        {
            _button.onClick.AddListener(() => onClick(this));
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetValueServerRpc(TicTacToeValue newValue)
        {
            _value.Value = newValue;
        }
        
    }
}
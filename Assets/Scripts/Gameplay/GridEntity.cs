using System;
using Enums;
using Gameplay.GameState;
using Gameplay.Structs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Gameplay
{
    public class GridEntity : MonoBehaviour
    {
        [SerializeField] private Coord _coord;

        public Coord Coord => _coord;
        
        private Button _button;
        private TMP_Text _textField;

        private GameMarkType _gridContent;
        public GameMarkType GridContent => _gridContent;

        private float _disabledColorAlpha = 0.35f;
        public void Initialize(Coord coord)
        {
            _coord = coord;
            _button = GetComponent<Button>();
            _textField = _button.GetComponentInChildren<TMP_Text>();
            SetMark(GameMarkType.NONE);
            _button.onClick.AddListener(OnButtonPressed);
            SetButtonColor(Color.white, new Color(1,1,1,_disabledColorAlpha));
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnButtonPressed);
            }
        }

        public void OnButtonPressed()
        {
            if (_gridContent != GameMarkType.NONE)
            {
                Debug.LogError("Trying to press  not empty button");
                return;
            }

            Debug.Log("button pressed " + _coord);
            SetButtonColor(Color.cyan, new Color(0,1,1,_disabledColorAlpha));
            SetInteractable(false);
            ClientTicTacToeState.Instance.MakeMove(_coord);

        }
        
        public void SetInteractable(bool isInteractable = true)
        {
            _button.interactable = isInteractable;
        }

        public void SetMark(GameMarkType mark)
        {
            _gridContent = mark;
            _textField.text = mark + "\n" + _coord.ToString();

            if (mark == GameMarkType.NONE)
            {
                SetButtonColor(Color.white, new Color(1,1,1,_disabledColorAlpha));
            }
            else
            {
                SetButtonColor(Color.green, new Color(0,1,0,_disabledColorAlpha));
            }
        }

        private void SetButtonColor(Color normalColor,  Color disabledColor)
        {
            var colors = _button.colors;
            colors.normalColor = normalColor;
            colors.disabledColor = disabledColor;
            _button.colors = colors;
        }
    }
}
using TMPro;
using UnityEngine;

namespace DefaultNamespace.TicTacToe.UI
{
    public class UIMessage : UIPanel
    {
        [SerializeField] private TMP_Text _text;

        public void SetMessage(string message)
        {
            _text.text = message;
        }
    }
}
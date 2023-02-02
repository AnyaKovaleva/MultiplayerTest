using TMPro;
using UnityEngine;

namespace UI
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
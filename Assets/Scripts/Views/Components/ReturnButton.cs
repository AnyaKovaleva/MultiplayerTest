using System;
using Extensions;
using UnityEngine.UIElements;

namespace Views.Components
{
    public class ReturnButton
    {
        private Button _button;
        
        public event Action OnClicked
        {
            add => _button.clicked += value;
            remove => _button.clicked -= value;
        }

        public ReturnButton(VisualElement root)
        {
            _button = root.MapFieldToUI<Button>("ReturnButton");
            OnClicked += ViewsController.Return;
        }
    }
}
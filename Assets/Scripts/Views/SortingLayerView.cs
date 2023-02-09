using Enums.UI;
using UnityEngine.UIElements;

namespace Views
{
    public class SortingLayerView
    {
        public SortingLayer SortingLayer { get; private set; }
        
        private VisualElement _view;

        public SortingLayerView(SortingLayer sortingLayer, VisualElement view)
        {
            SortingLayer = sortingLayer;
            _view = view;
        }

        public void Show()
        {
            _view.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _view.style.display = DisplayStyle.None;
        }

        public void ToggleVisibility(bool isVisible)
        {
            _view.visible = isVisible;
        }

        public void HideEverythingInThisLayer()
        {
            foreach (var child in _view.Children())
            {
                child.style.display = DisplayStyle.None;
            }
        }
    }
}
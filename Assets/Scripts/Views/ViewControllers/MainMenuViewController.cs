using Enums.UI;
using Interfaces.UI;
using UnityEngine.UIElements;
using Views.Views;

namespace Views.ViewControllers
{
    public class MainMenuViewController : IView
    {
        public SortingLayer SortingLayer => SortingLayer.MAIN_MENU;
        public ViewType Type => ViewType.PANEL;

        private MainMenuView _view;

        public MainMenuViewController(UIDocument document)
        {
            _view = new MainMenuView(document);
            
            InitButtonEvents();
        }
        
        public void Open()
        {
            _view.Root.style.display = DisplayStyle.Flex;
        }

        public void Close()
        {
            _view.Root.style.display = DisplayStyle.None;
        }

        public void InitButtonEvents()
        {
            throw new System.NotImplementedException();
        }

        public void InitLocalization()
        {
            throw new System.NotImplementedException();
        }
    }
}
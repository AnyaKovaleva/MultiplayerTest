using Enums.UI;

namespace Interfaces.UI
{
    public interface IView
    {
        public SortingLayer SortingLayer { get; }
        public ViewType Type { get; }

        public void Open();
        public void Close();
        
        //TODO: move to protected?
        public void InitButtonEvents();
        public void InitLocalization();
    }
}
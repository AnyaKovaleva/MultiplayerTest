using Enums.UI;

namespace Interfaces.UI
{
    public interface IPopupView : IView
    {
        public PopupType PopupType { get; }
    }
}
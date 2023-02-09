using Enums.UI;

namespace Interfaces.UI
{
    public interface IMessageView : IPopupView
    {
        public void Open(string message);
    }
}
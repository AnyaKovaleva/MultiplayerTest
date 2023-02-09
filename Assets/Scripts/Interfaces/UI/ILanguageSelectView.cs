using System;

namespace Interfaces.UI
{
    public interface ILanguageSelectView : IPopupView
    {
        public void Open(Action<string> languageSelectedCallback);
    }
}
using Enums.UI;
using UnityEngine.UIElements;
using VContainer;

namespace Interfaces.UI
{
    public interface IView
    {
        public SortingLayer SortingLayer { get; }
        public ViewType Type { get; }
        public void Open();
        public void Close();
    }
}
using System;

namespace Interfaces.UI
{
    public interface IListView : IView
    {
        public Type ContentType { get; }
    }
}
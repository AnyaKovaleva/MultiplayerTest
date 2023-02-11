using Enums.UI;
using UnityEngine.UIElements;
using VContainer;

namespace Views
{
    public abstract class ViewController
    {
        protected View _view;

        public ViewType Type => ViewType;

        public ViewType ViewType;

        [Inject]
        public abstract void InjectDependenciesAndInitialize(UIDocument document);

        public virtual void Initialize(View view, ViewType viewType = ViewType.PANEL)
        {
            _view = view;
            ViewType = viewType;
            InitButtonEvents();
        }

        public virtual void Open()
        {
            _view.Root.style.display = DisplayStyle.Flex;
        }

        public virtual void Close()
        {
            _view.Root.style.display = DisplayStyle.None;
        }

        protected abstract void InitButtonEvents();

        protected virtual void InitLocalization()
        {
        }
    }
}
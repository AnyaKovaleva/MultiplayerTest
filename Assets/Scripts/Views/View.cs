using UnityEngine.UIElements;

namespace Views
{
    public abstract class View
    {
        public VisualElement Root { get; protected set; }

        public View(UIDocument document)
        {
            MapFieldsToUI(document.rootVisualElement);
        }
        
        public View(VisualElement  root)
        {
            MapFieldsToUI(root);
        }

        public abstract void MapFieldsToUI(VisualElement  root);
    }
}
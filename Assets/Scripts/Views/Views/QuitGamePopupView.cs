using UnityEngine.UIElements;

namespace Views.Views
{
    public class QuitGamePopupView : View
    {
        public Button CancelButton { get; private set; }
        public Button QuitButton { get; private set; }

        
        public QuitGamePopupView(UIDocument document) : base(document)
        {
        }

        public override void MapFieldsToUI(VisualElement root)
        {
            Root = root.Q<VisualElement>("QuitPopup");

            CancelButton = Root.Q<Button>("CancelButton");
            QuitButton = Root.Q<Button>("QuitButton");

        }
    }
}
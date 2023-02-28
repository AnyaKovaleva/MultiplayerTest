using Extensions;
using UnityEngine.UIElements;

namespace Views.Views
{
    public class GameHUDView: View
    {
        public Label PlayerSideLabel { get; private set; }
        public Label TurnLabel { get; private set; }
        public Button QuitButton { get; private set; }

        public GameHUDView(UIDocument document) : base(document)
        {
        }

        public override void MapFieldsToUI(VisualElement root)
        {
            Root = root.Q<VisualElement>("HUD");

            PlayerSideLabel = Root.MapFieldToUI<Label>("PlayerSideLabel");
            TurnLabel = Root.MapFieldToUI<Label>("TurnLabel");
            QuitButton = Root.Q<Button>("QuitButton");
        }
    }
}
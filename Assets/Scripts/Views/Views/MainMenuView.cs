using Extensions;
using UnityEngine.UIElements;

namespace Views.Views
{
    public class MainMenuView : View
    {
        public Button PlayButton { get; private set; }
        public Button ChangeProfileButton { get; private set; }
        
        public MainMenuView(UIDocument document) : base(document)
        {
        }

        public override void MapFieldsToUI(VisualElement root)
        {
            Root = root.MapFieldToUI<VisualElement>("MainMenu");

            PlayButton = Root.MapFieldToUI<Button>("PlayButton");
            ChangeProfileButton = Root.MapFieldToUI<Button>("ChangeProfileButton");
        }
    }
}
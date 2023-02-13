using Extensions;
using UnityEngine.UIElements;

namespace Views.Views
{
    public class CreateLobbyView : View
    {
        public TextField LobbyNameInputField { get; private set; }
        public Toggle  IsPrivateToggle { get; private set; }
        public Button CloseButton { get; private set; }
        public Button CreateLobbyButton { get; private set; }
        
        public CreateLobbyView(VisualElement root) : base(root)
        {
            CloseButton.clicked += Close;
            Close();
        }
        
        public void Open()
        {
            Root.style.display = DisplayStyle.Flex;
        }

        public void Close()
        {
            Root.style.display = DisplayStyle.None;
        }

        public override void MapFieldsToUI(VisualElement root)
        {
            Root = root.MapFieldToUI<VisualElement>("CreateLobbyPopup");

            LobbyNameInputField = Root.MapFieldToUI<TextField>("LobbyNameInputField");
            IsPrivateToggle = Root.MapFieldToUI<Toggle>("IsPrivateToggle");

            CloseButton = Root.MapFieldToUI<Button>("CloseButton");
            CreateLobbyButton = Root.MapFieldToUI<Button>("CreateLobbyButton");
        }
    }
}
using Extensions;
using UnityEngine.UIElements;
using Views.Components;

namespace Views.Views
{
    public class ProfileView : View
    {
        public ReturnButton ReturnButton { get; private set; }
        public Label Username { get; private set; }
        public TextField UsernameInputField { get; private set; }
        public TextField PasswordInputField { get; private set; }
        
        public Button SignInButton { get; private set; }
        public Button SignUpButton { get; private set; }
        
        public Label MessageLabel { get; private set; }
        
        public ProfileView(UIDocument document) : base(document)
        {
        }

        public override void MapFieldsToUI(VisualElement root)
        {
            Root = root.MapFieldToUI<VisualElement>("ProfilePanel");

            ReturnButton = new ReturnButton(Root);

            Username = Root.MapFieldToUI<Label>("UsernameLabel");
            UsernameInputField = Root.MapFieldToUI<TextField>("UsernameInputField");
            PasswordInputField = Root.MapFieldToUI<TextField>("PasswordInputField");

            SignInButton = Root.MapFieldToUI<Button>("SignInButton");
            SignUpButton = Root.MapFieldToUI<Button>("SignUpButton");

            MessageLabel = Root.MapFieldToUI<Label>("MessageLabel");
        }
    }
}
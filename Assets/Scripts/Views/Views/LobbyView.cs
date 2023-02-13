using Extensions;
using UnityEngine.UIElements;
using Views.Components;

namespace Views.Views
{
    public class LobbyView : View
    {
        public ReturnButton ReturnButton { get; private set; }

        public Button CreateGameButton { get; private set; }
        public Button JoinGameButton { get; private set; }
        
        public Label LobbiesLabel { get; private set; }
        
        public Label PlayerNameLabel { get; private set; }
        
        public LobbyView(UIDocument document) : base(document)
        {
        }

        public override void MapFieldsToUI(VisualElement root)
        {
            Root = root.MapFieldToUI<VisualElement>("LobbyPanel");

            ReturnButton = new ReturnButton(Root);

            CreateGameButton = Root.MapFieldToUI<Button>("CreateGameButton");
            JoinGameButton = Root.MapFieldToUI<Button>("JoinGameButton");

            LobbiesLabel = Root.MapFieldToUI<Label>("LobbiesLabel");
            PlayerNameLabel = Root.MapFieldToUI<Label>("PlayerNameLabel");
        }
    }
}
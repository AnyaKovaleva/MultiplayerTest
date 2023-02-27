using UnityEngine.UIElements;

namespace Views.Views
{
    public class ChooseSideView : View
    {
        public Label JoinCodeLabel { get; private set; }
        public Button CopyCodeButton { get; private set; }
        public Button QuitButton { get; private set; }
        
        public VisualElement XButton { get; private set; }
        public VisualElement OButton { get; private set; }
        
        public Button ReadyButton { get; private set; }
        
        public Label MessageLabel { get; private set; }
        public Label LobbyStateLabel { get; private set; }
        public Label NumPlayersLabel { get; private set; }

        public ChooseSideView(UIDocument document) : base(document)
        {
        }

        public override void MapFieldsToUI(VisualElement root)
        {
            Root = root.Q<VisualElement>("ChooseSidePanel");

            JoinCodeLabel = Root.Q<Label>("JoinCodeLabel");
            
            CopyCodeButton = Root.Q<Button>("CopyCodeButton");
            QuitButton = Root.Q<Button>("QuitButton");
            
            XButton = Root.Q<VisualElement>("XButton");
            OButton = Root.Q<VisualElement>("OButton");
            
            ReadyButton = Root.Q<Button>("ReadyButton");
            
            MessageLabel = Root.Q<Label>("MessageLabel");
            
            LobbyStateLabel = Root.Q<Label>("LobbyStateLabel");
            NumPlayersLabel = Root.Q<Label>("NumPlayersLabel");

        }
    }
}
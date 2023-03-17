using Extensions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace Views.Views
{
    public class PostGameView : View
    {
        public Label GameResultLabel { get; private set; }
        public Label WaitForHostLabel { get; private set; }

        public Button RestartButton { get; private set; }
        public Button ToMainMenuButton { get; private set; }

        public PostGameView(UIDocument document) : base(document)
        {
            if (document == null)
            {
                Debug.LogError("POST GAME UI DOCUMENT IS NULL");
            }
        }

        public override void MapFieldsToUI(VisualElement root)
        {
            if (NetworkManager.Singleton.IsClient)
            {
                Debug.Log("initializing PostGameView on client");
                if (root == null)
                {
                    Debug.LogError("Root is NULL!!!");
                }
            }
            else
            {
                Debug.LogWarning("initializing PostGameView on SERVER!!");
            }

            Root = root.MapFieldToUI<VisualElement>("PostGamePanel");

            GameResultLabel = Root.MapFieldToUI<Label>("GameResultLabel");
            WaitForHostLabel = Root.MapFieldToUI<Label>("WaitForHostLabel");

            RestartButton = Root.MapFieldToUI<Button>("RestartButton");
            ToMainMenuButton = Root.MapFieldToUI<Button>("ToMainMenuButton");
        }
    }
}
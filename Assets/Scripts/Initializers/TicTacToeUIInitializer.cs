using System.Collections.Generic;
using Interfaces.UI;
using Views;
using Views.ViewControllers;

namespace Initializers
{
    public class TicTacToeUIInitializer : UIInitializer
    {
        public static TicTacToeUIInitializer Instance { get; private set; }

        public GameHUD HUD = new GameHUD();
        public QuitGamePopup QuitGamePopup = new QuitGamePopup();
        
        public override void Start()
        {
            base.Start();
            Instance = this;
        }
        
        protected override void InjectDependencies()
        {
            Inject(HUD);
            Inject(QuitGamePopup);
        }

        protected override void InitializeViewsController()
        {
            ViewsController.Initialize(new List<IView>()
            {
                HUD,
                QuitGamePopup
            }, new List<SortingLayerView>());
        }

        protected override void OpenStartView()
        {
           ViewsController.Open(typeof(GameHUD));
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
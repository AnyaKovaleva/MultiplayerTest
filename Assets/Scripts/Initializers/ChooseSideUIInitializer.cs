using System.Collections.Generic;
using Interfaces.UI;
using Views;
using Views.ViewControllers;

namespace Initializers
{
    public class ChooseSideUIInitializer : UIInitializer
    {
        public static ChooseSideUIInitializer Instance { get; private set; }
        
        public ChooseSide ChooseSide => _chooseSide;
        private ChooseSide _chooseSide = new ChooseSide();

        public override void Start()
        {
            base.Start();
            Instance = this;
        }

        protected override void InjectDependencies()
        {
            Inject(_chooseSide);
        }

        protected override void InitializeViewsController()
        {
            ViewsController.Initialize(new List<IView>()
            {
                _chooseSide
            }, new List<SortingLayerView>());
        }

        protected override void OpenStartView()
        {
            ViewsController.Open(typeof(ChooseSide));
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}
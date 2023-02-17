using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Initializers
{
    public abstract class UIInitializer : IStartable,  IDisposable
    {
        [Inject] protected LifetimeScope _serviceScope;

        public virtual void Start()
        {
            Debug.Log("Initializing on start");
            InitializeViews();
            OpenStartView();
        }

        protected virtual void InitializeViews()
        {
            InjectDependencies();
            InitializeViewsController();
        }
        protected abstract void InjectDependencies();

        protected abstract void InitializeViewsController();

        protected virtual void Inject(object instance)
        {
            _serviceScope.Container.Inject(instance);
        }

        protected abstract void OpenStartView();

        public abstract void Dispose();
    }
}
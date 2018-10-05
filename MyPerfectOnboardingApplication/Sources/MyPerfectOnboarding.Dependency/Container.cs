using System;
using MyPerfectOnboarding.Contracts.Dependency;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace MyPerfectOnboarding.Dependency
{
    internal class Container : IContainer
    {
        internal readonly IUnityContainer UnityContainer;

        public Container(IUnityContainer unityContainer)
        {
            UnityContainer = unityContainer;
        }

        public void Dispose()
        {
            UnityContainer.Dispose();
        }

        public IContainer RegisterBootstraper<TBootstraper>() where TBootstraper : IBootstraper, new()
        {
            new TBootstraper().RegisterTypesTo(this);
            return this;
        }

        public IContainer RegisterType<TIType, TType>() where TType : TIType
        {
            UnityContainer.RegisterType<TIType, TType>(new HierarchicalLifetimeManager());
            return this;
        }

        public IContainer RegisterType<TType>(Func<TType> function)
        {
            UnityContainer.RegisterType<TType>(new HierarchicalLifetimeManager(), new InjectionFactory(_ => function()));
            return this;
        }
    }
}

using System;
using System.Collections.Generic;
using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Dependency;
using MyPerfectOnboarding.Dependency.Extensions;
using Unity;
using Unity.Injection;

namespace MyPerfectOnboarding.Dependency.Containers
{
    internal class Container : IContainer
    {
        internal readonly IUnityContainer UnityContainer;

        public Container(IUnityContainer unityContainer)
        {
            UnityContainer = unityContainer;
        }

        public void Dispose()
            => UnityContainer.Dispose();

        public IContainer CreateChildContainer()
        {
            var child = UnityContainer.CreateChildContainer();
            return new Container(child);
        }

        public object Resolve(Type serviceType) 
            => UnityContainer.Resolve(serviceType);

        public IEnumerable<object> ResolveAll(Type serviceType)
            => UnityContainer.ResolveAll(serviceType);

        public IContainer RegisterBootstraper<TBootstraper>() where TBootstraper : IBootstraper, new()
        {
            new TBootstraper().RegisterTypesTo(this);
            return this;
        }

        public IContainer RegisterType<TIType, TType>(LifetimeManagerType type) where TType : TIType
        {
            UnityContainer.RegisterType<TIType, TType>(type.CreateLifetimeManager());
            return this;
        }

        public IContainer RegisterType<TType>(Func<TType> function, LifetimeManagerType type)
        {
            UnityContainer.RegisterType<TType>(type.CreateLifetimeManager(), new InjectionFactory(_ => function()));
            return this;
        }

        public IContainer RegisterInstance<TIType>(TIType instance)
        {
            UnityContainer.RegisterInstance(instance);
            return this;
        }
    }
}

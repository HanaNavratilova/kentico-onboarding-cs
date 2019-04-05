using System;
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

        public IContainer Register<TContract, TImplementation>(Lifetime lifetime)
            where TImplementation : TContract
        {
            UnityContainer.RegisterType<TContract, TImplementation>(lifetime.CreateLifetimeManager());
            return this;
        }

        public IContainer Register<TImplementation>(Func<TImplementation> creatingFunction, Lifetime lifetime)
        {
            UnityContainer.RegisterType<TImplementation>(lifetime.CreateLifetimeManager(), new InjectionFactory(_ => creatingFunction()));
            return this;
        }

        public IContainer Register<TInstance>(TInstance instance)
        {
            UnityContainer.RegisterInstance(instance);
            return this;
        }
    }
}

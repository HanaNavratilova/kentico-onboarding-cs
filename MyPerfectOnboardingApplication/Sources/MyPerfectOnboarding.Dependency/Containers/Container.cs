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

        public IContainer Register<TIType, TType>(Lifetime lifetime)
            where TType : TIType
        {
            UnityContainer.RegisterType<TIType, TType>(lifetime.CreateLifetimeManager());
            return this;
        }

        public IContainer Register<TType>(Func<TType> creatingFunction, Lifetime lifetime)
        {
            UnityContainer.RegisterType<TType>(lifetime.CreateLifetimeManager(), new InjectionFactory(_ => creatingFunction()));
            return this;
        }

        public IContainer Register<TIType>(TIType instance)
        {
            UnityContainer.RegisterInstance(instance);
            return this;
        }
    }
}

using System;
using System.Collections.Generic;

namespace MyPerfectOnboarding.Contracts.Dependency
{
    public interface IContainer : IDisposable
    {
       IContainer RegisterBootstraper<TBootstraper>() where TBootstraper: IBootstraper, new();

        IContainer RegisterInstance<TIType>(TIType instance);

        IContainer RegisterType<TIType, TType>(LifetimeManagerType type) where TType : TIType;

        IContainer RegisterType<TType>(Func<TType> function, LifetimeManagerType type);

       IContainer CreateChildContainer();

        object Resolve(Type serviceType);

        IEnumerable<object> ResolveAll(Type serviceType);
    }
}

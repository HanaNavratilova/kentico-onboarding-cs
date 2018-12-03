using System;

namespace MyPerfectOnboarding.Contracts.Dependency
{
    public interface IContainer : IDisposable
    {
       IContainer RegisterBootstraper<TBootstraper>() where TBootstraper: IBootstraper, new();

        IContainer RegisterInstance<TIType>(TIType instance);

        IContainer RegisterType<TIType, TType>(Lifetime lifetime) where TType : TIType;

        IContainer RegisterType<TType>(Func<TType> creatingFunction, Lifetime lifetime);
    }
}

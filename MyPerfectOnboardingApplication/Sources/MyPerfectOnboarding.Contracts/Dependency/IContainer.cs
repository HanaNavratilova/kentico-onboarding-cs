using System;

namespace MyPerfectOnboarding.Contracts.Dependency
{
    public interface IContainer : IDisposable
    {
        IContainer Register<TIType>(TIType instance);

        IContainer Register<TIType, TType>(Lifetime lifetime) where TType : TIType;

        IContainer Register<TType>(Func<TType> creatingFunction, Lifetime lifetime);
    }
}

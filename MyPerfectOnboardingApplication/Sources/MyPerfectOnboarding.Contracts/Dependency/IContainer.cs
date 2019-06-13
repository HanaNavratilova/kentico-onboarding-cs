using System;

namespace MyPerfectOnboarding.Contracts.Dependency
{
    public interface IContainer : IDisposable
    {
        IContainer Register<TInstance>(TInstance instance);

        IContainer Register<TContract, TImplementation>(Lifetime lifetime)
            where TImplementation : TContract;

        IContainer Register<TImplementation>(Func<TImplementation> creatingFunction, Lifetime lifetime);
    }
}

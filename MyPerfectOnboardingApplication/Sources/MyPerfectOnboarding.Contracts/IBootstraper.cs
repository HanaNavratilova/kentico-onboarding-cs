using MyPerfectOnboarding.Contracts.Dependency;

namespace MyPerfectOnboarding.Contracts
{
    public interface IBootstraper
    {
        IContainer RegisterTypesTo(IContainer container);
    }
}

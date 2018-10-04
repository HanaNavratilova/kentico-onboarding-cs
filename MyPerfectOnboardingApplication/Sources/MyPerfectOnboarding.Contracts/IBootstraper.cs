using Unity;

namespace MyPerfectOnboarding.Contracts
{
    public interface IBootstraper
    {
        IUnityContainer RegisterTypesTo(IUnityContainer container);
    }
}

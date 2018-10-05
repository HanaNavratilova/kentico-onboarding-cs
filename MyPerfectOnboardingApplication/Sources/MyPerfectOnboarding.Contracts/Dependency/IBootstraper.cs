namespace MyPerfectOnboarding.Contracts.Dependency
{
    public interface IBootstraper
    {
        IContainer RegisterTypesTo(IContainer container);
    }
}

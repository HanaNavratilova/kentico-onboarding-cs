using MyPerfectOnboarding.Contracts.Services.Location;

namespace MyPerfectOnboarding.Api.Configuration
{
    internal class ControllersRouteNames: IControllersRouteNames
    {
        public const string GetItemRouteName = "GetListItem";

        public string ListItemRouteName { get; } = GetItemRouteName;
    }
}
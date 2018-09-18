using MyPerfectOnboarding.Contracts.Api;

namespace MyPerfectOnboarding.Api
{
    public class UrlLocatorConfig: IUrlLocatorConfig
    {
        public const string GetItemRouteName = "GetListItem";

        public string ListItemRouteName { get; } = GetItemRouteName;
    }
}
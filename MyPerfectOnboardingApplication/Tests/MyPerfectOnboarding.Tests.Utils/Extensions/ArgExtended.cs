using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Tests.Utils.Comparers;
using NSubstitute;

namespace MyPerfectOnboarding.Tests.Utils.Extensions
{
    public static class ArgExtended
    {
        public static ListItem IsListItem(IListItem expectedItem)
            => Arg.Is<ListItem>(acutalItem => ListItemEqualityComparer.Instance.Equals(acutalItem, expectedItem));
    }
}

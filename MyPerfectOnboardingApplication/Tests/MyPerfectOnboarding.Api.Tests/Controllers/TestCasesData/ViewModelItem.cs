using MyPerfectOnboarding.Api.Models;
using MyPerfectOnboarding.Tests.Utils.Builders;

namespace MyPerfectOnboarding.Api.Tests.Controllers.TestCasesData
{
    internal static class ViewModelItem
    {
        public static ListItemViewModel WithEmptyText =
            ListItemViewModelBuilder.CreateItem(string.Empty);

        public static ListItemViewModel WithText =
            ListItemViewModelBuilder.CreateItem("item");

        public static ListItemViewModel WithIdAndText =
            ListItemViewModelBuilder.CreateItem("0A777043-8991-49FA-9F4F-DAF15005BC77", "aaa");

        public static ListItemViewModel WithIdAndEmptyText =
            ListItemViewModelBuilder.CreateItem("22AC59B7-9517-4EDD-9DDD-EB418A7C1689", string.Empty);

        public static ListItemViewModel WithEmptyIdTextAndCreationTime =
            ListItemViewModelBuilder.CreateItem(null, "aaa", "2014-12-31");

        public static ListItemViewModel WithIdTextAndCreationTime =
            ListItemViewModelBuilder.CreateItem("E5D7ECEB-855C-4B75-8166-F39068583916", "aaa", "2014-12-31");

        public static ListItemViewModel WithEmptyIdTextAndTimes =
            ListItemViewModelBuilder.CreateItem(null, "aaa", "2014-12-31", "2018-11-25");
    }
}

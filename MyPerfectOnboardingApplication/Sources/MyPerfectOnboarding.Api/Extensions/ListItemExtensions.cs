using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyPerfectOnboarding.Api.Models;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Api.Extensions
{
    internal static class ListItemExtensions
    {
        public static ListItemViewModel ToViewModel(this ListItem item)
            => new ListItemViewModel
            {
                Id = item.Id,
                Text = item.Text,
                IsActive = item.IsActive,
                CreationTime = item.CreationTime,
                LastUpdateTime = item.LastUpdateTime
            };

        public static async Task<IEnumerable<ListItemViewModel>> ToViewModelsAsync(this Task<IEnumerable<ListItem>> items)
            => (await items).Select(ToViewModel);
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPerfectOnboarding.Contracts.Models
{
    public static class IListItemExtensions
    {
        public static async Task<ListItem> AsImmutable(this Task<IListItem> awaitableItem)
        {
            var item = await awaitableItem;
            return new ListItem(item);
        }

        public static async Task<IReadOnlyList<ListItem>> AsImmutable(this Task<List<IListItem>> awaitableItems)
        {
            var items = await awaitableItems;
            return items.Select(item => new ListItem(item)).ToList().AsReadOnly();
        }

        public static ListItem AsImmutable(this IListItem item)
            => new ListItem(item);
    }
}

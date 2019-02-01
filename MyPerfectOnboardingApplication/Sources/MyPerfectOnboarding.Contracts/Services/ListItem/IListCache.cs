using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyPerfectOnboarding.Contracts.Services.ListItem
{
    public interface IListCache
    {
        Task<Models.ListItem> AddItemAsync(Models.ListItem item);

        Task<Models.ListItem> DeleteItemAsync(Guid id);

        Task ReplaceItemAsync(Models.ListItem editedItem);

        Task<IEnumerable<Models.ListItem>> GetAllItemsAsync();

        Task<Models.ListItem> GetItemAsync(Guid itemId);

        Task<bool> ExistsItemWithIdAsync(Guid id);
    }
}

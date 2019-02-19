using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Contracts.Services.ListItems
{
    public interface IListCache
    {
        Task<ListItem> AddItemAsync(ListItem item);

        Task<ListItem> DeleteItemAsync(Guid id);

        Task<ListItem> ReplaceItemAsync(ListItem editedItem);

        Task<IEnumerable<Models.ListItem>> GetAllItemsAsync();

        Task<ListItem> GetItemAsync(Guid itemId);

        Task<bool> ExistsItemWithIdAsync(Guid id);
    }
}

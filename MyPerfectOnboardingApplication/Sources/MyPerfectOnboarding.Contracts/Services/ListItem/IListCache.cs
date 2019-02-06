using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Contracts.Services.ListItem
{
    public interface IListCache
    {
        Task<IListItem> AddItemAsync(IListItem item);

        Task<IListItem> DeleteItemAsync(Guid id);

        Task ReplaceItemAsync(IListItem editedItem);

        Task<IEnumerable<IListItem>> GetAllItemsAsync();

        Task<IListItem> GetItemAsync(Guid itemId);

        Task<bool> ExistsItemWithIdAsync(Guid id);
    }
}

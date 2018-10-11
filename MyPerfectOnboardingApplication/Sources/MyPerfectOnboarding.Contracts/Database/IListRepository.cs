using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Contracts.Database
{
    public interface IListRepository
    {
        Task<ListItem> AddItemAsync(ListItem item);

        Task<ListItem> DeleteItemAsync(Guid id);

        Task ReplaceItemAsync(ListItem editedItem);

        Task<IEnumerable<ListItem>> GetAllItemsAsync();

        Task<ListItem> GetItemAsync(Guid itemId);
    }
}

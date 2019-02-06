using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Contracts.Database
{
    public interface IListRepository
    {
        Task<IListItem> AddItemAsync(IListItem item);

        Task<IListItem> DeleteItemAsync(Guid id);

        Task<IListItem> ReplaceItemAsync(IListItem editedItem);

        Task<IEnumerable<IListItem>> GetAllItemsAsync();

        Task<IListItem> GetItemAsync(Guid itemId);
    }
}

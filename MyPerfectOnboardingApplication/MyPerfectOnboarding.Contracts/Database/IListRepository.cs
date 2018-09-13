using System;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Contracts
{
    public interface IListRepository
    {
        Task<ListItem> AddItemAsync(ListItem item);

        Task DeleteItemAsync(Guid id);

        Task EditItemAsync(Guid id, ListItem editedItem);

        Task<ListItem[]> GetAllItemsAsync();

        Task<ListItem> GetItemAsync(Guid itemId);
    }
}

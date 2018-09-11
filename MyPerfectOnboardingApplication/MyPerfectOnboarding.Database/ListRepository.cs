using System;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Database
{
    internal class ListRepository : IListRepository
    {
        private ListItem[] _items = {
            new ListItem{Id = Guid.NewGuid(), Text = "aaaa", IsActive = false, CreationTime = DateTime.MinValue, LastUpdateTime = DateTime.MinValue},
            new ListItem{Id = Guid.NewGuid(), Text = "dfads", IsActive = false, CreationTime = DateTime.MinValue, LastUpdateTime = DateTime.MinValue},
        };

        public async Task<ListItem> AddItemAsync(ListItem item)
        {
            return await Task.FromResult(item);
        }

        public async Task DeleteItemAsync(Guid id)
        {
        }

        public async Task EditItemAsync(Guid id, ListItem editedItem)
        {
        }

        public async Task<ListItem[]> GetAllItemsAsync()
        {
            return await Task.FromResult(_items);
        }

        public async Task<ListItem> GetItemAsync(Guid itemId)
        {
            return await Task.FromResult(_items[0]);
        }
    }
}

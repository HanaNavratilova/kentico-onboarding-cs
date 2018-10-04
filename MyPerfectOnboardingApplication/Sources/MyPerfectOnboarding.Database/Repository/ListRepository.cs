using System;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Database.Repository
{
    internal class ListRepository : IListRepository
    {
        private readonly ListItem[] _items = {
            new ListItem
            {
                Id = new Guid("11AC59B7-9517-4EDD-9DDD-EB418A7C1644"),
                Text = "aaaaa",
                IsActive = false,
                CreationTime = new DateTime(1589, 12, 3),
                LastUpdateTime = new DateTime(1896, 4, 7)
            },
            new ListItem
            {
                Id = new Guid("0B9E6EAF-83DC-4A99-9D57-A39FAF258CAC"),
                Text = "dfads",
                IsActive = false,
                CreationTime = new DateTime(4568, 6, 23),
                LastUpdateTime = new DateTime(8569, 8, 24)
            },
        };

        public async Task<ListItem> AddItemAsync(ListItem item) => await Task.FromResult(_items[1]);

        public async Task<ListItem> DeleteItemAsync(Guid id) => await Task.FromResult(_items[0]);

        public async Task ReplaceItemAsync(ListItem editedItem) => await Task.CompletedTask;

        public async Task<ListItem[]> GetAllItemsAsync() => await Task.FromResult(_items);

        public async Task<ListItem> GetItemAsync(Guid itemId) => await Task.FromResult(_items[0]);
    }
}

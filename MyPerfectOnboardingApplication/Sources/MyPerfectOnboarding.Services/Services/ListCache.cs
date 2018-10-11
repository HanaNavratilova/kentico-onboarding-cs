using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Database.Services;

namespace MyPerfectOnboarding.Services.Services
{
    internal class ListCache : IListCache
    {
        private readonly IListRepository _listRepository;
        internal ConcurrentDictionary<Guid, ListItem> Items;
        private bool _isInitialized;

        public ListCache(IListRepository listRepository)
        {
            _listRepository = listRepository;
        }

        public async Task<ListItem> AddItemAsync(ListItem item)
        {
            await TryInitializeItemsAsync();

            Items.TryAdd(item.Id, item);
            return await _listRepository.AddItemAsync(item);
        }

        public async Task<ListItem> DeleteItemAsync(Guid id)
        {
            await TryInitializeItemsAsync();

            if (!ExistsItemWithId(id))
                return null;
            
            Items.TryRemove(id, out _);
            return await _listRepository.DeleteItemAsync(id);
        }

        public async Task ReplaceItemAsync(ListItem item)
        {
            await TryInitializeItemsAsync();

            if (!ExistsItemWithId(item.Id))
                return;

            Items.TryGetValue(item.Id, out var oldItem);
            Items.TryUpdate(item.Id, item, oldItem);
            await _listRepository.ReplaceItemAsync(item);
        }

        public async Task<IEnumerable<ListItem>> GetAllItemsAsync()
        {
            await TryInitializeItemsAsync();

            return Items.Values;
        }

        public async Task<ListItem> GetItemAsync(Guid id)
        {
            await TryInitializeItemsAsync();

            if (ExistsItemWithId(id))
            {
                Items.TryGetValue(id, out var item);
                return item;
            }

            return null;
        }

        private async Task TryInitializeItemsAsync()
        {
            if (!_isInitialized) {
                _isInitialized = true;
                var items = await _listRepository.GetAllItemsAsync();
                Items = new ConcurrentDictionary<Guid, ListItem>(items.ToDictionary(item => item.Id, item => item));
            }
        }

        public bool ExistsItemWithId(Guid id)
            => Items.Keys.Contains(id);
    }
}

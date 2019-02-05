using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.ListItem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPerfectOnboarding.Services.Services
{
    internal class ListCache : IListCache
    {
        private readonly IListRepository _listRepository;
        internal ConcurrentDictionary<Guid, ListItem> Items = new ConcurrentDictionary<Guid, ListItem>();
        private bool _isInitialized;

        public ListCache(IListRepository listRepository)
        {
            _listRepository = listRepository;
        }

        public async Task<ListItem> AddItemAsync(ListItem item)
            => await ExecuteFunctionAsync(async () =>
            {
                Items.TryAdd(item.Id, item);
                Items.AddOrUpdate(item.Id, item, (id, oldItem) => item);

                return await _listRepository.AddItemAsync(item);
            });

        public async Task<ListItem> DeleteItemAsync(Guid id)
            => await ExecuteFunctionAsync(async () =>
            {
                if (!ExistsItemWithId(id))
                {
                    throw new ArgumentNullException();
                }

                Items.TryRemove(id, out _);

                return await _listRepository.DeleteItemAsync(id);
            });

        public async Task ReplaceItemAsync(ListItem item)
            => await ExecuteFunctionAsync(async () =>
            {
                if (!ExistsItemWithId(item.Id))
                {
                    throw new ArgumentNullException();
                }

                Items.TryGetValue(item.Id, out var oldItem);
                Items.TryUpdate(item.Id, item, oldItem);
                return await _listRepository.ReplaceItemAsync(item);
            });

        public async Task<IEnumerable<ListItem>> GetAllItemsAsync()
            => await ExecuteFunction(() => Items.Values);

        public async Task<ListItem> GetItemAsync(Guid id)
            => await ExecuteFunction(() =>
            {
                if (!ExistsItemWithId(id))
                {
                    throw new ArgumentNullException();
                }

                Items.TryGetValue(id, out var item);
                return item;
            });

        private async Task<T> ExecuteFunction<T>(Func<T> function)
        {
            await TryInitializeItemsAsync();

            return function();
        }

        private async Task<T> ExecuteFunctionAsync<T>(Func<Task<T>> operation)
        {
            await TryInitializeItemsAsync();

            return await operation();
        }

        private async Task ExecuteFunctionAsync(Func<Task> operation)
        {
            await TryInitializeItemsAsync();

            await operation();
        }

        private bool ExistsItemWithId(Guid id)
            => Items.Keys.Contains(id);

        public async Task<bool> ExistsItemWithIdAsync(Guid id)
            => await ExecuteFunction(() => ExistsItemWithId(id));

        internal async Task TryInitializeItemsAsync()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                var items = await _listRepository.GetAllItemsAsync();
                Items = new ConcurrentDictionary<Guid, ListItem>(items.ToDictionary(item => item.Id, item => item));
            }
        }
    }
}
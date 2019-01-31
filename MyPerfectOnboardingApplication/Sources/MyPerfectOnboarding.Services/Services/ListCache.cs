using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.ListItem;

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
        {
            /*await TryInitializeItemsAsync();

            var addedItem = item;
            Items.TryAdd(item.Id, addedItem);

            return await _listRepository.AddItemAsync(item);*/



            Func<Task<ListItem>> addItem = (async () =>
            {
                var addedItem = item;
                Items.TryAdd(item.Id, addedItem);

                return await _listRepository.AddItemAsync(item);
            });

            return await ExecuteFunctionAsync(addItem).Result;
        }

        public async Task<ListItem> DeleteItemAsync(Guid id)
        {
            /*await TryInitializeItemsAsync();

            if (!ExistsItemWithId(id))
                return null;

            //var deletedItem = await _listRepository.DeleteItemAsync(id);
            Items.TryRemove(id, out var deletedItem);
            return deletedItem;*/

            Func<Task<ListItem>> deleteItem = (async () =>
            {
                if (!ExistsItemWithId(id))
                    return null;

                Items.TryRemove(id, out _);

                return await _listRepository.DeleteItemAsync(id);
            });

            return await ExecuteFunctionAsync(deleteItem).Result;
        }

        public async Task ReplaceItemAsync(ListItem item)
        {
           /*await TryInitializeItemsAsync();

            if (!ExistsItemWithId(item.Id))
                return;

            Items.TryGetValue(item.Id, out var oldItem);
            Items.TryUpdate(item.Id, item, oldItem);
            await _listRepository.ReplaceItemAsync(item);*/


            Func<Task> replaceItem = (async () =>
            {
                if (!ExistsItemWithId(item.Id))
                    return;

                Items.TryGetValue(item.Id, out var oldItem);
                Items.TryUpdate(item.Id, item, oldItem);
                await _listRepository.ReplaceItemAsync(item);
            });

            await ExecuteFunctionAsync(replaceItem);
        }

        public async Task<IEnumerable<ListItem>> GetAllItemsAsync()
        {
            /*await TryInitializeItemsAsync();

            return Items.Values;*/

            Func<IEnumerable<ListItem>> getAll = (() => Items.Values);

            return await ExecuteFunctionAsync(getAll);
        }

        public async Task<ListItem> GetItemAsync(Guid id)
        {
           /*
           myFunc(() => { })

            await TryInitializeItemsAsync();

            if (!ExistsItemWithId(id))
            {
                return null;
            }

            Items.TryGetValue(id, out var item);
            return item;
            */

            Func<ListItem> getItem = () =>
            {
                if (!ExistsItemWithId(id))
                {
                    return null;
                }

                Items.TryGetValue(id, out var item);
                return item;
            };

            return await ExecuteFunctionAsync(getItem);
        }
        
        private async Task<T> ExecuteFunctionAsync<T>(Func<T> function)
        {
            await TryInitializeItemsAsync();

            return function();
        }

        public bool ExistsItemWithId(Guid id)
        {
            /*return Items.Keys.Contains(id);*/

            Func<bool> existsItem = () => Items.Keys.Contains(id);

            return ExecuteFunctionAsync(existsItem).Result;
        }

        private async Task TryInitializeItemsAsync()
        {
            if (!_isInitialized) {
                _isInitialized = true;
                var items = await _listRepository.GetAllItemsAsync();
                //foreach?
                Items = new ConcurrentDictionary<Guid, ListItem>(items.ToDictionary(item => item.Id, item => item));
            }
        }
    }
}

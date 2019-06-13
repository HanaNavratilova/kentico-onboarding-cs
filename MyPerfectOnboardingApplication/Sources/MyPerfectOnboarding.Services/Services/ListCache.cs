using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Services.ListItems;
using MyPerfectOnboarding.Services.Services.Extensions;

namespace MyPerfectOnboarding.Services.Services
{
    internal class ListCache : IListCache
    {
        private readonly IListRepository _listRepository;
        private readonly ICachedItemsProvider _cachedItemsProvider;

        public ListCache(IListRepository listRepository, ICachedItemsProvider cachedItemsProvider)
        {
            _listRepository = listRepository;
            _cachedItemsProvider = cachedItemsProvider;
        }

        public async Task<ListItem> AddItemAsync(ListItem item)
            => await _cachedItemsProvider.ExecuteOnItemsAsync(async items =>
            {
                items.AddOrUpdate(item.Id, item, (_, __) => item);

                return await _listRepository.AddItemAsync(item);
            });

        public async Task<ListItem> DeleteItemAsync(Guid id)
            => await _cachedItemsProvider.ExecuteOnItemsAsync(async items =>
            {
                if (!items.ExistsItemWithId(id))
                {
                    throw new ArgumentException($"Item with id: {id} was not found.");
                }

                items.TryRemove(id, out _);

                return await _listRepository.DeleteItemAsync(id);
            });

        public async Task<ListItem> ReplaceItemAsync(ListItem item)
            => await _cachedItemsProvider.ExecuteOnItemsAsync(async items =>
            {
                if (!items.ExistsItemWithId(item.Id))
                {
                    throw new ArgumentException($"Item with id: {item.Id} was not found.");
                }

                items[item.Id] = item;
                return await _listRepository.ReplaceItemAsync(item);
            });

        public async Task<IEnumerable<ListItem>> GetAllItemsAsync()
            => await _cachedItemsProvider.ExecuteOnItems(items => items.Values);

        public async Task<ListItem> GetItemAsync(Guid id)
            => await _cachedItemsProvider.ExecuteOnItems(items =>
            {
                if (!items.ExistsItemWithId(id))
                {
                    throw new ArgumentException($"Item with id: {id} was not found.");
                }

                items.TryGetValue(id, out var item);
                return item;
            });

        public async Task<bool> ExistsItemWithIdAsync(Guid id)
            => await _cachedItemsProvider.ExecuteOnItems(items => items.ExistsItemWithId(id));
    }
}
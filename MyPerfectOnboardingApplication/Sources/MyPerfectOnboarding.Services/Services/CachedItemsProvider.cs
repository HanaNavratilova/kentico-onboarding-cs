using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.ListItems;

namespace MyPerfectOnboarding.Services.Services
{
    internal class CachedItemsProvider : ICachedItemsProvider
    {
        private readonly IListRepository _listRepository;
        private readonly Lazy<Task<ConcurrentDictionary<Guid, ListItem>>> _lazyItems;
        internal Task<ConcurrentDictionary<Guid, ListItem>> Items => _lazyItems.Value;

        public CachedItemsProvider(IListRepository listRepository)
        {
            _listRepository = listRepository;
            _lazyItems = new Lazy<Task<ConcurrentDictionary<Guid, ListItem>>>(LoadItemsFromRepositoryAsync);
        }

        public async Task<T> ExecuteOnItems<T>(Func<ConcurrentDictionary<Guid, ListItem>, T> function)
            => function(await Items);

        public async Task<T> ExecuteOnItemsAsync<T>(Func<ConcurrentDictionary<Guid, ListItem>, Task<T>> operation)
            => await operation(await Items);

        private async Task<ConcurrentDictionary<Guid, ListItem>> LoadItemsFromRepositoryAsync()
        {
            var items = await _listRepository.GetAllItemsAsync();
            return new ConcurrentDictionary<Guid, ListItem>(items.ToDictionary(item => item.Id, item => item));
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Contracts.Services.ListItems
{
    public interface ICachedItemsProvider
    {
        Task<T> ExecuteOnItems<T>(Func<ConcurrentDictionary<Guid, ListItem>, T> function);

        Task<T> ExecuteOnItemsAsync<T>(Func<ConcurrentDictionary<Guid, ListItem>, Task<T>> operation);
    }
}

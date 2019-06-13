using System;
using System.Collections.Concurrent;
using MyPerfectOnboarding.Contracts.Models;

namespace MyPerfectOnboarding.Services.Services.Extensions
{
    internal static class ConcurrentDictionaryExtensions
    {
        public static bool ExistsItemWithId(this ConcurrentDictionary<Guid, ListItem> items, Guid id)
            => items.Keys.Contains(id);
    }
}

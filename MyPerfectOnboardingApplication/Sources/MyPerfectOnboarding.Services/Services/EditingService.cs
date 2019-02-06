using System;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Contracts.Services.ListItem;

namespace MyPerfectOnboarding.Services.Services
{
    internal class EditingService : IEditingService
    {
        private readonly IListCache _cache;
        private readonly ITimeGenerator _timeGenerator;

        public EditingService(IListCache cache, ITimeGenerator timeGenerator)
        {
            _cache = cache;
            _timeGenerator = timeGenerator;
        }

        public async Task ReplaceItemAsync(Guid id, ListItem editedItem)
        {
            var cachedItem = await _cache.GetItemAsync(id);

            var updatedItem = UpdateItem(cachedItem, editedItem);

            await _cache.ReplaceItemAsync(updatedItem);
        }

        private ListItem UpdateItem(ListItem itemToUpdate, ListItem editedItem)
            => itemToUpdate
                .With(item => item.Text, editedItem.Text)
                .With(item => item.IsActive, editedItem.IsActive)
                .With(item => item.LastUpdateTime, _timeGenerator.GetCurrentTime());
    }
}

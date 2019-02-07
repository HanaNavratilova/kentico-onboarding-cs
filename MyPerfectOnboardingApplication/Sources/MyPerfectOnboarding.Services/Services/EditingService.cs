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

        public async Task ReplaceItemAsync(Guid id, IListItem editedItem)
        {
            var cachedItem = await _cache.GetItemAsync(id);

            var updatedItem = UpdateItem(cachedItem, editedItem);
            var listItem = updatedItem.Build();

            await _cache.ReplaceItemAsync(listItem);
        }

        private ListItem UpdateItem(IListItem itemToUpdate, IListItem editedItem)
            => new ListItem(itemToUpdate)
                .With(item => item.Text, editedItem.Text)
                .With(item => item.IsActive, editedItem.IsActive)
                .With(item => item.LastUpdateTime, _timeGenerator.GetCurrentTime());
    }
}

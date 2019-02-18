using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Contracts.Services.ListItems;

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

        public async Task<ListItem> ReplaceItemAsync(Guid id, ListItem editedItem)
        {
            ListItem cachedItem;
            try
            {
                cachedItem = await _cache.GetItemAsync(id);
            }
            catch (ArgumentException)
            {
                throw new KeyNotFoundException($"Item with id: {id} was not found.");
            }

            UpdateItem(cachedItem, editedItem);

            return await _cache.ReplaceItemAsync(cachedItem);
        }

        private void UpdateItem(ListItem itemToUpdate, ListItem editedItem) {
            itemToUpdate.Text = editedItem.Text;
            itemToUpdate.IsActive = editedItem.IsActive;
            itemToUpdate.LastUpdateTime = _timeGenerator.GetCurrentTime();
        }
    }
}

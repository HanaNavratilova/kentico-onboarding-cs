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

        public async Task ReplaceItemAsync(ListItem editedItem)
        {
            var cachedItem = await _cache.GetItemAsync(editedItem.Id);

            UpdateItem(cachedItem, editedItem);

            await _cache.ReplaceItemAsync(cachedItem);
        }

        private void UpdateItem(ListItem itemToUpdate, ListItem editedItem) {
            itemToUpdate.Text = editedItem.Text;
            itemToUpdate.IsActive = editedItem.IsActive;
            itemToUpdate.LastUpdateTime = _timeGenerator.GetCurrentTime();
        }
    }
}

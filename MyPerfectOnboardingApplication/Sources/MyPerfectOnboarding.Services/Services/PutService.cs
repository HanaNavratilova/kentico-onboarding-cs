using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Database.Generators;
using MyPerfectOnboarding.Contracts.Services.Database.Services;

namespace MyPerfectOnboarding.Services.Services
{
    internal class PutService : IPutService
    {
        private readonly IListCache _cache;
        private readonly ITimeGenerator _timeGenerator;

        public PutService(IListCache cache, ITimeGenerator timeGenerator)
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

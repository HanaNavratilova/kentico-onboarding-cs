using System;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Models;
using MyPerfectOnboarding.Contracts.Services.Generators;
using MyPerfectOnboarding.Contracts.Services.ListItem;

namespace MyPerfectOnboarding.Services.Services
{
    internal class AdditionService : IAdditionService
    {
        private readonly IListCache _cache;
        private readonly ITimeGenerator _timeGenerator;
        private readonly IGuidGenerator _guidGenerator;

        public AdditionService(IListCache cache, ITimeGenerator timeGenerator, IGuidGenerator guidGenerator)
        {
            _cache = cache;
            _timeGenerator = timeGenerator;
            _guidGenerator = guidGenerator;
        }

        public async Task<ListItem> AddItemAsync(ListItem item)
        {
            var newItem = await MakeItemCompleted(item);
            return await _cache.AddItemAsync(newItem);
        }

        private async Task<ListItem> MakeItemCompleted(ListItem item)
        {
            var time = _timeGenerator.GetCurrentTime();
            return new ListItem()
            {
                Id = await GetIdAsync(),
                Text = item.Text,
                IsActive = false,
                CreationTime = time,
                LastUpdateTime = time,
            };
        }

        private async Task<Guid> GetIdAsync()
        {
            var id = _guidGenerator.Generate();
            while (await _cache.GetItemAsync(id) != null)
            {
                id = _guidGenerator.Generate();
            }

            return id;
        }
    }
}

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
            await MakeItemCompleted(item);
            return await _cache.AddItemAsync(item);
        }

        private async Task MakeItemCompleted(ListItem item)
        {
            item.Id = await GetIdAsync();
            item.IsActive = false;
            var time = _timeGenerator.GetCurrentTime();
            item.CreationTime = time;
            item.LastUpdateTime = time;
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

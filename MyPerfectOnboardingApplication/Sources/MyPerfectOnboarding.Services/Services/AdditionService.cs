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

        public async Task<IListItem> AddItemAsync(IListItem item)
        {
            var newItem = await MakeItemCompleted(item);
            var listItem = new ListItemBuilder().BuildItem(newItem);

            return await _cache.AddItemAsync(listItem);
        }

        private async Task<ListItem> MakeItemCompleted(IListItem incompleteItem)
        {
            var time = _timeGenerator.GetCurrentTime();
            return new ListItem(incompleteItem)
                .With(item => item.Id, await GetIdAsync())
                .With(item => item.IsActive, false)
                .With(item => item.CreationTime, time)
                .With(item => item.LastUpdateTime, time);
        }

        private async Task<Guid> GetIdAsync()
        {
            var id = _guidGenerator.Generate();
            while (await _cache.ExistsItemWithIdAsync(id))
            {
                id = _guidGenerator.Generate();
            }

            return id;
        }
    }
}

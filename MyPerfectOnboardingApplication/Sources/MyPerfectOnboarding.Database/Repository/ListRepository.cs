using MongoDB.Driver;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyPerfectOnboarding.Database.Models;

namespace MyPerfectOnboarding.Database.Repository
{
    internal class ListRepository : IListRepository
    {
        private readonly IMongoCollection<IListItem> _collection;

        private const string DatabaseCollectionName = "items";

        public ListRepository(IConnectionDetails connectionDetails)
        {
            var url = MongoUrl.Create(connectionDetails.DataConnectionString);
            var client = new MongoClient(url);
            var db = client.GetDatabase(url.DatabaseName);
            _collection = db.GetCollection<IListItem>(DatabaseCollectionName);
        }

        public async Task<ListItem> AddItemAsync(ListItem item)
        {
            await _collection.InsertOneAsync(new ListItemModel(item));
            return item;
        }

        public async Task<ListItem> DeleteItemAsync(Guid id)
            => await _collection
                .FindOneAndDeleteAsync(item => item.Id == id)
                .AsImmutable();

        public async Task<ListItem> ReplaceItemAsync(ListItem editedItem)
        {
            // Database returns from FindAndReplace function original item. However, the edited item should be returned.
            await _collection
                .FindOneAndReplaceAsync(
                    item => item.Id == editedItem.Id,
                    new ListItemModel(editedItem)
                    )
                .AsImmutable();
            return editedItem;
        }

        public async Task<IEnumerable<ListItem>> GetAllItemsAsync()
            => await _collection
                .Find(FilterDefinition<IListItem>.Empty)
                .ToListAsync()
                .AsImmutable();

        public async Task<ListItem> GetItemAsync(Guid itemId)
            => await _collection
                .Find(item => item.Id == itemId)
                .FirstOrDefaultAsync()
                .AsImmutable();
    }
}

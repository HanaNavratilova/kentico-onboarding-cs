using MongoDB.Driver;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyPerfectOnboarding.Contracts.Services.Database;

namespace MyPerfectOnboarding.Database.Repository
{
    internal class ListRepository : IListRepository
    {
        private readonly IMongoCollection<ListItem> _collection;

        public ListRepository(IConnectionDetails connectionDetails)
        {
            var url = MongoUrl.Create(connectionDetails.DataConnectionString);
            var client = new MongoClient(url);
            var db = client.GetDatabase(url.DatabaseName);
            _collection = db.GetCollection<ListItem>("items");
        }

        public async Task<ListItem> AddItemAsync(ListItem item)
        {
            MakeItemCompleted(item);
            await _collection.InsertOneAsync(item);
            return item;
        }

        public async Task<ListItem> DeleteItemAsync(Guid id)
        {
            var deletedItem = await _collection.FindOneAndDeleteAsync(item => item.Id == id);
            return deletedItem;
        }

        public async Task ReplaceItemAsync(ListItem editedItem)
        {
            var update = Builders<ListItem>.Update
                .Set("Text", editedItem.Text)
                .Set("IsActive", editedItem.IsActive)
                .Set("LastUpdateTime", DateTime.UtcNow);
            await _collection.FindOneAndUpdateAsync(item => item.Id == editedItem.Id, update);
        }

        public async Task<IEnumerable<ListItem>> GetAllItemsAsync()
        {
            var items = await _collection.FindAsync(FilterDefinition<ListItem>.Empty);
            return items.ToEnumerable();
        }

        public async Task<ListItem> GetItemAsync(Guid itemId)
        {
            var it = await _collection.FindAsync(item => item.Id == itemId);
            return it.FirstOrDefault();
        }

        private static void MakeItemCompleted(ListItem item)
        {
            item.Id = Guid.NewGuid();
            item.IsActive = false;
            var time = DateTime.UtcNow;
            item.CreationTime = time;
            item.LastUpdateTime = time;
        }
    }
}

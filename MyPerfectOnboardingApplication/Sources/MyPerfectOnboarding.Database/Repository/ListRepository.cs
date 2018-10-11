using MongoDB.Driver;
using MyPerfectOnboarding.Contracts.Database;
using MyPerfectOnboarding.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyPerfectOnboarding.Database.Repository
{
    internal class ListRepository : IListRepository
    {
        private readonly IMongoCollection<ListItem> _collection;

        private const string NameOfDBCollection = "items";

        public ListRepository(IConnectionDetails connectionDetails)
        {
            var url = MongoUrl.Create(connectionDetails.DataConnectionString);
            var client = new MongoClient(url);
            var db = client.GetDatabase(url.DatabaseName);
            _collection = db.GetCollection<ListItem>(NameOfDBCollection);
        }

        public async Task<ListItem> AddItemAsync(ListItem item)
        {
            await _collection.InsertOneAsync(item);
            return item;
        }

        public async Task<ListItem> DeleteItemAsync(Guid id)
            => await _collection.FindOneAndDeleteAsync(item => item.Id == id);

        public async Task<ListItem> ReplaceItemAsync(ListItem editedItem)
            => await _collection.FindOneAndReplaceAsync(item => item.Id == editedItem.Id, editedItem);

        public async Task<IEnumerable<ListItem>> GetAllItemsAsync()
            => await _collection.Find(FilterDefinition<ListItem>.Empty).ToListAsync();

        public async Task<ListItem> GetItemAsync(Guid itemId)
            => await _collection.Find(item => item.Id == itemId).FirstOrDefaultAsync();
    }
}

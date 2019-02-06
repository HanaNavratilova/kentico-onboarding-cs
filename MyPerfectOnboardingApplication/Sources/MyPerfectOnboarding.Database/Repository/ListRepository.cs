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
        private readonly IMongoCollection<ListItemModel> _collection;

        private const string NameOfDBCollection = "items";

        public ListRepository(IConnectionDetails connectionDetails)
        {
            var url = MongoUrl.Create(connectionDetails.DataConnectionString);
            var client = new MongoClient(url);
            var db = client.GetDatabase(url.DatabaseName);
            _collection = db.GetCollection<ListItemModel>(NameOfDBCollection);
        }

        public async Task<IListItem> AddItemAsync(IListItem item)
        {
            await _collection.InsertOneAsync(new ListItemModel(item));
            return item;
        }

        public async Task<IListItem> DeleteItemAsync(Guid id)
            => await _collection.FindOneAndDeleteAsync(item => item.Id == id);

        public async Task<IListItem> ReplaceItemAsync(IListItem editedItem)
            => await _collection.FindOneAndReplaceAsync(item => item.Id == editedItem.Id, new ListItemModel(editedItem));

        public async Task<IEnumerable<IListItem>> GetAllItemsAsync()
            => await _collection.Find(FilterDefinition<ListItemModel>.Empty).ToListAsync();

        public async Task<IListItem> GetItemAsync(Guid itemId)
            => await _collection.Find(item => item.Id == itemId).FirstOrDefaultAsync();
    }
}

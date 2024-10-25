using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace GrammarGuide.Services;

public class MongoCache(string cacheCollectionName, IMongoDatabase database) : ICache
{
    private class CacheItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
    }

    private readonly IMongoCollection<CacheItem> _collection = database.GetCollection<CacheItem>(cacheCollectionName);

    public async Task<T> Get<T>(string key)
    {
        var item = await _collection.Find(f => f.Key == key).FirstOrDefaultAsync();
        if (item == null)
            throw new Exception("Not found.");

        return JsonConvert.DeserializeObject<T>(item.Value);
    }

    public async Task<string> GetString(string key)
    {
        var item = await _collection.Find(f => f.Key == key).FirstOrDefaultAsync();
        if (item == null)
            throw new Exception("Not found.");

        return item.Value;
    }

    public async Task Set<T>(string key, T value)
    {
        var item = _collection.Find(f => f.Key == key).FirstOrDefault();
        if (item == null)
        {
            item = new CacheItem
            {
                Key = key,
                Value = JsonConvert.SerializeObject(value)
            };
            await _collection.InsertOneAsync(item);
        }
        else
        {
            item.Value = JsonConvert.SerializeObject(value);
            await _collection.ReplaceOneAsync(f => f.Key == key, item);
        }
    }

    public async Task Delete(string key)
    {
        await _collection.DeleteOneAsync(f => f.Key == key);
    }
}
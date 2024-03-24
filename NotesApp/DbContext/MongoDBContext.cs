using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

public class MongoDBContext
{
    private static readonly IMongoClient _client;
    private static readonly IMongoDatabase _database;

    static MongoDBContext()
    {
        var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase("notes");
    }

    public IMongoDatabase Database => _database;
}
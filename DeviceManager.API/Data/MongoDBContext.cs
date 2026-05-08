using DeviceManager.API.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace DeviceManager.API.Data;
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string DevicesCollectionName { get; set; } = "devices";
    public string UsersCollectionName { get; set; } = "users";
}
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
        EnsureIndexes();
    }
    public IMongoCollection<Device> Devices =>
        _database.GetCollection<Device>("devices");
    public IMongoCollection<User> Users =>
        _database.GetCollection<User>("users");
    private void EnsureIndexes()
    {
        var deviceNameIndex = new CreateIndexModel<Device>(
            Builders<Device>.IndexKeys.Ascending(d => d.Name),
            new CreateIndexOptions { Unique = true, Name = "idx_device_name_unique" }
        );

        // Index pe câmpuri folosite frecvent în search
        var deviceSearchIndex = new CreateIndexModel<Device>(
            Builders<Device>.IndexKeys
                .Text(d => d.Name)
                .Text(d => d.Manufacturer)
                .Text(d => d.Processor),
            new CreateIndexOptions { Name = "idx_device_text_search" }
        );

        Devices.Indexes.CreateMany(new[] { deviceNameIndex, deviceSearchIndex });

        // Index unic pe email — un utilizator = un cont
        var userEmailIndex = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true, Name = "idx_user_email_unique" }
        );

        Users.Indexes.CreateOne(userEmailIndex);
    }
}   
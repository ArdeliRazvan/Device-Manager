using DeviceManager.API.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace DeviceManager.API.Data;

/// <summary>
/// Configurare puternic tipizată pentru conexiunea MongoDB.
/// Valorile sunt citite din appsettings.json prin sistemul de configurare .NET.
/// </summary>
public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string DevicesCollectionName { get; set; } = "devices";
    public string UsersCollectionName { get; set; } = "users";
}

/// <summary>
/// MongoDbContext este echivalentul DbContext din Entity Framework.
/// Oferă acces la colecțiile MongoDB și centralizează logica de conectare.
/// 
/// Principiu SOLID aplicat: Single Responsibility — această clasă se ocupă
/// exclusiv de gestionarea conexiunii și expunerea colecțiilor.
/// </summary>
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        // MongoClient este thread-safe și trebuie instanțiat o singură dată
        // (înregistrat ca Singleton în DI container).
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);

        // Creăm indecșii la inițializare pentru performanță optimă.
        EnsureIndexes();
    }

    /// <summary>
    /// Expune colecția de dispozitive din MongoDB.
    /// </summary>
    public IMongoCollection<Device> Devices =>
        _database.GetCollection<Device>("devices");

    /// <summary>
    /// Expune colecția de utilizatori din MongoDB.
    /// </summary>
    public IMongoCollection<User> Users =>
        _database.GetCollection<User>("users");

    /// <summary>
    /// Creează indecșii necesari pentru performanță și unicitate.
    /// Metodă idempotentă — poate fi apelată de mai multe ori fără efecte secundare.
    /// </summary>
    private void EnsureIndexes()
    {
        // Index unic pe numele dispozitivului — previne duplicate
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
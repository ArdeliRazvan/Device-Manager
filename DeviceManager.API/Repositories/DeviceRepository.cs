using DeviceManager.API.Data;
using DeviceManager.API.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DeviceManager.API.Repositories;

/// <summary>
/// Implementarea concretă a IDeviceRepository.
/// Toate interacțiunile cu MongoDB sunt centralizate aici.
/// 
/// Repository Pattern: izolează logica de acces la date de logica de business
/// din Controller. Controller-ul nu cunoaște detaliile MongoDB.
/// 
/// Acest pattern este esențial pentru testabilitate: în Integration Tests
/// putem înlocui această clasă cu un mock fără a modifica controllere.
/// </summary>
public class DeviceRepository : IDeviceRepository
{
    private readonly IMongoCollection<Device> _collection;

    // Dependency Injection: MongoDbContext este injectat de containerul DI,
    // nu instanțiat manual. Aceasta respectă principiul DIP din SOLID.
    public DeviceRepository(MongoDbContext context)
    {
        _collection = context.Devices;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Device>> GetAllAsync()
    {
        // FindAsync cu FilterDefinition.Empty returnează toate documentele.
        // Sort descendent după createdAt — cele mai noi apar primele.
        return await _collection
            .Find(FilterDefinition<Device>.Empty)
            .SortByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Device?> GetByIdAsync(string id)
    {
        // Validăm că string-ul este un ObjectId valid înainte de query.
        // Fără această verificare, MongoDB.Driver aruncă o excepție de format.
        if (!ObjectId.TryParse(id, out _))
            return null;

        var filter = Builders<Device>.Filter.Eq(d => d.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<Device?> GetByNameAsync(string name)
    {
        // Regex case-insensitive pentru comparare flexibilă a numelor.
        var filter = Builders<Device>.Filter.Regex(
            d => d.Name,
            new BsonRegularExpression($"^{name}$", "i")
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByNameAsync(string name, string? excludeId = null)
    {
        var filter = Builders<Device>.Filter.Regex(
            d => d.Name,
            new BsonRegularExpression($"^{name}$", "i")
        );

        // La update, excludem dispozitivul curent din verificare
        if (excludeId != null)
        {
            filter &= Builders<Device>.Filter.Ne(d => d.Id, excludeId);
        }

        return await _collection.Find(filter).AnyAsync();
    }

    /// <inheritdoc />
    public async Task<Device> CreateAsync(Device device)
    {
        device.CreatedAt = DateTime.UtcNow;
        device.UpdatedAt = DateTime.UtcNow;

        await _collection.InsertOneAsync(device);

        // După inserare, MongoDB populează device.Id cu ObjectId-ul generat.
        return device;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(string id, Device device)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        device.UpdatedAt = DateTime.UtcNow;

        // ReplaceOneAsync înlocuiește întregul document (nu face merge).
        // Alternativa este UpdateOneAsync cu $set pentru update parțial.
        var filter = Builders<Device>.Filter.Eq(d => d.Id, id);
        var result = await _collection.ReplaceOneAsync(filter, device);

        // ModifiedCount > 0 înseamnă că documentul a fost găsit și modificat.
        return result.ModifiedCount > 0;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        var filter = Builders<Device>.Filter.Eq(d => d.Id, id);
        var result = await _collection.DeleteOneAsync(filter);

        return result.DeletedCount > 0;
    }
}
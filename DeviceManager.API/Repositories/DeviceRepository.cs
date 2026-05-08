using DeviceManager.API.Data;
using DeviceManager.API.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DeviceManager.API.Repositories;


public class DeviceRepository : IDeviceRepository
{
    private readonly IMongoCollection<Device> _collection;

    // MongoDbContext este injectat de containerul DI,
    public DeviceRepository(MongoDbContext context)
    {
        _collection = context.Devices;
    }
    public async Task<IEnumerable<Device>> GetAllAsync()
    {
        // FindAsync cu FilterDefinition.Empty returnează toate documentele.
        return await _collection
            .Find(FilterDefinition<Device>.Empty)
            .SortByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<Device?> GetByIdAsync(string id)
    {
        // Validăm că string-ul este un ObjectId valid înainte de query.
        if (!ObjectId.TryParse(id, out _))
            return null;

        var filter = Builders<Device>.Filter.Eq(d => d.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Device?> GetByNameAsync(string name)
    {
        var filter = Builders<Device>.Filter.Regex(
            d => d.Name,
            new BsonRegularExpression($"^{name}$", "i")
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
    public async Task<bool> ExistsByNameAsync(string name, string? excludeId = null)
    {
        var filter = Builders<Device>.Filter.Regex(
            d => d.Name,
            new BsonRegularExpression($"^{name}$", "i")
        );
        if (excludeId != null)
        {
            filter &= Builders<Device>.Filter.Ne(d => d.Id, excludeId);
        }

        return await _collection.Find(filter).AnyAsync();
    }
    public async Task<Device> CreateAsync(Device device)
    {
        device.CreatedAt = DateTime.UtcNow;
        device.UpdatedAt = DateTime.UtcNow;

        await _collection.InsertOneAsync(device);
        return device;
    }
    public async Task<bool> UpdateAsync(string id, Device device)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        device.UpdatedAt = DateTime.UtcNow;
        var filter = Builders<Device>.Filter.Eq(d => d.Id, id);
        var result = await _collection.ReplaceOneAsync(filter, device);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        var filter = Builders<Device>.Filter.Eq(d => d.Id, id);
        var result = await _collection.DeleteOneAsync(filter);

        return result.DeletedCount > 0;
    }
}
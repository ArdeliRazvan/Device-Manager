using DeviceManager.API.Data;
using DeviceManager.API.Models;
using DeviceManager.API.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

public class UserRepository : IUserRepository  
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(MongoDbContext context)
    {
        _collection = context.Users;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _collection
            .Find(FilterDefinition<User>.Empty)
            .SortBy(u => u.Name)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _)) return null;
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
    public async Task<User?> GetByEmailAsync(string email)
    {
        var filter = Builders<User>.Filter.Regex(
            u => u.Email,
            new BsonRegularExpression($"^{email}", "i"));
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<User?> CreateAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
         await _collection.InsertOneAsync(user);
        return user;

    }
}
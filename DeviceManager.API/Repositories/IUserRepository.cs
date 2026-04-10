using DeviceManager.API.Models;

namespace DeviceManager.API.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string id);
    Task<User?> CreateAsync(User user);
}
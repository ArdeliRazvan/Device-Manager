using DeviceManager.API.Models;

namespace DeviceManager.API.Repositories;

public interface IDeviceRepository
{
    Task<IEnumerable<Device>> GetAllAsync();

    Task<Device?> GetByIdAsync(string id);

    
    Task<Device?> GetByNameAsync(string name);

    Task<Device> CreateAsync(Device device);


    Task<bool> UpdateAsync(string id, Device device);

    Task<bool> DeleteAsync(string id);

    Task<bool> ExistsByNameAsync(string name, string? excludeId = null);
}
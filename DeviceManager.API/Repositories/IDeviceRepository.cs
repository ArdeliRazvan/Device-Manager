using DeviceManager.API.Models;

namespace DeviceManager.API.Repositories;

/// <summary>
/// Interfața repository-ului pentru Device.
/// 
/// Principiu SOLID aplicat:
/// - DIP (Dependency Inversion): Controller-ul depinde de această abstracție,
///   nu de implementarea concretă. Aceasta facilitează testarea (putem injecta
///   un mock în Integration Tests) și înlocuirea implementării fără a modifica codul client.
/// - ISP (Interface Segregation): interfața expune doar operațiile relevante pentru Device.
/// </summary>
public interface IDeviceRepository
{
    /// <summary>
    /// Returnează toate dispozitivele din colecție.
    /// </summary>
    Task<IEnumerable<Device>> GetAllAsync();

    /// <summary>
    /// Returnează un dispozitiv după ID-ul său MongoDB (ObjectId ca string).
    /// Returnează null dacă nu există.
    /// </summary>
    Task<Device?> GetByIdAsync(string id);

    /// <summary>
    /// Returnează un dispozitiv după nume (căutare exactă, case-insensitive).
    /// Folosit pentru validarea duplicatelor la inserare.
    /// </summary>
    Task<Device?> GetByNameAsync(string name);

    /// <summary>
    /// Inserează un dispozitiv nou și returnează entitatea cu ID-ul generat.
    /// </summary>
    Task<Device> CreateAsync(Device device);

    /// <summary>
    /// Actualizează un dispozitiv existent.
    /// Returnează true dacă actualizarea a reușit, false dacă ID-ul nu există.
    /// </summary>
    Task<bool> UpdateAsync(string id, Device device);

    /// <summary>
    /// Șterge un dispozitiv după ID.
    /// Returnează true dacă ștergerea a reușit, false dacă ID-ul nu există.
    /// </summary>
    Task<bool> DeleteAsync(string id);

    /// <summary>
    /// Verifică dacă un dispozitiv cu același nume există deja (pentru validare la insert).
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, string? excludeId = null);
}
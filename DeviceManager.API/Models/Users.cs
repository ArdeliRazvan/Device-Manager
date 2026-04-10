using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DeviceManager.API.Models;

/// <summary>
/// Entitatea User stochează datele de autentificare și profilul utilizatorului.
/// Parola este stocată exclusiv ca hash (BCrypt) — niciodată în clar.
/// </summary>
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Rolul utilizatorului în sistem: "admin" sau "user".
    /// Folosit pentru autorizare în Faza 3.
    /// </summary>
    [BsonElement("role")]
    public string Role { get; set; } = "user";

    [BsonElement("location")]
    public string Location { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash-ul parolei generat cu BCrypt. Nu se expune niciodată prin API.
    /// </summary>
    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
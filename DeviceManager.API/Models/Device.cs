using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DeviceManager.API.Models;
public class Device
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("manufacturer")]
    public string Manufacturer { get; set; } = string.Empty;
    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;

    [BsonElement("os")]
    public string OS { get; set; } = string.Empty;

    [BsonElement("osVersion")]
    public string OSVersion { get; set; } = string.Empty;

    [BsonElement("processor")]
    public string Processor { get; set; } = string.Empty;
    [BsonElement("ram")]
    public int RAM { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

 
    [BsonElement("assignedUserId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? AssignedUserId { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
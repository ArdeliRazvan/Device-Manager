using System.ComponentModel.DataAnnotations;

namespace DeviceManager.API.DTOs;

// ---------------------------------------------------------------------------
// DTO-urile (Data Transfer Objects) separă modelul intern de baza de date
// de contractul public al API-ului. Aceasta respectă principiul SRP (SOLID)
// și previne expunerea accidentală a câmpurilor interne (ex: passwordHash).
// ---------------------------------------------------------------------------

/// <summary>
/// DTO folosit la crearea unui dispozitiv nou (POST /api/devices).
/// Conține validări prin Data Annotations pentru a garanta integritatea datelor
/// înainte ca acestea să ajungă la nivelul repository-ului.
/// </summary>
public class DeviceCreateDto
{
    [Required(ErrorMessage = "Numele este obligatoriu.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Numele trebuie să aibă între 2 și 100 de caractere.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Producătorul este obligatoriu.")]
    public string Manufacturer { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tipul dispozitivului este obligatoriu.")]
    [RegularExpression("^(phone|tablet)$", ErrorMessage = "Tipul trebuie să fie 'phone' sau 'tablet'.")]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sistemul de operare este obligatoriu.")]
    public string OS { get; set; } = string.Empty;

    [Required(ErrorMessage = "Versiunea OS este obligatorie.")]
    public string OSVersion { get; set; } = string.Empty;

    [Required(ErrorMessage = "Procesorul este obligatoriu.")]
    public string Processor { get; set; } = string.Empty;

    [Range(1, 64, ErrorMessage = "RAM-ul trebuie să fie între 1 și 64 GB.")]
    public int RAM { get; set; }

    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// DTO folosit la actualizarea unui dispozitiv existent (PUT /api/devices/{id}).
/// Toate câmpurile sunt opționale — se actualizează doar cele trimise (partial update).
/// </summary>
public class DeviceUpdateDto
{
    [StringLength(100, MinimumLength = 2)]
    public string? Name { get; set; }

    public string? Manufacturer { get; set; }

    [RegularExpression("^(phone|tablet)$", ErrorMessage = "Tipul trebuie să fie 'phone' sau 'tablet'.")]
    public string? Type { get; set; }

    public string? OS { get; set; }

    public string? OSVersion { get; set; }

    public string? Processor { get; set; }

    [Range(1, 64)]
    public int? RAM { get; set; }

    public string? Description { get; set; }
}

/// <summary>
/// DTO returnat de API ca răspuns (GET). Include ID-ul și timestamp-urile,
/// dar exclude câmpuri interne sensibile.
/// </summary>
public class DeviceResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string OS { get; set; } = string.Empty;
    public string OSVersion { get; set; } = string.Empty;
    public string Processor { get; set; } = string.Empty;
    public int RAM { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? AssignedUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
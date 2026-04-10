using DeviceManager.API.DTOs;
using DeviceManager.API.Models;
using DeviceManager.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeviceManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceRepository _repository;
    private readonly ILogger<DevicesController> _logger;

    public DevicesController(IDeviceRepository repository, ILogger<DevicesController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceResponseDto>>> GetAll()
    {
        var devices = await _repository.GetAllAsync();
        return Ok(devices.Select(MapToResponseDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DeviceResponseDto>> GetById(string id)
    {
        var device = await _repository.GetByIdAsync(id);
        if (device is null)
            return NotFound(new { message = $"Device '{id}' not found." });
        return Ok(MapToResponseDto(device));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<DeviceResponseDto>> Create([FromBody] DeviceCreateDto dto)
    {
        if (await _repository.ExistsByNameAsync(dto.Name))
            return Conflict(new { message = $"A device named '{dto.Name}' already exists." });

        var device = new Device
        {
            Name = dto.Name.Trim(),
            Manufacturer = dto.Manufacturer.Trim(),
            Type = dto.Type.ToLower(),
            OS = dto.OS.Trim(),
            OSVersion = dto.OSVersion.Trim(),
            Processor = dto.Processor.Trim(),
            RAM = dto.RAM,
            Description = dto.Description.Trim()
        };

        var created = await _repository.CreateAsync(device);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToResponseDto(created));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<DeviceResponseDto>> Update(string id, [FromBody] DeviceUpdateDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            return NotFound(new { message = $"Device '{id}' not found." });

        if (dto.Name is not null && !dto.Name.Equals(existing.Name, StringComparison.OrdinalIgnoreCase))
            if (await _repository.ExistsByNameAsync(dto.Name, excludeId: id))
                return Conflict(new { message = $"A device named '{dto.Name}' already exists." });

        if (dto.Name is not null) existing.Name = dto.Name.Trim();
        if (dto.Manufacturer is not null) existing.Manufacturer = dto.Manufacturer.Trim();
        if (dto.Type is not null) existing.Type = dto.Type.ToLower();
        if (dto.OS is not null) existing.OS = dto.OS.Trim();
        if (dto.OSVersion is not null) existing.OSVersion = dto.OSVersion.Trim();
        if (dto.Processor is not null) existing.Processor = dto.Processor.Trim();
        if (dto.RAM.HasValue) existing.RAM = dto.RAM.Value;
        if (dto.Description is not null) existing.Description = dto.Description.Trim();

        await _repository.UpdateAsync(id, existing);
        return Ok(MapToResponseDto(existing));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _repository.DeleteAsync(id);
        if (!success) return NotFound(new { message = $"Device '{id}' not found." });
        return NoContent();
    }

    // Asignează device-ul userului logat
    [HttpPost("{id}/assign")]
    [Authorize]
    public async Task<IActionResult> Assign(string id)
    {
        var device = await _repository.GetByIdAsync(id);
        if (device is null) return NotFound(new { message = "Device not found." });

        if (device.AssignedUserId is not null)
            return Conflict(new { message = "Device is already assigned to another user." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub");

        device.AssignedUserId = userId;
        await _repository.UpdateAsync(id, device);
        return Ok(new { message = "Device assigned successfully.", assignedUserId = userId });
    }

    // Deasignează device-ul — doar dacă e asignat userului logat
    [HttpPost("{id}/unassign")]
    [Authorize]
    public async Task<IActionResult> Unassign(string id)
    {
        var device = await _repository.GetByIdAsync(id);
        if (device is null) return NotFound(new { message = "Device not found." });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? User.FindFirstValue("sub");

        if (device.AssignedUserId != userId)
            return Forbid();

        device.AssignedUserId = null;
        await _repository.UpdateAsync(id, device);
        return Ok(new { message = "Device unassigned successfully." });
    }

    private static DeviceResponseDto MapToResponseDto(Device d) => new()
    {
        Id = d.Id!,
        Name = d.Name,
        Manufacturer = d.Manufacturer,
        Type = d.Type,
        OS = d.OS,
        OSVersion = d.OSVersion,
        Processor = d.Processor,
        RAM = d.RAM,
        Description = d.Description,
        AssignedUserId = d.AssignedUserId,
        CreatedAt = d.CreatedAt,
        UpdatedAt = d.UpdatedAt
    };
}
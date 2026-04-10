using DeviceManager.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UsersController(IUserRepository repository)
    {
        _repository = repository;
    }

    // GET /api/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _repository.GetAllAsync();
        return Ok(users.Select(u => new
        {
            id = u.Id,
            name = u.Name,
            role = u.Role,
            location = u.Location,
            email = u.Email
        }));
    }

    // GET /api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user is null) return NotFound(new { message = $"User '{id}' not found." });

        return Ok(new
        {
            id = user.Id,
            name = user.Name,
            role = user.Role,
            location = user.Location,
            email = user.Email
        });
    }
}
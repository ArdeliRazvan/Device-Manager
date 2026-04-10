using DeviceManager.API.DTOs;

namespace DeviceManager.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    string GenerateToken(string userId, string email, string role);
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DeviceManager.API.Data;
using DeviceManager.API.DTOs;
using DeviceManager.API.Models;
using DeviceManager.API.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DeviceManager.API.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // Verificăm că email-ul nu există deja
        var existing = await _userRepository.GetByEmailAsync(dto.Email);
        if (existing is not null)
            throw new InvalidOperationException("Email already in use.");

        var user = new User
        {
            Name = dto.Name.Trim(),
            Email = dto.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "user",
            Location = dto.Location.Trim()
        };

        await _userRepository.CreateAsync(user);

        var token = GenerateToken(user.Id!, user.Email, user.Role);
        return MapToResponse(user, token);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email.ToLower().Trim());

        // Verificăm email și parolă — același mesaj pentru ambele (securitate)
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = GenerateToken(user.Id!, user.Email, user.Role);
        return MapToResponse(user, token);
    }

    public string GenerateToken(string userId, string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpiryHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static AuthResponseDto MapToResponse(User user, string token) => new()
    {
        Token = token,
        UserId = user.Id!,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role
    };
}
using ConstructionManagement.Application.DTOs;

namespace ConstructionManagement.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> Login(LoginDto loginDto);
    Task<bool> RegisterAsync(RegisterDto registerDto);
}
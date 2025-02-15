using ConstructionManagement.Application.DTOs;
using ConstructionManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManagement.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var authResponse = await authService.Login(loginDto);
        if (authResponse == null) return Unauthorized("Invalid credentials.");

        return Ok(authResponse);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var success = await authService.RegisterAsync(registerDto);
        if (!success) return BadRequest("Username already exists.");
        return Ok("User registered successfully.");
    }

}
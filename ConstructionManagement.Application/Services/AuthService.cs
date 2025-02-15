using ConstructionManagement.Application.DTOs;
using ConstructionManagement.Application.Helpers;
using ConstructionManagement.Application.Interfaces.Repositories;
using ConstructionManagement.Application.Interfaces.Services;
using ConstructionManagement.Domain.Entities;

namespace ConstructionManagement.Application.Services;

public class AuthService(IUserRepository userRepository, JwtTokenGenerator tokenGenerator)
   : IAuthService
{
   private readonly IUserRepository _userRepository = userRepository;
   private readonly JwtTokenGenerator _tokenGenerator = tokenGenerator;

   public async Task<AuthResponseDto?> Login(LoginDto loginDto) 
   {
      var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
      if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
         return null;

      var accessToken = _tokenGenerator.GenerateToken(user);
      
      return new AuthResponseDto
      {
         Id = user.Id,
         Username = user.Username,
         Email = user.Email,
         Token = accessToken
      };
   }
   
   public async Task<bool> RegisterAsync(RegisterDto registerDto)
   {
      if (await _userRepository.IsEmailTakenAsync(registerDto.Username))
         return false;

      var user = new User
      {
         Username = registerDto.Username,
         Email = registerDto.Email,
         Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
      };

      await _userRepository.AddAsync(user);
      await _userRepository.SaveChangesAsync();
      return true;
   }
}
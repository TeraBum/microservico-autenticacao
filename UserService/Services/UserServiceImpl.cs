using UserService.DTOs;
using UserService.Models;
using UserService.Repositories;
using UserService.Configurations;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Services
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly JwtSettings _jwtSettings;

        public UserServiceImpl(IUserRepository repository, IOptions<JwtSettings> jwtSettings)
        {
            _repository = repository;
            _jwtSettings = jwtSettings.Value;
        }

        // 🔹 Registro de usuário
        public async Task<string?> Register(UserRegisterDto dto)
        {
            var existingUser = await _repository.GetByEmailAsync(dto.Email);
            if (existingUser != null) return null;

            var user = new User
            {
                Nome = dto.Nome,
                Email = dto.Email.ToLowerInvariant(), // garante lowercase
                SenhaHash = BcryptHelper.HashPassword(dto.Senha),
                Role = dto.Role
            };

            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();

            return GenerateToken(user);
        }

        // 🔹 Login de usuário
        public async Task<string?> Login(UserLoginDto dto)
        {
            var emailNormalized = dto.Email.ToLowerInvariant();
            var user = await _repository.GetByEmailAsync(emailNormalized);
            if (user == null || !BcryptHelper.VerifyPassword(dto.Senha, user.SenhaHash))
                return null;

            return GenerateToken(user);
        }

        // 🔹 Geração de token JWT
        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

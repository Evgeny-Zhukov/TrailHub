using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrailHub.API.Application.DTOs.Auth;
using TrailHub.API.Application.Interfaces;
using TrailHub.API.Domain.Entities;
using TrailHub.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TrailHub.API.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IHashingService _hashingService;
        private readonly IConfiguration _configuration;

        public AuthService(
            AppDbContext context,
            IHashingService hashingService,
            IConfiguration configuration)
        {
            _context = context;
            _hashingService = hashingService;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // Проверка уникальности логина и email
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == dto.Login || u.Email == dto.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("Пользователь с таким логином или email уже существует");
            }

            // Создание пользователя
            var user = new User
            {
                Login = dto.Login,
                Email = dto.Email,
                PasswordHash = _hashingService.Hash(dto.Password),
                Name = dto.Name,
                AvatarUrl = null,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Генерация токенов
            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(1440),
                UserId = user.Id.ToString(),
                Login = user.Login,
                Name = user.Name
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == dto.Login);

            if (user == null || !_hashingService.Verify(dto.Password, user.PasswordHash))
            {
                return null; // Неправильный логин или пароль
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(1440),
                UserId = user.Id.ToString(),
                Login = user.Login,
                Name = user.Name
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["Key"]!;
            var issuer = jwtSettings["Issuer"]!;
            var audience = jwtSettings["Audience"]!;
            var expiresInMinutes = int.Parse(jwtSettings["ExpiresInMinutes"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim("Name", user.Name)
        };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}

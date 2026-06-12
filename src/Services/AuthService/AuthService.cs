using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoAPI.src.DTOs;
using TodoAPI.src.Entities;
using TodoAPI.src.Repo.UserRepository;

namespace TodoAPI.src.Services.AuthService
{
    public class AuthService(IUserRepo userRepository, IConfiguration configuration) : IAuthService
    {

        public async Task<string> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default)
        {
            var entity = await userRepository.GetByUsernameAsync(dto.Username, cancellationToken);

            if (entity == null)
                throw new ArgumentException("Пользователя с таким именем не существует");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, entity.HashedPassword))
                throw new ArgumentException("Неверный пароль");

            return GenerateJWTToken(entity);
        }

        public string GenerateJWTToken(UserEntity entity)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, entity.UserId.ToString()),
                new Claim(ClaimTypes.Name, entity.Username)
            };

            var jwt = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(60)),
            signingCredentials: new SigningCredentials
            (
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}

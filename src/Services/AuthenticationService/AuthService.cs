using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Xml;
using TodoAPI.DTOs;
using TodoAPI.Entities;
using TodoAPI.Repo.UserRepository;
using TodoAPI.Services.TaskServices;

namespace TodoAPI.Services.AuthenticationService
{
    public class AuthService(IUserRepo userRepository, IConfiguration configuration) : IAuthService
    {
        private static readonly Serilog.ILogger _log = Log.ForContext<AuthService>();

        public async Task<string> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default)
        {
            var entity = await userRepository.GetByUsernameAsync(dto.Username, cancellationToken);

            if (entity == null)
            {
                _log.Information("{Username} пользователь не найден", dto.Username);
                throw new ArgumentException("Пользователя с таким именем не существует");
            }
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, entity.HashedPassword))
            {
                _log.Information("{Username} пользователь ввёл неверный пароль", dto.Username);
                throw new ArgumentException("Неверный пароль");
            }

            _log.Information("генерация jwt для пользователя {Username}, {UserId}", dto.Username, entity.UserId);
            return GenerateJWT(entity);
        }

        public string GenerateJWT(UserEntity entity)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, entity.UserId.ToString()),
                new Claim(ClaimTypes.Name, entity.Username),
                new Claim("iat", DateTime.UtcNow.ToString())
            };

            var jwt = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(60)),
            signingCredentials: new SigningCredentials
            (
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}

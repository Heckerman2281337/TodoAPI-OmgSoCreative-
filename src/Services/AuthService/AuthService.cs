using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TodoAPI.src.DTOs;
using TodoAPI.src.Entities;
using TodoAPI.src.Repo.UserRepository;

namespace TodoAPI.src.Services.AuthService
{
    public class AuthService(IUserRepo userRepository) : IAuthService
    {

        public async Task LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default)
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
            var jwt = new JwtSecurityToken(
            issuer: ,
            audience: ,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(60)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}

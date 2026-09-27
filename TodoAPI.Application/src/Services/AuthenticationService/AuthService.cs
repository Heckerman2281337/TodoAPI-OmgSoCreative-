using TodoAPI.Application.DTOs;
using TodoAPI.Application.Repo;
using Microsoft.Extensions.Logging;

namespace TodoAPI.Application.Services
{
    public class AuthService(IUserRepo userRepository, 
        ILogger<AuthService> logger, ITokenService tokenService) : IAuthService
    {
        public async Task<LoginResponseDTO> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default)
        {
            var userEntity = await userRepository.GetByUsernameAsync(dto.Username, cancellationToken);

            if (userEntity == null)
            {
                logger.LogInformation("{Username} пользователь не найден", dto.Username);
                throw new UnauthorizedAccessException("Ошибка, неверный пароль или логин");
            }
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, userEntity.HashedPassword))
            {
                logger.LogInformation("{Username} пользователь ввёл неверный пароль", dto.Username);
                throw new UnauthorizedAccessException("Ошибка, неверный пароль или логин");
            }

            logger.LogInformation("генерация jwt для пользователя {Username}, {UserId}", dto.Username, userEntity.UserId);

            var refreshToken = await tokenService.GenerateRefreshTokenAsync(userEntity);
            var accessToken = tokenService.GenerateAccessJWT(userEntity);

            var response = new LoginResponseDTO(refreshToken, accessToken);
            return response;
        }
        public async Task LogoutAsync(string token, Guid userId, CancellationToken cancellation = default)
        {
            var refreshToken = await tokenService.GetTokenAsync(token, cancellation);
            if (refreshToken is null)
                throw new UnauthorizedAccessException("Refresh токен не найден");
            if (refreshToken.UserId != userId)
                throw new UnauthorizedAccessException("Refresh токен не найден для юзера");
            await tokenService.RevokeAsync(refreshToken, cancellation);
        }
        public async Task<LoginResponseDTO> RefreshAsync(string rawToken, CancellationToken cancellationToken = default)
        {
            return await tokenService.RefreshAsync(rawToken, cancellationToken);
        }
    }
}

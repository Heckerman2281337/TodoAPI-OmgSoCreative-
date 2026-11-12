using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TodoAPI.Domain.Entities;
using TodoAPI.Application.Repo;
using TodoAPI.Application.DTOs;
using Microsoft.Extensions.Configuration;
using TodoAPI.Application.src.DTOs.Auth;

namespace TodoAPI.Application.Services
{
    public class TokenService(IConfiguration configuration, ITokenRepo tokenRepo, 
        IUserRepo userRepo, ICacheService cache) : ITokenService
    {

        public async Task<string> GenerateRefreshTokenAsync(UserEntity entity, CancellationToken cancellationToken = default)
        {
            byte[] bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            var rawToken = Convert.ToBase64String(bytes);
            var refreshToken = new RefreshTokenEntity(entity.UserId, HashRefreshToken(rawToken));
            await tokenRepo.CreateAsync(refreshToken, cancellationToken);

            var cachedToken = new CachedRefreshToken(
                refreshToken.Id,
                refreshToken.UserId,
                refreshToken.IsRevoked,
                refreshToken.ExpiresAt);
            string cachedKey = $"tokens:{refreshToken.TokenHash}";
            var ttl = refreshToken.ExpiresAt - DateTime.UtcNow;
            await cache.WriteInCacheAsync<CachedRefreshToken>(cachedKey, cachedToken, cancellationToken, ttl);

            return rawToken;
        }
        public string GenerateAccessJWT(UserEntity entity)
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
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(15)),
            signingCredentials: new SigningCredentials
            (
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        public async Task<LoginResponseDTO> RefreshAsync(string rawToken, CancellationToken cancellationToken = default)
        {
            var refreshToken = await GetTokenAsync(rawToken, cancellationToken);
            if (refreshToken is null)
                throw new UnauthorizedAccessException("Refresh токен не найден");
            if (refreshToken.IsRevoked)
                throw new UnauthorizedAccessException("Токен отозван");
            if (refreshToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Токен недействителен");

            var userEntity = await userRepo.GetByIdAsync(refreshToken.UserId, cancellationToken);
            if (userEntity is null)
                throw new ArgumentException("Такого пользователя не существует");

            if (!await tokenRepo.IsTokenAffected(refreshToken.Id, cancellationToken))
                throw new UnauthorizedAccessException("Токен использован");

            await RevokeCachedAsync(refreshToken, cancellationToken);

            var newRefreshToken = await GenerateRefreshTokenAsync(userEntity);
            var newAccessToken = GenerateAccessJWT(userEntity);

            var response = new LoginResponseDTO(newRefreshToken, newAccessToken);
            return response;
        }
        public async Task RevokeAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default)
        {
            refreshToken.RevokeToken();
            await tokenRepo.RevokeTokenAsync(refreshToken, cancellationToken);
            await RevokeCachedAsync(refreshToken, cancellationToken);
        }
        public async Task DeleteAsync(string rawToken, CancellationToken cancellationToken = default)
        {
            var hashedToken = HashRefreshToken(rawToken);
            var tokenEntity = await tokenRepo.GetByTokenAsync(hashedToken, cancellationToken);
            if (tokenEntity is null)
                throw new UnauthorizedAccessException("Refresh токен не найден");
            await cache.DeleteFromCacheAsync($"tokens:{hashedToken}", cancellationToken);
            await tokenRepo.DeleteAsync(tokenEntity, cancellationToken);
        }
        public async Task<RefreshTokenEntity> GetTokenAsync(string rawToken, CancellationToken cancellationToken = default)
        {
            var hashedToken = HashRefreshToken(rawToken);

            var cachedKey = $"tokens:{hashedToken}";
            var cachedData = await cache.ReadFromCacheAsync<CachedRefreshToken>(cachedKey, cancellationToken);

            if (cachedData is not null)
            {
                var tokenEntity = new RefreshTokenEntity(cachedData.UserId, hashedToken);
                tokenEntity.RestoreState(cachedData.Id, cachedData.ExpiresAt, cachedData.IsRevoked);
                return tokenEntity;
            }
            
            var refreshToken = await tokenRepo.GetByTokenAsync(hashedToken, cancellationToken);

            if (refreshToken is null)
                throw new UnauthorizedAccessException("Refresh токен не найден");

           var cachedToken = new CachedRefreshToken(
                refreshToken.Id,
                refreshToken.UserId,
                refreshToken.IsRevoked,
                refreshToken.ExpiresAt);
           var ttl = refreshToken.ExpiresAt - DateTime.UtcNow;
           await cache.WriteInCacheAsync<CachedRefreshToken>(cachedKey, cachedToken, cancellationToken, ttl);
           return refreshToken;
        }

        private async Task RevokeCachedAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default)
        {
            var cachedKey = $"tokens:{refreshToken.TokenHash}";
            var cachedToken = new CachedRefreshToken(
                refreshToken.Id,
                refreshToken.UserId,
                true,//revoked
                refreshToken.ExpiresAt);
            var ttl = refreshToken.ExpiresAt - DateTime.UtcNow;
            await cache.WriteInCacheAsync<CachedRefreshToken>(cachedKey, cachedToken, cancellationToken, ttl);
        }

        private static string HashRefreshToken(string token)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(token);
            byte[] hashBytes = SHA256.HashData(bytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}

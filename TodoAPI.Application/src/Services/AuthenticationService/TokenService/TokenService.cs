using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TodoAPI.Domain.Entities;
using TodoAPI.Application.Repo;
using TodoAPI.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using TodoAPI.Application.src.DTOs.Auth;
using System.Text.Json;

namespace TodoAPI.Application.Services
{
    public class TokenService(IConfiguration configuration, ITokenRepo tokenRepo, 
        IUserRepo userRepo, IDistributedCache cache) : ITokenService
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
            await cache.SetStringAsync(cachedKey,
                JsonSerializer.Serialize(cachedToken),
                new DistributedCacheEntryOptions { 
                    AbsoluteExpirationRelativeToNow = refreshToken.ExpiresAt - DateTime.UtcNow
                },
                cancellationToken);

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

           await RevokeAsync(refreshToken);

            var newRefreshToken = await GenerateRefreshTokenAsync(userEntity);
            var newAccessToken = GenerateAccessJWT(userEntity);

            var response = new LoginResponseDTO(newRefreshToken, newAccessToken);
            return response;
        }
        public async Task RevokeAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default)
        {
            refreshToken.RevokeToken();
            await tokenRepo.RevokeTokenAsync(refreshToken, cancellationToken);

            var cachedKey = $"tokens:{refreshToken.TokenHash}";
            var cachedToken = new CachedRefreshToken(
                refreshToken.Id,
                refreshToken.UserId,
                true,//revoked
                refreshToken.ExpiresAt);
            await cache.SetStringAsync(cachedKey,
                JsonSerializer.Serialize(cachedToken),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = refreshToken.ExpiresAt - DateTime.UtcNow
                },
                cancellationToken);
        }
        public async Task DeleteAsync(string rawToken, CancellationToken cancellationToken = default)
        {
            var hashedToken = HashRefreshToken(rawToken);
            var tokenEntity = await tokenRepo.GetByTokenAsync(hashedToken, cancellationToken);
            if (tokenEntity is null)
                throw new UnauthorizedAccessException("Refresh токен не найден");
            await cache.RemoveAsync($"tokens:{hashedToken}", cancellationToken);
            await tokenRepo.DeleteAsync(tokenEntity, cancellationToken);
        }
        public async Task<RefreshTokenEntity> GetTokenAsync(string rawToken, CancellationToken cancellationToken = default)
        {
            var hashedToken = HashRefreshToken(rawToken);

            var cachedKey = $"tokens:{hashedToken}";
            var cachedData = await cache.GetStringAsync(cachedKey, cancellationToken);

            if (cachedData is not null)
            {
                var cached = JsonSerializer.Deserialize<CachedRefreshToken>(cachedData);
                if (cached is null)
                    throw new UnauthorizedAccessException("Refresh токен не найден");

                var tokenEntity = new RefreshTokenEntity(cached.UserId, hashedToken);
                tokenEntity.RestoreState(cached.Id,cached.ExpiresAt, cached.IsRevoked);
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
            await cache.SetStringAsync(cachedKey,
                JsonSerializer.Serialize(cachedToken),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = refreshToken.ExpiresAt - DateTime.UtcNow
                },
                cancellationToken);

            return refreshToken;
        }
        private static string HashRefreshToken(string token)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(token);
            byte[] hashBytes = SHA256.HashData(bytes);
            return Convert.ToHexString(hashBytes);
        }
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Moq;
using TodoAPI.Application.DTOs;
using TodoAPI.Application.Repo;
using TodoAPI.Application.Services;
using TodoAPI.Application.src.DTOs.Auth;
using TodoAPI.Domain.Entities;

namespace TodoTests.Tokens
{
    public class TokenServiceTests
    {
        public TokenServiceTests()
        {
            _configurationMock.Setup(c => c["Jwt:Key"])
                .Returns("super-secret-test-key-at-least-32-chars-long!");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

            // По умолчанию кэш пустой
            _cacheMock
                .Setup(c => c.ReadFromCacheAsync<CachedRefreshToken>(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((CachedRefreshToken?)null);

            _sut = new TokenService(
                _configurationMock.Object,
                _tokenRepoMock.Object,
                _userRepoMock.Object,
                _cacheMock.Object);
        }

        private readonly Mock<IConfiguration> _configurationMock = new();
        private readonly Mock<ITokenRepo> _tokenRepoMock = new();
        private readonly Mock<IUserRepo> _userRepoMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();

        private readonly TokenService _sut;

        // ---------- helpers ----------

        private static string Hash(string raw) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));

        private static string Key(string raw) => $"tokens:{Hash(raw)}";

        private static UserEntity CreateUser() =>
            new("admin", "hash", "abc@mail.ru");

        private void SetupCacheHit(string raw, CachedRefreshToken cached) =>
            _cacheMock
                .Setup(c => c.ReadFromCacheAsync<CachedRefreshToken>(
                    Key(raw),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cached);

        private void SetupRepoToken(RefreshTokenEntity? entity) =>
            _tokenRepoMock
                .Setup(r => r.GetByTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

        private void VerifyCacheWrite(string key, Func<CachedRefreshToken, bool> match, Times times) =>
            _cacheMock.Verify(
                c => c.WriteInCacheAsync<CachedRefreshToken>(
                    key,
                    It.Is<CachedRefreshToken>(t => match(t)),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TimeSpan>()),
                times);

        private void VerifyNoCacheWrites() =>
            _cacheMock.Verify(
                c => c.WriteInCacheAsync<CachedRefreshToken>(
                    It.IsAny<string>(),
                    It.IsAny<CachedRefreshToken>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TimeSpan>()),
                Times.Never());

        // ---------- GenerateRefreshTokenAsync ----------

        [Fact]
        public async Task GenerateRefreshTokenAsync_SavesRefreshToken()
        {
            var user = CreateUser();

            var token = await _sut.GenerateRefreshTokenAsync(user);

            Assert.False(string.IsNullOrWhiteSpace(token));

            _tokenRepoMock.Verify(
                r => r.CreateAsync(
                    It.IsAny<RefreshTokenEntity>(),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public async Task GenerateRefreshTokenAsync_StoresHashNotRawToken()
        {
            var user = CreateUser();
            RefreshTokenEntity? saved = null;

            _tokenRepoMock
                .Setup(r => r.CreateAsync(
                    It.IsAny<RefreshTokenEntity>(),
                    It.IsAny<CancellationToken>()))
                .Callback<RefreshTokenEntity, CancellationToken>((t, _) => saved = t);

            var raw = await _sut.GenerateRefreshTokenAsync(user);

            Assert.NotNull(saved);
            Assert.Equal(user.UserId, saved.UserId);
            Assert.Equal(Hash(raw), saved.TokenHash);
            Assert.NotEqual(raw, saved.TokenHash);
        }

        [Fact]
        public async Task GenerateRefreshTokenAsync_WritesTokenToCache()
        {
            var user = CreateUser();

            var raw = await _sut.GenerateRefreshTokenAsync(user);

            _cacheMock.Verify(
                c => c.WriteInCacheAsync<CachedRefreshToken>(
                    Key(raw),
                    It.Is<CachedRefreshToken>(t => t.UserId == user.UserId && !t.IsRevoked),
                    It.IsAny<CancellationToken>(),
                    It.Is<TimeSpan>(ttl => ttl > TimeSpan.Zero)),
                Times.Once());
        }

        [Fact]
        public async Task GenerateRefreshTokenAsync_ReturnsUniqueTokens()
        {
            var user = CreateUser();

            var first = await _sut.GenerateRefreshTokenAsync(user);
            var second = await _sut.GenerateRefreshTokenAsync(user);

            Assert.NotEqual(first, second);
        }

        // ---------- GenerateAccessJWT ----------

        [Fact]
        public void GenerateAccessJWT_ContainsUserClaimsIssuerAndAudience()
        {
            var user = CreateUser();

            var jwt = _sut.GenerateAccessJWT(user);

            var parsed = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

            Assert.Equal(user.UserId.ToString(),
                parsed.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Assert.Equal(user.Username,
                parsed.Claims.First(c => c.Type == ClaimTypes.Name).Value);
            Assert.Equal("test-issuer", parsed.Issuer);
            Assert.Contains("test-audience", parsed.Audiences);
        }

        [Fact]
        public void GenerateAccessJWT_ExpiresInAbout15Minutes()
        {
            var user = CreateUser();
            var before = DateTime.UtcNow;

            var jwt = _sut.GenerateAccessJWT(user);

            var parsed = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

            Assert.InRange(
                parsed.ValidTo,
                before.AddMinutes(14),
                DateTime.UtcNow.AddMinutes(16));
        }

        // ---------- GetTokenAsync ----------

        [Fact]
        public async Task GetTokenAsync_ReturnsFromCache_WithoutHittingRepo_WhenCacheHit()
        {
            var raw = "raw-token";
            var cached = new CachedRefreshToken(
                Guid.NewGuid(), Guid.NewGuid(), false, DateTime.UtcNow.AddDays(7));
            SetupCacheHit(raw, cached);

            var result = await _sut.GetTokenAsync(raw);

            Assert.Equal(cached.Id, result.Id);
            Assert.Equal(cached.UserId, result.UserId);
            Assert.Equal(cached.IsRevoked, result.IsRevoked);
            Assert.Equal(cached.ExpiresAt, result.ExpiresAt);
            Assert.Equal(Hash(raw), result.TokenHash);

            _tokenRepoMock.Verify(
                r => r.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never());
        }

        [Fact]
        public async Task GetTokenAsync_LoadsFromRepoAndCachesIt_WhenCacheMiss()
        {
            var raw = "raw-token";
            var entity = new RefreshTokenEntity(Guid.NewGuid(), Hash(raw));
            SetupRepoToken(entity);

            var result = await _sut.GetTokenAsync(raw);

            Assert.Same(entity, result);

            _tokenRepoMock.Verify(
                r => r.GetByTokenAsync(Hash(raw), It.IsAny<CancellationToken>()),
                Times.Once());
            VerifyCacheWrite(
                Key(raw),
                t => t.Id == entity.Id && t.UserId == entity.UserId && !t.IsRevoked,
                Times.Once());
        }

        [Fact]
        public async Task GetTokenAsync_Throws_WhenNotInCacheAndNotInRepo()
        {
            SetupRepoToken(null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.GetTokenAsync("raw-token"));

            VerifyNoCacheWrites();
        }

        // ---------- RefreshAsync ----------

        [Fact]
        public async Task RefreshAsync_Throws_WhenTokenNotFound()
        {
            SetupRepoToken(null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.RefreshAsync("token"));
        }

        [Fact]
        public async Task RefreshAsync_Throws_WhenRevoked()
        {
            var token = new RefreshTokenEntity(Guid.NewGuid(), "hash");
            token.RevokeToken();
            SetupRepoToken(token);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.RefreshAsync("token"));
        }

        [Fact]
        public async Task RefreshAsync_Throws_WhenRevokedInCache_AndDoesNotHitRepo()
        {
            var raw = "raw-token";
            SetupCacheHit(raw, new CachedRefreshToken(
                Guid.NewGuid(), Guid.NewGuid(), true, DateTime.UtcNow.AddDays(7)));

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.RefreshAsync(raw));

            _tokenRepoMock.Verify(
                r => r.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never());
        }

        [Fact]
        public async Task RefreshAsync_Throws_WhenExpired()
        {
            var raw = "raw-token";
            SetupCacheHit(raw, new CachedRefreshToken(
                Guid.NewGuid(), Guid.NewGuid(), false, DateTime.UtcNow.AddDays(-1)));

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.RefreshAsync(raw));

            _tokenRepoMock.Verify(
                r => r.CreateAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()),
                Times.Never());
        }

        [Fact]
        public async Task RefreshAsync_Throws_WhenUserDoesntExist()
        {
            var raw = "raw-token";
            var cached = new CachedRefreshToken(
                Guid.NewGuid(), Guid.NewGuid(), false, DateTime.UtcNow.AddDays(7));
            SetupCacheHit(raw, cached);

            _userRepoMock
                .Setup(r => r.GetByIdAsync(cached.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.RefreshAsync(raw));
        }

        [Fact]
        public async Task RefreshAsync_Throws_WhenTokenAlreadyUsed()
        {
            var raw = "raw-token";
            var user = CreateUser();
            var cached = new CachedRefreshToken(
                Guid.NewGuid(), user.UserId, false, DateTime.UtcNow.AddDays(7));
            SetupCacheHit(raw, cached);

            _userRepoMock
                .Setup(r => r.GetByIdAsync(user.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _tokenRepoMock
                .Setup(r => r.IsTokenAffected(cached.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.RefreshAsync(raw));

            _tokenRepoMock.Verify(
                r => r.CreateAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()),
                Times.Never());
            VerifyNoCacheWrites();
        }

        [Fact]
        public async Task RefreshAsync_ReturnsNewTokens_AndMarksOldTokenRevokedInCache()
        {
            var raw = "raw-token";
            var user = CreateUser();
            var cached = new CachedRefreshToken(
                Guid.NewGuid(), user.UserId, false, DateTime.UtcNow.AddDays(7));
            SetupCacheHit(raw, cached);

            _userRepoMock
                .Setup(r => r.GetByIdAsync(user.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _tokenRepoMock
                .Setup(r => r.IsTokenAffected(cached.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _sut.RefreshAsync(raw);

            Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
            Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
            Assert.NotEqual(raw, result.RefreshToken);

            // старый токен помечен как отозванный в кэше
            VerifyCacheWrite(Key(raw), t => t.Id == cached.Id && t.IsRevoked, Times.Once());
            // новый refresh токен сохранён
            _tokenRepoMock.Verify(
                r => r.CreateAsync(
                    It.Is<RefreshTokenEntity>(t => t.UserId == user.UserId),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        // ---------- RevokeAsync ----------

        [Fact]
        public async Task RevokeAsync_UpdatesRepository()
        {
            var entity = new RefreshTokenEntity(Guid.NewGuid(), "hash");

            await _sut.RevokeAsync(entity);

            Assert.True(entity.IsRevoked);

            _tokenRepoMock.Verify(
                r => r.RevokeTokenAsync(
                    entity,
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public async Task RevokeAsync_MarksTokenRevokedInCache()
        {
            var entity = new RefreshTokenEntity(Guid.NewGuid(), "hash");

            await _sut.RevokeAsync(entity);

            VerifyCacheWrite(
                "tokens:hash",
                t => t.Id == entity.Id && t.UserId == entity.UserId && t.IsRevoked,
                Times.Once());
        }

        // ---------- DeleteAsync ----------

        [Fact]
        public async Task DeleteAsync_DeletesToken()
        {
            var entity = new RefreshTokenEntity(Guid.NewGuid(), "hash");
            SetupRepoToken(entity);

            await _sut.DeleteAsync("token");

            _tokenRepoMock.Verify(
                r => r.DeleteAsync(
                    entity,
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_RemovesTokenFromCache()
        {
            var raw = "token";
            var entity = new RefreshTokenEntity(Guid.NewGuid(), Hash(raw));
            SetupRepoToken(entity);

            await _sut.DeleteAsync(raw);

            _cacheMock.Verify(
                c => c.DeleteFromCacheAsync(
                    Key(raw),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_Throws_WhenTokenNotFound()
        {
            SetupRepoToken(null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.DeleteAsync("token"));

            _tokenRepoMock.Verify(
                r => r.DeleteAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()),
                Times.Never());
            _cacheMock.Verify(
                c => c.DeleteFromCacheAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never());
        }
    }
}
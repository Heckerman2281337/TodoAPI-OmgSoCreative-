using Microsoft.Extensions.Logging;
using Moq;
using TodoAPI.Application.DTOs;
using TodoAPI.Domain.Entities;
using TodoAPI.Application.Repo;
using TodoAPI.Application.Services;

namespace TodoTests.Tests.User
{
    public class AuthServiceTests
    {
        public AuthServiceTests()
        {
            _sut = new AuthService(_userRepoMock.Object, _loggerMock.Object,
                _tokenServiceMock.Object);
        }

        private readonly Mock<IUserRepo> _userRepoMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<ILogger<AuthService>> _loggerMock = new();

        private readonly AuthService _sut;

        // ---------- LoginAsync ----------

        [Fact]
        public async Task LoginAsync_ThrowsUnauthorizedAccessException_WhenUserDoesntExist()
        {
            var dto = new LoginDTO("admin", "123");

            _userRepoMock
                .Setup(r => r.GetByUsernameAsync(dto.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.LoginAsync(dto));

            _tokenServiceMock.Verify(
                t => t.GenerateRefreshTokenAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ThrowsUnauthorizedAccessException_WhenPasswordInvalid()
        {
            var dto = new LoginDTO("admin", "wrong");

            var user = new UserEntity(
                "admin",
                BCrypt.Net.BCrypt.HashPassword("correct"), "abc@mail.ru");

            _userRepoMock
                .Setup(r => r.GetByUsernameAsync(dto.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.LoginAsync(dto));

            _tokenServiceMock.Verify(
                t => t.GenerateRefreshTokenAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ReturnsLoginResponse()
        {
            var dto = new LoginDTO("admin", "123");

            var user = new UserEntity(
                "admin",
                BCrypt.Net.BCrypt.HashPassword("123"), "abc@mail.ru");

            _userRepoMock
                .Setup(r => r.GetByUsernameAsync(dto.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _tokenServiceMock
                .Setup(t => t.GenerateRefreshTokenAsync(user, It.IsAny<CancellationToken>()))
                .ReturnsAsync("refresh");

            _tokenServiceMock
                .Setup(t => t.GenerateAccessJWT(user))
                .Returns("access");

            var result = await _sut.LoginAsync(dto);

            Assert.Equal("refresh", result.RefreshToken);
            Assert.Equal("access", result.AccessToken);
        }

        // ---------- LogoutAsync ----------

        [Fact]
        public async Task LogoutAsync_RevokesRefreshToken()
        {
            var token = "refresh";
            var userId = Guid.NewGuid();

            var entity = new RefreshTokenEntity(userId, "hash");

            _tokenServiceMock
                .Setup(t => t.GetTokenAsync(token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await _sut.LogoutAsync(token, userId);

            _tokenServiceMock.Verify(
                t => t.RevokeAsync(entity, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_Throws_WhenTokenNotFound()
        {
            var token = "refresh";

            _tokenServiceMock
                .Setup(t => t.GetTokenAsync(token, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshTokenEntity?)null!);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.LogoutAsync(token, Guid.NewGuid()));

            _tokenServiceMock.Verify(
                t => t.RevokeAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task LogoutAsync_Throws_WhenTokenBelongsToAnotherUser()
        {
            var token = "refresh";
            var entity = new RefreshTokenEntity(Guid.NewGuid(), "hash");

            _tokenServiceMock
                .Setup(t => t.GetTokenAsync(token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.LogoutAsync(token, Guid.NewGuid()));

            _tokenServiceMock.Verify(
                t => t.RevokeAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ---------- RefreshAsync ----------

        [Fact]
        public async Task RefreshAsync_DelegatesToTokenService()
        {
            var expected = new LoginResponseDTO("new-refresh", "new-access");

            _tokenServiceMock
                .Setup(t => t.RefreshAsync("raw", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _sut.RefreshAsync("raw");

            Assert.Equal("new-refresh", result.RefreshToken);
            Assert.Equal("new-access", result.AccessToken);

            _tokenServiceMock.Verify(
                t => t.RefreshAsync("raw", It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
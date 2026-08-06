using Microsoft.Extensions.Logging;
using Moq;
using TodoAPI.DTOs;
using TodoAPI.Entities;
using TodoAPI.Repo.UserRepository;
using TodoAPI.Services.AuthenticationService;

namespace TodoTests.Tests.User
{
    public class AuthServiceTests
    {
        public AuthServiceTests()
        {
            _sut = new AuthService(_userRepoMock.Object,_loggerMock.Object,
                _tokenServiceMock.Object);
        }

        private readonly Mock<IUserRepo> _userRepoMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<ILogger<AuthService>> _loggerMock = new();

        private readonly AuthService _sut;

        [Fact]
        public async Task LoginAsync_ThrowsArgumentException_WhenUserDoesntExist()
        {
            var dto = new LoginDTO
            {
                Username = "admin",
                Password = "123"
            };

            _userRepoMock
                .Setup(r => r.GetByUsernameAsync(dto.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.LoginAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_ThrowsArgumentException_WhenPasswordInvalid()
        {
            var dto = new LoginDTO
            {
                Username = "admin",
                Password = "wrong"
            };

            var user = new UserEntity(
                "admin",
                BCrypt.Net.BCrypt.HashPassword("correct"), "abc@mail.ru");

            _userRepoMock
                .Setup(r => r.GetByUsernameAsync(dto.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.LoginAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_ReturnsLoginResponse()
        {
            var dto = new LoginDTO
            {
                Username = "admin",
                Password = "123"
            };

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
    }
}

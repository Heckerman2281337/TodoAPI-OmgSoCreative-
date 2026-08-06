using Moq;
using TodoAPI.Repo.UserRepository;
using TodoAPI.Repo.TokenRepository;
using TodoAPI.Services;
using Microsoft.Extensions.Configuration;
using TodoAPI.Entities;

namespace TodoTests.Tokens
{
    public class TokenServiceTests
    {
        public TokenServiceTests()
        {
            _sut = new TokenService(_configurationMock.Object, _tokenRepoMock.Object,
                _userRepoMock.Object);
        }

        private readonly Mock<IConfiguration> _configurationMock = new();
        private readonly Mock<ITokenRepo> _tokenRepoMock = new();
        private readonly Mock<IUserRepo> _userRepoMock = new();

        private readonly TokenService _sut;

        [Fact]
        public async Task GenerateRefreshTokenAsync_SavesRefreshToken()
        {
            var user = new UserEntity(
                "admin",
                "hash",
                "abc@mail.ru");

            var token = await _sut.GenerateRefreshTokenAsync(user);

            Assert.False(string.IsNullOrWhiteSpace(token));

            _tokenRepoMock.Verify(
                r => r.CreateAsync(
                    It.IsAny<RefreshTokenEntity>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        [Fact]
        public async Task RefreshAsync_Throws_WhenTokenNotFound()
        {
            _tokenRepoMock
                .Setup(r => r.GetByTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshTokenEntity?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.RefreshAsync("token"));
        }
        [Fact]
        public async Task RefreshAsync_Throws_WhenRevoked()
        {
            var token = new RefreshTokenEntity(Guid.NewGuid(), "hash");

            token.RevokeToken();

            _tokenRepoMock
                .Setup(r => r.GetByTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(token);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.RefreshAsync("token"));
        }
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
                Times.Once);
        }
        [Fact]
        public async Task DeleteAsync_DeletesToken()
        {
            var entity = new RefreshTokenEntity(Guid.NewGuid(), "hash");

            _tokenRepoMock
                .Setup(r => r.GetByTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await _sut.DeleteAsync("token");

            _tokenRepoMock.Verify(
                r => r.DeleteAsync(
                    entity,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}

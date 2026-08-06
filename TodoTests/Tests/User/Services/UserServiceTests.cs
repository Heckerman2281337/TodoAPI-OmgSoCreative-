using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using TodoAPI.DTOs;
using TodoAPI.Entities;
using TodoAPI.Repo.UserRepository;
using TodoAPI.Services.UserServices;

namespace TodoTests.Tests.User
{
    public class UserServiceTests
    {
        public UserServiceTests()
        {
            _sut = new UserService(
                _userRepoMock.Object,
                _userValidatorMock.Object, _loggerMock.Object);
        }
        private readonly Mock<IUserRepo> _userRepoMock = new();
        private readonly Mock<IValidator<RegisterDTO>> _userValidatorMock = new();
        private readonly Mock<ILogger<UserService>> _loggerMock = new();
        private readonly UserService _sut;

        // CreateAsync tests
        [Fact]
        public async Task CreateAsync_CreatesUser_WhenDataValid()
        {
            var dto = new RegisterDTO
            {
                Username = "User123",
                Password = "Password123",
                ConfirmedPassword = "Password123",
                Email = "user@test.com"
            };
            UserEntity? createdUser = null;

            _userRepoMock
                .Setup(r => r.CreateAsync(
                    It.IsAny<UserEntity>(),
                    It.IsAny<CancellationToken>()))
                .Callback<UserEntity, CancellationToken>(
                    (user, _) => createdUser = user);

            _userValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<RegisterDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());


            await _sut.CreateAsync(dto, CancellationToken.None);

            Assert.NotNull(createdUser);
            Assert.Equal(dto.Username, createdUser.Username);
            Assert.Equal(dto.Email, createdUser.Email);
            Assert.True(
                BCrypt.Net.BCrypt.Verify(dto.Password, createdUser.HashedPassword));

            _userRepoMock.Verify(
                r => r.CreateAsync(
                    It.IsAny<UserEntity>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenValidationFails()
        {
            var dto = new RegisterDTO
            {
                Username = "User123",
                Password = "Password123",
                ConfirmedPassword = "Password123",
                Email = "user@test.com"
            };

            _userValidatorMock
                .Setup(v => v.ValidateAsync(
                    It.IsAny<RegisterDTO>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(
                [
                    new ValidationFailure("Username", "Username required")
                ]));

            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.CreateAsync(dto, CancellationToken.None));

            _userRepoMock.Verify(
                r => r.CreateAsync(
                    It.IsAny<UserEntity>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // DeleteAsync tests
        [Fact]
        public async Task DeleteAsync_DeletesUser_WhenUserExists()
        {
            var id = Guid.NewGuid();
            var entity = new UserEntity(
                "User123",
                "hashedPassword",
                "user@test.com");


            _userRepoMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);


            await _sut.DeleteAsync(id, CancellationToken.None);


            _userRepoMock.Verify(
                r => r.DeleteAsync(
                    entity,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Throws_WhenUserDoesntExist()
        {
            var id = Guid.NewGuid();

            _userRepoMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.DeleteAsync(id, CancellationToken.None));

            _userRepoMock.Verify(
                r => r.DeleteAsync(
                    It.IsAny<UserEntity>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // GetByIdAsync tests
        [Fact]
        public async Task GetByIdAsync_ReturnsUser_WhenUserExists()
        {
            var id = Guid.NewGuid();
            var entity = new UserEntity(
                "User123",
                "hashedPassword",
                "user@test.com");

            _userRepoMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await _sut.GetByIdAsync(
                id,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(entity.Username, result.Username);
            Assert.Equal(entity.Email, result.Email);
        }



        [Fact]
        public async Task GetByIdAsync_Throws_WhenUserDoesntExist()
        {
            var id = Guid.NewGuid();

            _userRepoMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.GetByIdAsync(
                    id,
                    CancellationToken.None));
        }

        // GetByUsernameAsync tests
        [Fact]
        public async Task GetByUsernameAsync_ReturnsUser_WhenExists()
        {
            var username = "User123";
            var entity = new UserEntity(
                username,
                "hashedPassword",
                "user@test.com");

            _userRepoMock
                .Setup(r => r.GetByUsernameAsync(
                    username,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await _sut.GetByUsernameAsync(
                username,
                CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
        }

        [Fact]
        public async Task GetByUsernameAsync_Throws_WhenUserDoesntExist()
        {
            var username = "Unknown";

            _userRepoMock
                .Setup(r => r.GetByUsernameAsync(
                    username,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.GetByUsernameAsync(
                    username,
                    CancellationToken.None));
        }

        // UpdateAsync tests
        [Fact]
        public async Task UpdateAsync_UpdatesUser_WhenUserExists()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateUserDTO
            {
                Username = "NewUsername",
                Password = "NewPassword123"
            };
            var entity = new UserEntity(
                "OldUsername",
                "OldHash",
                "user@test.com");

            _userRepoMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await _sut.UpdateAsync(
                dto,
                id,
                CancellationToken.None);

            _userRepoMock.Verify(
                r => r.UpdateAsync(
                    entity,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.Equal(
                dto.Username,
                entity.Username);
            Assert.True(
                BCrypt.Net.BCrypt.Verify(
                    dto.Password,
                    entity.HashedPassword));
        }

        [Fact]
        public async Task UpdateAsync_Throws_WhenUserDoesntExist()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateUserDTO
            {
                Username = "NewUsername",
                Password = "Password123"
            };

            _userRepoMock
                .Setup(r => r.GetByIdAsync(
                    id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserEntity?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateAsync(
                    dto,
                    id,
                    CancellationToken.None));


            _userRepoMock.Verify(
                r => r.UpdateAsync(
                    It.IsAny<UserEntity>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using TodoAPI.Application.DTOs;
using TodoAPI.Application.QueryParams;
using TodoAPI.Application.Repo;
using TodoAPI.Application.Services;
using TodoAPI.Domain.Entities;

namespace TodoTests.Tasks
{
    public class TaskServiceTests
    {
        public TaskServiceTests()
        {
            // По умолчанию кэш пустой — тесты, которым нужен кэш, переопределяют Setup
            _cacheMock
                .Setup(c => c.ReadFromCacheAsync<TaskResponseDTO>(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskResponseDTO?)null);

            _sut = new TaskService(
                _taskRepoMock.Object,
                _taskValidatorMock.Object,
                _updateValidatorMock.Object,
                _cacheMock.Object,
                _loggerMock.Object);
        }

        private readonly Mock<ITaskRepo> _taskRepoMock = new();
        private readonly Mock<IValidator<TaskDTO>> _taskValidatorMock = new();
        private readonly Mock<IValidator<UpdateTaskDTO>> _updateValidatorMock = new();
        private readonly Mock<ICacheService> _cacheMock = new();
        private readonly Mock<ILogger<TaskService>> _loggerMock = new();
        private readonly TaskService _sut;

        // ---------- helpers ----------

        private static string CacheKey(Guid taskId) => $"tasks:{taskId}";

        private void SetupValidUpdate() =>
            _updateValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<UpdateTaskDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

        private void VerifyCacheWritten(Guid taskId, Times times) =>
            _cacheMock.Verify(
                c => c.WriteInCacheAsync<TaskResponseDTO>(
                    CacheKey(taskId),
                    It.IsAny<TaskResponseDTO>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<CacheDuration>(), It.IsAny<CacheSlidingDuration>()),
                times);

        private void VerifyCacheDeleted(Guid taskId, Times times) =>
            _cacheMock.Verify(
                c => c.DeleteFromCacheAsync(
                    CacheKey(taskId),
                    It.IsAny<CancellationToken>()),
                times);

        private void VerifyCacheUntouched()
        {
            _cacheMock.Verify(
                c => c.WriteInCacheAsync<TaskResponseDTO>(
                    It.IsAny<string>(),
                    It.IsAny<TaskResponseDTO>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<CacheDuration>(), It.IsAny<CacheSlidingDuration>()),
                Times.Never());
            _cacheMock.Verify(
                c => c.DeleteFromCacheAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never());
        }

        // ---------- GetByIdAsync ----------

        [Fact]
        public async Task GetByIdAsync_Throws_KeyNotFoundEx_WhenTaskDoesntExist()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskEntity?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetByIdAsync(taskId, userId));

            VerifyCacheWritten(taskId, Times.Never());
        }

        [Fact]
        public async Task GetByIdAsync_Throws_KeyNotFoundEx_WhenNotOwnerTryingToGetTask()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new TaskEntity("Title", "Desc", Guid.NewGuid(), null, null, null);

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetByIdAsync(taskId, userId));

            VerifyCacheWritten(taskId, Times.Never());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsTaskResponseDTO_WhenTaskExistsAndOwner()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new TaskEntity("Title", "Desc", userId, null, null, null);
            var expected = new TaskResponseDTO(entity);

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await _sut.GetByIdAsync(taskId, userId);

            Assert.NotNull(result);
            Assert.Equal(expected.Title, result.Title);
            Assert.Equal(expected.Description, result.Description);
            Assert.Equal(expected.Category, result.Category);
            Assert.Equal(expected.Priority, result.Priority);
        }

        [Fact]
        public async Task GetByIdAsync_WritesToCache_WhenCacheMiss()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new TaskEntity("Title", "Desc", userId, null, null, null);

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await _sut.GetByIdAsync(taskId, userId);

            _cacheMock.Verify(
                c => c.WriteInCacheAsync<TaskResponseDTO>(
                    CacheKey(taskId),
                    It.Is<TaskResponseDTO>(d => d.Title == "Title" && d.Description == "Desc"),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<CacheDuration>(), It.IsAny<CacheSlidingDuration>()),
                Times.Once());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsFromCache_WithoutHittingRepo_WhenCacheHitAndOwner()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var cached = new TaskResponseDTO(new TaskEntity("Cached", "Desc", userId, null, null, null));

            _cacheMock
                .Setup(c => c.ReadFromCacheAsync<TaskResponseDTO>(
                    CacheKey(taskId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cached);

            var result = await _sut.GetByIdAsync(taskId, userId);

            Assert.Equal("Cached", result.Title);

            _taskRepoMock.Verify(
                r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never());
            VerifyCacheWritten(taskId, Times.Never());
        }

        [Fact]
        public async Task GetByIdAsync_Throws_KeyNotFoundEx_WhenCachedTaskBelongsToAnotherUser()
        {
            var taskId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var cached = new TaskResponseDTO(new TaskEntity("Cached", "Desc", ownerId, null, null, null));

            _cacheMock
                .Setup(c => c.ReadFromCacheAsync<TaskResponseDTO>(
                    CacheKey(taskId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cached);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetByIdAsync(taskId, anotherUserId));

            _taskRepoMock.Verify(
                r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never());
        }

        // ---------- DeleteAsync ----------

        [Fact]
        public async Task DeleteAsync_DeletesTask_WhenTaskExistsAndOwner()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new TaskEntity("Title", "Desc", userId, null, null, null);

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await _sut.DeleteAsync(taskId, userId);

            _taskRepoMock.Verify(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_RemovesTaskFromCache_WhenDeleted()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new TaskEntity("Title", "Desc", userId, null, null, null);

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await _sut.DeleteAsync(taskId, userId);

            VerifyCacheDeleted(taskId, Times.Once());
        }

        [Fact]
        public async Task DeleteAsync_ThrowsKeyNotFound_WhenTaskDoesntExist()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskEntity?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteAsync(taskId, userId));

            _taskRepoMock.Verify(r => r.DeleteAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Never());
            VerifyCacheUntouched();
        }

        [Fact]
        public async Task DeleteAsync_ThrowsKeyNotFound_WhenNotOwner()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new TaskEntity("Title", "Desc", Guid.NewGuid(), null, null, null);

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteAsync(taskId, userId));

            _taskRepoMock.Verify(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>()), Times.Never());
            VerifyCacheUntouched();
        }

        // ---------- UpdateAsync ----------

        [Fact]
        public async Task UpdateAsync_UpdateTask_WhenTaskExistsAndOwner()
        {
            var dto = new UpdateTaskDTO("New Title", "New Desc",
                null, null, null, null);
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var entity = new TaskEntity("Old Title", "Old Desc", userId, null, null, null);

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);
            SetupValidUpdate();

            var result = await _sut.UpdateAsync(dto, taskId, userId);

            Assert.NotNull(result);
            Assert.Equal("New Title", result.Title);
            Assert.Equal("New Desc", result.Description);

            _taskRepoMock.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_RefreshesCache_WhenUpdated()
        {
            var dto = new UpdateTaskDTO("New Title", "New Desc",
                null, null, null, null);
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var entity = new TaskEntity("Old Title", "Old Desc", userId, null, null, null);

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);
            SetupValidUpdate();

            await _sut.UpdateAsync(dto, taskId, userId);

            _cacheMock.Verify(
                c => c.WriteInCacheAsync<TaskResponseDTO>(
                    CacheKey(taskId),
                    It.Is<TaskResponseDTO>(d => d.Title == "New Title" && d.Description == "New Desc"),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<CacheDuration>(), It.IsAny<CacheSlidingDuration>()),
                Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_ThrowsKeyNotFound_WhenTaskDoesntExist()
        {
            var dto = new UpdateTaskDTO("Title", "Desc",
                null, null, null, null);
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskEntity?)null);
            SetupValidUpdate();

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateAsync(dto, taskId, userId));

            _taskRepoMock.Verify(r => r.UpdateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Never());
            VerifyCacheUntouched();
        }

        [Fact]
        public async Task UpdateAsync_ThrowsKeyNotFound_WhenNotOwner()
        {
            var dto = new UpdateTaskDTO("Title", "Desc",
                null, null, null, null);
            var taskId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var entity = new TaskEntity("Title", "Desc", Guid.NewGuid(), null, null, null);

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);
            SetupValidUpdate();

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateAsync(dto, taskId, anotherUserId));

            _taskRepoMock.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Never());
            VerifyCacheUntouched();
        }

        [Fact]
        public async Task UpdateAsync_ThrowsValidationException_WhenValidationFails()
        {
            var dto = new UpdateTaskDTO("Title", "Desc",
                null, null, null, null);
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _updateValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<UpdateTaskDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Title", "Error") }));

            await Assert.ThrowsAsync<ValidationException>(() => _sut.UpdateAsync(dto, taskId, userId));

            _taskRepoMock.Verify(
                r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never());
            _updateValidatorMock.Verify(
                v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()),
                Times.Once());
            VerifyCacheUntouched();
        }

        // ---------- CreateAsync ----------

        [Fact]
        public async Task CreateAsync_CreateTask()
        {
            var dto = new TaskDTO("Title", null, null, null, null);
            var userId = Guid.NewGuid();

            _taskValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<TaskDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            await _sut.CreateAsync(dto, userId);

            _taskRepoMock.Verify(
                r => r.CreateAsync(
                    It.Is<TaskEntity>(t =>
                        t.Title == dto.Title &&
                        t.Description == dto.Description &&
                        t.UserId == userId &&
                        t.Category == dto.Category &&
                        t.Priority == dto.Priority),
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public async Task CreateAsync_ThrowsValidationException_WhenValidationFails()
        {
            var dto = new TaskDTO("", null, null, null, null);

            _taskValidatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<TaskDTO>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Title", "Title required") }));

            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.CreateAsync(dto, Guid.NewGuid()));

            _taskRepoMock.Verify(
                r => r.CreateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()),
                Times.Never());
        }

        // ---------- GetAllAsync ----------

        [Fact]
        public async Task GetAllAsync_ReturnsTaskResponses()
        {
            var userId = Guid.NewGuid();
            var filter = new TaskFilterParams();
            var sort = new TaskSortParams();
            var pagination = new TaskPaginationParams();

            var tasks = new[]
            {
                new TaskEntity("Task 1", "Desc 1", userId, null, null, null),
                new TaskEntity("Task 2", "Desc 2", userId, null, null, null)
            };

            var pagedResult = new PagedResult<TaskEntity>(tasks, 2);

            _taskRepoMock
                .Setup(r => r.GetAllAsync(
                    userId,
                    filter,
                    sort,
                    pagination,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            var result = await _sut.GetAllAsync(userId, filter, sort, pagination);

            Assert.NotNull(result);
            Assert.Equal(2, result.Data.Count());
            Assert.Equal("Task 1", result.Data.First().Title);
            Assert.Equal("Task 2", result.Data.Last().Title);
            Assert.Equal(2, result.TotalCount);

            _taskRepoMock.Verify(
                r => r.GetAllAsync(
                    userId,
                    filter,
                    sort,
                    pagination,
                    It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}
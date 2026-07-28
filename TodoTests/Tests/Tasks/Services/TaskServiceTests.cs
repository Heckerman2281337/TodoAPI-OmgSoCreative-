using Moq;
using TodoAPI;
using TodoAPI.DTOs;
using TodoAPI.Entities;
using TodoAPI.QueryParams;
using TodoAPI.Repo.TaskRepository;
using TodoAPI.Services.TaskServices;
using TodoAPI.Validators;

namespace TodoTests.Tasks
{
    public class TaskServiceTests
    {
        public TaskServiceTests()
        {
            _sut = new TaskService(_taskRepoMock.Object, _taskValidatorMock.Object,
                _updateValidatorMock.Object);
        }

        private readonly Mock<ITaskRepo> _taskRepoMock = new();
        private readonly Mock<IValidator<TaskDTO>> _taskValidatorMock = new();
        private readonly Mock<IValidator<UpdateTaskDTO>> _updateValidatorMock = new();
        private readonly TaskService _sut;

        //GetByIdAsync tests
        [Fact]
        public async Task GetByIdAsync_Throws_KeyNotFoundEx_WhenTaskDoesntExist()
        {
            //arrange
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskEntity?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetByIdAsync(taskId, userId));
        }

        [Fact]
        public async Task GetByIdAsync_Throws_KeyNotFoundEx_WhenNotOwnerTryingToGetTask()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new TaskEntity("Title","Desc", Guid.NewGuid(), null, TaskCategory.None, TaskPriority.None);
            
            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetByIdAsync(taskId,userId));
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsTaskResponseDTO_WhenTaskExistsAndOwner()
        {
            var taskId = Guid.NewGuid();    
            var userId = Guid.NewGuid();
            var entity = new TaskEntity("Title", "Desc", userId, null, TaskCategory.None, TaskPriority.None);
            var expected = new TaskResponseDTO(entity);

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await _sut.GetByIdAsync(taskId, userId);
            

            Assert.NotNull(result);
            Assert.Equal(expected.Id, result.Id);
            Assert.Equal(expected.Title, result.Title);
            Assert.Equal(expected.Description, result.Description);
            Assert.Equal(expected.Category, result.Category);
            Assert.Equal(expected.Priority, result.Priority);
        }

        //DeleteAsync tests
        [Fact]
        public async Task DeleteAsync_DeletesTask_WhenTaskExistsAndOwner()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new TaskEntity("Title", "Desc", userId, null, TaskCategory.None, TaskPriority.None);

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await _sut.DeleteAsync(taskId, userId);

            _taskRepoMock.Verify(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
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

            _taskRepoMock.Verify(r => r.DeleteAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsKeyNotFound_WhenNotOwner()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var entity = new TaskEntity("Title", "Desc", Guid.NewGuid(), null, TaskCategory.None, TaskPriority.None);

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteAsync(taskId, userId));

            _taskRepoMock.Verify(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>()), Times.Never);
        }

        //UpdateAsync tests
        [Fact]
        public async Task UpdateAsync_UpdateTask_WhenTaskExistsAndOwner()
        {
            var dto = new UpdateTaskDTO
            {
                Title = "New Title",
                Description = "New Desc",
            };
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var entity = new TaskEntity("Old Title", "Old Desc", userId, null, TaskCategory.None, TaskPriority.None); 
            
            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await _sut.UpdateAsync(dto, taskId, userId);

            Assert.NotNull(result);
            Assert.Equal("New Title", result.Title);
            Assert.Equal("New Desc", result.Description);

            _taskRepoMock.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsKeyNotFound_WhenTaskDoesntExist()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskEntity?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateAsync(It.IsAny<UpdateTaskDTO>(),taskId, userId));

            _taskRepoMock.Verify(r => r.UpdateAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsKeyNotFound_WhenNotOwner()
        {
            var dto = new UpdateTaskDTO
            {
                Title = "Title",
                Description = "Desc",
            };
            var taskId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var entity = new TaskEntity("Title", "Desc", Guid.NewGuid(), null, TaskCategory.None, TaskPriority.None);

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateAsync(dto, taskId, anotherUserId));

            _taskRepoMock.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ThrowArgumentEx_WhenValidationFails()
        {
            var dto = new UpdateTaskDTO
            {
                Title = "Title",
                Description= "Desc",
            };
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _updateValidatorMock
                .Setup(r => r.Validate(It.IsAny<UpdateTaskDTO>()))
                .Throws<ArgumentException>();

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateAsync(dto, taskId, userId));

            _taskRepoMock.Verify(
                r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _updateValidatorMock.Verify(
                v => v.Validate(dto),
                Times.Once);
        }
        //CreateAsync tests
        [Fact]
        public async Task CreateAsync_CreateTask()
        {
            var dto = new TaskDTO("Title", null, null, TaskCategory.None, TaskPriority.None);
            var userId = Guid.NewGuid();

            await _sut.CreateAsync(dto, userId);

            _taskValidatorMock.Verify(
                v => v.Validate(dto),
                Times.Once);
            _taskRepoMock.Verify(
                r => r.CreateAsync(
                    It.Is<TaskEntity>(t =>
                        t.Title == dto.Title &&
                        t.Description == string.Empty &&
                        t.UserId == userId &&
                        t.Category == dto.Category &&
                        t.Priority == dto.Priority),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        //GetAllAsync tests
        [Fact]
        public async Task GetAllAsync_ReturnsTaskResponses()
        {
            var userId = Guid.NewGuid();
            var filter = new TaskFilterParams();
            var sort = new TaskSortParams();
            var pagination = new TaskPaginationParams();

            var tasks = new[]
            {
            new TaskEntity(
                "Task 1",
                "Desc 1",
                userId,
                null,
                TaskCategory.None,
                TaskPriority.None),

            new TaskEntity(
                "Task 2",
                "Desc 2",
                userId,
                null,
                TaskCategory.None,
                TaskPriority.None)
            };

            var pagedResult = new PagedResult<TaskEntity>(
                tasks,
                2);

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
                Times.Once);
        }
    }
}

using Xunit;
using TodoAPI.Entities;

namespace TodoTests.Tasks
{
    public class TaskEntityTests
    {
        //Constructor tests
        [Fact]
        public void Constructor_SetsAllPropertiesCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var deadline = DateTime.UtcNow.AddDays(3);

            // Act
            var task = new TaskEntity(
                "Buy groceries",
                "Milk, eggs, bread",
                userId,
                deadline,
                TaskCategory.Chores,
                TaskPriority.Medium);

            // Assert
            Assert.Equal("Buy groceries", task.Title);
            Assert.Equal("Milk, eggs, bread", task.Description);
            Assert.Equal(userId, task.UserId);
            Assert.Equal(deadline, task.Deadline);
            Assert.Equal(TaskCategory.Chores, task.Category);
            Assert.Equal(TaskPriority.Medium, task.Priority);
            Assert.False(task.IsCompleted);
            Assert.NotEqual(Guid.Empty, task.Id);
        }

        [Fact]
        public void Constructor_SetsCreatedToUtcNow()
        {
            // Arrange
            var before = DateTime.UtcNow;

            // Act
            var task = new TaskEntity("Title", "Desc", Guid.NewGuid(),
                null, TaskCategory.Work, TaskPriority.Low);

            var after = DateTime.UtcNow;

            // Assert 
            Assert.InRange(task.Created, before, after);
        }

        // Exparation tests

        [Fact]
        public void Exparation_ReturnsExpired_WhenDeadlineIsInPast()
        {
            var task = new TaskEntity("Title", "Desc", Guid.NewGuid(),
                DateTime.UtcNow.AddDays(-1), TaskCategory.Work, TaskPriority.Medium);

            Assert.Equal(TaskExparation.Expired, task.Exparation);
        }

        [Fact]
        public void Exparation_ReturnsNotExpired_WhenDeadlineIsInFuture()
        {
            var task = new TaskEntity("Title", "Desc", Guid.NewGuid(),
                DateTime.UtcNow.AddDays(1), TaskCategory.Work, TaskPriority.Medium);

            Assert.Equal(TaskExparation.NotExpired, task.Exparation);
        }

        [Fact]
        public void Exparation_ReturnsNotExpired_WhenDeadlineIsNull()
        {
            var task = new TaskEntity("Title", "Desc", Guid.NewGuid(),
                null, TaskCategory.Work, TaskPriority.Medium);

            Assert.Equal(TaskExparation.NotExpired, task.Exparation);
        }

        // Update tests

        [Fact]
        public void Update_ChangesTitleDescriptionCategoryPriority()
        {
            // Arrange
            var task = new TaskEntity("Old title", "Old desc", Guid.NewGuid(),
                DateTime.UtcNow.AddDays(1), TaskCategory.Work, TaskPriority.Low);

            // Act
            task.Update("New title", "New desc", true,
                TaskCategory.Personal, TaskPriority.High, DateTime.UtcNow.AddDays(5));

            // Assert
            Assert.Equal("New title", task.Title);
            Assert.Equal("New desc", task.Description);
            Assert.Equal(TaskCategory.Personal, task.Category);
            Assert.Equal(TaskPriority.High, task.Priority);
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public void Update_SetsUpdatedToUtcNow()
        {
            var task = new TaskEntity("Title", "Desc", Guid.NewGuid(),
                null, TaskCategory.Work, TaskPriority.Low);

            Assert.Null(task.Updated);

            var before = DateTime.UtcNow;
            task.Update("New title", "Desc", false, TaskCategory.Work, TaskPriority.Low, null);
            var after = DateTime.UtcNow;

            Assert.NotNull(task.Updated);
            Assert.InRange(task.Updated!.Value, before, after);
        }
        [Fact]
        public void Exparation_IsNotExpired_WhenDeadlineIsInFuture_BeforeUpdate()
        {
            var task = new TaskEntity("Title", "Desc", Guid.NewGuid(),
                DateTime.UtcNow.AddDays(5), TaskCategory.Work, TaskPriority.Low);

            Assert.Equal(TaskExparation.NotExpired, task.Exparation);
        }

        [Fact]
        public void Update_RecalculatesExparation_WhenDeadlineChangesToPast()
        {
            //arranger
            var task = new TaskEntity("Title", "Desc", Guid.NewGuid(), 
                DateTime.UtcNow.AddDays(5), TaskCategory.Work, TaskPriority.Low);

            //act
            task.Update("Title", "Desc", false, TaskCategory.Work, TaskPriority.Low,
                DateTime.UtcNow.AddDays(-1));
            //assert
            Assert.Equal(TaskExparation.Expired, task.Exparation);
        }
    }
}
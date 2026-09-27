using TodoAPI.Domain.Enums;
using TodoAPI.Domain.Entities;

namespace TodoAPI.Application.DTOs
{
    public record TaskResponseDTO(
        string Title, string? Description,
        DateTime CreatedAt, DateTime? Deadline,
        DateTime? Updated, bool? IsCompleted,
        TaskCategory? Category, TaskPriority? Priority,
        TaskExparation? Exparation)
    {
        public TaskResponseDTO(TaskEntity taskEntity) : this(
            taskEntity.Title, taskEntity.Description, taskEntity.Created,
            taskEntity.Deadline, taskEntity.Updated, taskEntity.IsCompleted,
            taskEntity.Category, taskEntity.Priority, taskEntity.Exparation)
        { }
    }
}

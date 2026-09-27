using TodoAPI.Domain.Enums;

namespace TodoAPI.Application.DTOs
{
    public record UpdateTaskDTO(
        string Title, string? Description, DateTime? Deadline,
        TaskCategory? Category, TaskPriority? Priority, bool? IsCompleted);
}

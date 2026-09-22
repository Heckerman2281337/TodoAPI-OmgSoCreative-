using TodoAPI.Entities;

namespace TodoAPI.DTOs
{
    public record UpdateTaskDTO(
        string Title, string? Description, DateTime? Deadline,
        TaskCategory? Category, TaskPriority? Priority, bool? IsCompleted);
}

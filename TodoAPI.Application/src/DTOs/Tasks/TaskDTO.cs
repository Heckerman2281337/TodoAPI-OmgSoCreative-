using TodoAPI.Domain.Enums;

namespace TodoAPI.Application.DTOs
{
    public record TaskDTO(
        string Title, string? Description,
        DateTime? Deadline, TaskCategory? Category,
        TaskPriority? Priority);
}

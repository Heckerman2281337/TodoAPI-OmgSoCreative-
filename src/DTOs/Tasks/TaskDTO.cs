using TodoAPI.Entities;

namespace TodoAPI.DTOs
{
    public record TaskDTO(
        string Title, string? Description,
        DateTime? Deadline, TaskCategory Category,
        TaskPriority Priority);
}

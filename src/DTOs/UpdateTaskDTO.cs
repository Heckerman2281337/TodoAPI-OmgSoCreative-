using TodoAPI.Entities;

namespace TodoAPI.DTOs
{
    public class UpdateTaskDTO
    {
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? Deadline { get; init; }

        public TaskCategory Category { get; init; }
        public TaskPriority Priority { get; init; }
        
        public bool? IsCompleted { get; init; }
    }
}

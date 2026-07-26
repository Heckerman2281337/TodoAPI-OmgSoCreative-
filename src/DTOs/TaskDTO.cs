using TodoAPI.Entities;

namespace TodoAPI.DTOs
{
    public class TaskDTO
    {
        public TaskDTO(string title, string? description, 
            string deadline, TaskCategory category, TaskPriority priority) 
        { 
            Title = title;
            Description = description;
            Deadline = deadline;
            Category = category;
            Priority = priority;
        }

        public string Title { get; init; }
        public string? Description { get; init; }
        public string? Deadline { get; init; }

        public TaskCategory Category { get; init; }
        public TaskPriority Priority { get; init; }
    }
}

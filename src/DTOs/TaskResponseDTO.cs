using TodoAPI.Entities;

namespace TodoAPI.DTOs
{
    public class TaskResponseDTO
    {
        public TaskResponseDTO(TaskEntity task)
        {
            Title = task.Title;
            Description = task.Description;
            Created = task.Created;
            Deadline = task.Deadline;
            Updated = task.Updated;
            IsCompleted = task.IsCompleted;
            Category = task.Category;
            Priority = task.Priority;
            Exparation = task.Exparation;
        }

        public string Title { get; private set; }
        public string? Description { get; private set; }

        public DateTime Created { get; private set; }
        public DateTime? Deadline { get; private set; }
        public DateTime? Updated { get; private set; }

        public bool? IsCompleted { get; private set; }

        public TaskCategory Category { get; private set; }
        public TaskPriority Priority { get; private set; }
        public TaskExparation Exparation { get; private set; }
    }
}

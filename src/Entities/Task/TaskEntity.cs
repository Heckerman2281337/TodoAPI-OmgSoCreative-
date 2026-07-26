namespace TodoAPI.Entities
{
    public class TaskEntity
    {
        private TaskEntity(){ Title = null!; }

        public TaskEntity(string title, string description, Guid userId, DateTime? deadline, 
            TaskCategory category, TaskPriority priority) 
        { 
            Title = title;
            Description = description;
            Id = Guid.NewGuid();

            Created = DateTime.UtcNow;
            Deadline = deadline;
            IsCompleted = false;

            Category = category;
            Priority = priority;
            
            UserId = userId;
        }
            
        public string Title { get; private set; }
        public string? Description { get; private set; }
        public Guid Id { get; private set; }
        
        public DateTime Created { get; private set; }
        public DateTime? Deadline { get; private set; }
        public DateTime? Updated { get; private set; }
       
        public bool? IsCompleted { get; private set; }
        
        public TaskCategory Category { get; private set; }
        public TaskPriority Priority { get; private set; }
        public TaskExparation Exparation =>
                Deadline.HasValue && Deadline.Value <= DateTime.UtcNow
                    ? TaskExparation.Expired
                    : TaskExparation.NotExpired;

        public Guid UserId { get; private set; }
        public UserEntity? User { get; private set; }


        public void Update(string title, string? description, bool? isCompleted,
            TaskCategory category, TaskPriority priority, DateTime? deadline)
        {
            Title = title;
            Description = description;
            Updated = DateTime.UtcNow;
            Deadline = deadline;
            IsCompleted = isCompleted;
            Category = category;
            Priority = priority;
        }
    }
}

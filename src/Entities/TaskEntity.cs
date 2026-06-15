namespace TodoAPI.src.Entities
{
    public class TaskEntity
    {
        private TaskEntity(){ Title = null!; }

        public TaskEntity(string title, string description, Guid userId) 
        { 
            Title = title;
            Description = description;
            Id = Guid.NewGuid();
            UserId = userId;
            Created = DateTime.UtcNow;
            IsCompleted = false;
        }
            
        public string Title { get; private set; }
        public string? Description { get; private set; }
        public Guid Id { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime? Updated { get; private set; }
        public bool? IsCompleted { get; private set; }
        public Guid UserId { get; private set; }
        public UserEntity User { get; private set; }


        public void Update(string title, string? description)
        {
            Title = title;
            Description = description;
            Updated = DateTime.UtcNow;
        }
    }
}

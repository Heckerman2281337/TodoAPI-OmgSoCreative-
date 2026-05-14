namespace TodoAPI.src.Entities
{
    public class TaskEntity
    {
        public TaskEntity(string title, string description) 
        { 
            Title = title;
            Description = description;
            Id = Guid.NewGuid();
            Created = DateTime.Now;
            IsCompleted = false;
        }
            
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Guid Id { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime? Updated { get; private set; }
        public bool IsCompleted { get; private set; }
    }
}

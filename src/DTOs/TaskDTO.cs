namespace TodoAPI.src.DTOs
{
    public class TaskDTO
    {
        public TaskDTO(string title, string? description) 
        { 
            this.Title = title;
            this.Description = description;
        }
        public string Title { get; init; }
        public string? Description { get; init; }
    }
}

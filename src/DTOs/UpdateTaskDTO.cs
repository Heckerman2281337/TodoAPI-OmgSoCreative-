namespace TodoAPI.src.DTOs
{
    public class UpdateTaskDTO
    {
        public UpdateTaskDTO(string title, string? description, bool isComplete)
        {
            Title = title;
            Description = description;
            IsComplete = isComplete;
        }
        public string Title { get; init; }
        public string? Description { get; init; }
        public bool? IsComplete { get; init; }
    }
}

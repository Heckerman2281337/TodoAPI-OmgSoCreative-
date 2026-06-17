namespace TodoAPI.src.DTOs
{
    public class UpdateTaskDTO
    {
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public bool? IsComplete { get; init; }
    }
}

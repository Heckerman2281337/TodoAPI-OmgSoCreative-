namespace TodoAPI.Application.QueryParams
{
    public class TaskFilterParams
    {
        public string? Title { get; set; }
        public bool? IsCompleted { get; set; }
    }
}

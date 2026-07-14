namespace TodoAPI.QueryParams
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public class TaskSortParams
    {
        public string? OrderBy { get; set; }
        public SortDirection? SortDirection { get; set; }
    }
}

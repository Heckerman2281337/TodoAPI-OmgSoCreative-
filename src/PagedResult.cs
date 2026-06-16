//Class for frontend
namespace TodoAPI.src
{
    public class PagedResult<T>
    {
        public PagedResult(T[] data, int totalCount) 
        { 
            Data = data;
            TotalCount = totalCount;
        }

        public T[] Data { get; }
        public int TotalCount { get; }
    }
}

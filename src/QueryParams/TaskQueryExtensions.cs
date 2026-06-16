using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoAPI.src.Entities;
using TodoAPI.src.QueryParams;
namespace TodoAPI.src.QuerryParams
{
    public static class TaskQueryExtensions
    {
        public static IQueryable<TaskEntity> Filter(this IQueryable<TaskEntity> query, TaskFilterParams taskFilter)
        {
            if(!string.IsNullOrWhiteSpace(taskFilter.Title))
                query = query.Where(x => x.Title == taskFilter.Title);
            if(taskFilter.IsCompleted.HasValue)
                query = query.Where(x => x.IsCompleted == taskFilter.IsCompleted.Value);

            return query;
        }

        public static IQueryable<TaskEntity> Sort(this IQueryable<TaskEntity> query, TaskSortParams taskSort)
        {
            return taskSort.SortDirection == SortDirection.Descending
                ? query.OrderByDescending(GetKeySelector(taskSort.OrderBy))
                : query.OrderBy(GetKeySelector(taskSort.OrderBy));
        }

        public static async Task<PagedResult<TaskEntity>> ToPagedAsync
            (this IQueryable<TaskEntity> query,
            TaskPaginationParams taskPagination)
        {
            var count = await query.CountAsync();
            if (count == 0)
                return new PagedResult<TaskEntity>([], 0);

            var page = taskPagination.PageNumber ?? 1;
            var pageSize = taskPagination.PageSize ?? 10;

            var skip = (page - 1) * pageSize;
            var result = await query.Skip(skip).Take(pageSize).ToArrayAsync();

            return new PagedResult<TaskEntity>(result, count);
        }

        private static Expression<Func<TaskEntity, object>> GetKeySelector(string? orderBy)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return x => x.Title;

            return orderBy switch
            {
                nameof(TaskEntity.IsCompleted) => x => x.IsCompleted,
                nameof(TaskEntity.Updated) => x => x.Updated,
                nameof(TaskEntity.Created) => x => x.Created,
                _ => x => x.Title
            };
        }
    }
}

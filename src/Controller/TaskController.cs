using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoAPI.DTOs;
using TodoAPI.QueryParams;
using TodoAPI.Services.TaskServices;

namespace TodoAPI.Controller
{
    [Authorize]
    [ApiController]
    [Route(template:"Task")]
    public class TaskController(ITaskService taskService) : ControllerBase
    {
        private Guid GetUserId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpPost]
        public async Task<IActionResult> CreateTaskAsync([FromBody] TaskDTO dto)
        {
            await taskService.CreateAsync(dto, GetUserId());
            return StatusCode(201);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync
            ([FromQuery] TaskFilterParams taskFilter,[FromQuery] TaskSortParams taskSort
            ,[FromQuery] TaskPaginationParams taskPagination)
        {
            var tasks = await taskService.GetAllAsync(GetUserId(), taskFilter, 
                taskSort, taskPagination);
            return Ok(tasks);
        }

        [HttpGet(template:"{id:guid}")]
        public async Task<IActionResult> GetTaskAsync([FromRoute] Guid id)
        {
            var result = await taskService.GetByIdAsync(id, GetUserId());
            return Ok(result);
        }

        [HttpDelete(template:("{id:guid}"))]
        public async Task<IActionResult> DeleteTaskAsync([FromRoute] Guid id)
        {
            await taskService.DeleteAsync(id, GetUserId());
            return NoContent();
        }

        [HttpPatch(template:("{id:guid}"))]
        public async Task<IActionResult> UpdateTaskAsync([FromBody]UpdateTaskDTO dto,
            [FromRoute] Guid id)
        {
            var updatedTask = await taskService.UpdateAsync(dto, id, GetUserId());
            return Ok(updatedTask);
        }
    }
}
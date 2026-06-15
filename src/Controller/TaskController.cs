using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoAPI.src.DTOs;
using TodoAPI.src.Services.TaskServices;
namespace TodoAPI.src.Controller
{
    [Authorize]
    [ApiController]
    [Route(template:"Task")]
    public class TaskController(ITaskService taskService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateTaskAsync([FromBody] TaskDTO dto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdString == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdString);
            await taskService.CreateAsync(dto, userId);
            return StatusCode(201);
        }

        [HttpGet(template:"{id:guid}")]
        public async Task<IActionResult> GetTaskAsync([FromRoute] Guid id)
        {
            var result = await taskService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpDelete(template:("{id:guid}"))]
        public async Task<IActionResult> DeleteTaskAsync([FromRoute] Guid id)
        {
            await taskService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPatch(template:("{id:guid}"))]
        public async Task<IActionResult> UpdateTaskAsync([FromBody] TaskDTO dto, [FromRoute] Guid id)
        {
            var updatedTask = await taskService.UpdateAsync(dto, id);
            return Ok(updatedTask);
        }
    }
}
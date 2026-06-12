using Microsoft.AspNetCore.Mvc;
using TodoAPI.src.DTOs;
using TodoAPI.src.Services.TaskServices;
namespace TodoAPI.src.Controller
{
    [ApiController]
    [Route(template:"Task")]
    public class TaskController(IUserService taskService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateTaskAsync([FromBody] TaskDTO dto)
        {
            await taskService.CreateAsync(dto);
            return Ok();
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

        [HttpPost(template:("{id:guid}"))]
        public async Task<IActionResult> UpdateTaskAsync([FromBody] TaskDTO dto, [FromRoute] Guid id)
        {
            await taskService.UpdateAsync(dto, id);
            return Ok();
        }
    }
}
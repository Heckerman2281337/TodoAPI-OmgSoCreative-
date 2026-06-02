using Microsoft.AspNetCore.Mvc;
using TodoAPI.src.Services;
using TodoAPI.src.DTOs;
namespace TodoAPI.src.Controller
{
    [ApiController]
    [Route(template:"Task")]
    public class TaskController(ITaskService taskService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] TaskDTO dto)
        {
            await taskService.CreateAsync(dto);
            return Ok();
        }

    //[HttpPost]
    }
}
using Microsoft.AspNetCore.Mvc;

namespace TodoAPI.src.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok("Работает!");
        }
    }
}
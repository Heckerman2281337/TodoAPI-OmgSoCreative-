using Microsoft.AspNetCore.Mvc;
using TodoAPI.src.DTOs;
using TodoAPI.src.Services.AuthService;
using TodoAPI.src.Services.UserServices;

namespace TodoAPI.src.Controller
{
    [ApiController]
    [Route(template:"User")]
    public class AuthController(IUserService userService, IAuthService authService) : ControllerBase
    {
        [HttpPost(template:"register")]
        public async Task<IActionResult> UserCreate([FromBody] RegisterDTO dto)
        {
            await userService.CreateAsync(dto);
            return CreatedAtAction(nameof(UserCreate), null);
        }

        [HttpPost(template:"login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO dto)
        {
            var token = await authService.LoginAsync(dto);
            return Ok(new {token});
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using TodoAPI.DTOs;
using TodoAPI.Services.AuthenticationService;
using TodoAPI.Services.UserServices;

namespace TodoAPI.Controller
{
    [ApiController]
    [Route(template:"User")]
    public class AuthController(IUserService userService, IAuthService authService) : ControllerBase
    {
        [HttpPost(template:"register")]
        public async Task<IActionResult> UserCreate([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await userService.CreateAsync(dto);
            return CreatedAtAction(nameof(UserCreate), null);
        }

        [HttpPost(template:"login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO dto)
        {
            try
            {
                var token = await authService.LoginAsync(dto);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new {message = "Invalid login or password"});
            }
        }
    }
}

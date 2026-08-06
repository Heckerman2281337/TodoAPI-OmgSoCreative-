using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        [AllowAnonymous]
        public async Task<IActionResult> UserCreate([FromBody] RegisterDTO dto)
        {
            await userService.CreateAsync(dto);
            return CreatedAtAction(nameof(UserCreate), null);
        }

        [HttpPost(template:"login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO dto)
        {
            try
            {
                var token = await authService.LoginAsync(dto);
                return Ok(token);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new {message = "Invalid login or password"});
            }
        }
        [HttpPost(template:"refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshAsync([FromBody] string oldToken)
        {
            try
            {
                var token = await authService.RefreshAsync(oldToken);
                return Ok(token);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid login or password" });
            }
        }

        [HttpPost(template:"logout")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync([FromBody] string token)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await authService.LogoutAsync(token, userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Couldn't logout" });
            }
        }
    }
}

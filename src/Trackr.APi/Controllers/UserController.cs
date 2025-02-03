using Microsoft.AspNetCore.Mvc;
using Trackr.Application.DTOs;
using Trackr.Application.Interfaces;

namespace Trackr.Api.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UserController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> RegisterUser(RegisterDTO user)
        {
            try
            {
                bool result = await _authService.RegisterAsync(user);

                if (result) return Ok();
                return BadRequest();
            }
            catch(Exception ex) 
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

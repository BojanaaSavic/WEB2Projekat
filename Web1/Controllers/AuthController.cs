using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WebProjekat.Application.DTOs;
using WebProjekat.Application.Interfaces;

namespace Web1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [Tags("Authentication")]
        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="dto">User registration details</param>
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            try
            {
                var res = await _userService.RegisterAsync(dto);
                return Created("", res);
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        [Tags("Authentication")]
        /// <summary>
        /// Login and receive JWT token
        /// </summary>
        /// <param name="dto">Login credentials</param>
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var res = await _userService.AuthenticateAsync(dto);
            if (res == null) return Unauthorized();
            return Ok(res);
        }

        [Authorize]
        [HttpGet("me")]
        [Tags("Authentication")]
        /// <summary>
        /// Get current authenticated user's profile
        /// </summary>
        public IActionResult Me()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(idClaim))
            {
                return Unauthorized();
            }

            return Ok(new
            {
                id = Guid.Parse(idClaim),
                name,
                email,
                role
            });
        }
    }
}
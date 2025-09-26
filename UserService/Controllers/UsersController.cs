using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public IActionResult Register(UserRegisterDto dto)
        {
            var token = _service.Register(dto);
            if (token == null) return BadRequest("Usuário já existe.");
            return Ok(new { Token = token });
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginDto dto)
        {
            var token = _service.Login(dto);
            if (token == null) return Unauthorized();
            return Ok(new { Token = token });
        }
    }
}
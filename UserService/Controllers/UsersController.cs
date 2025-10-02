using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.DTOs;
using UserService.Models;
using UserService.Models.DTOs;
using UserService.Services;
using UserService.Configurations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AppDbContext _context;

        public UsersController(IUserService userService, AppDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        // POST: api/users/register
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(UserRegisterRequest request)
        {
            var dto = new UserRegisterDto
            {
                Nome = request.Nome,
                Email = request.Email,
                Senha = request.Senha,
                Role = UserRoles.User // sempre nasce como User
            };

            var token = _userService.Register(dto);
            if (token == null)
                return BadRequest("Usuário com este e-mail já existe.");

            return Ok(new { token });
        }

        // POST: api/users/login
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var dto = new UserLoginDto
            {
                Email = request.Email,
                Senha = request.Senha
            };

            var token = _userService.Login(dto);
            if (token == null)
                return Unauthorized("Credenciais inválidas.");

            return Ok(new { token });
        }

        // GET: api/users
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            var response = users.Select(user => new UserResponse
            {
                Id = user.Id,
                Nome = user.Nome,
                Email = user.Email,
                Role = user.Role
            });

            return Ok(response);
        }

        // GET: api/users/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            var userResponse = new UserResponse
            {
                Id = user.Id,
                Nome = user.Nome,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(userResponse);
        }

        // PUT: api/users/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID do usuário não corresponde.");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.Nome = request.Nome;
            user.Email = request.Email;

            if (!string.IsNullOrEmpty(request.Senha))
            {
                user.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
            }

            // 🚫 Role NÃO é alterada aqui!
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/users/{id}
        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/users/{id}/role (apenas Admin pode promover/rebaixar)
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] string newRole)
        {
            if (!UserRoles.All.Contains(newRole))
                return BadRequest("Role inválida. Roles permitidas: " + string.Join(", ", UserRoles.All));

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.Role = newRole;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Usuário {user.Email} agora é {newRole}" });
        }
    }
}
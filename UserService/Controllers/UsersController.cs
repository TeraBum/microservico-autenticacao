using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;
using UserService.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
                return BadRequest("Usuário com este e-mail já existe.");

            var newUser = new User
            {
                Nome = request.Nome,
                Email = request.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
                Role = request.Role
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var userResponse = new UserResponse
            {
                Id = newUser.Id,
                Nome = newUser.Nome,
                Email = newUser.Email,
                Role = newUser.Role
            };

            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, userResponse);
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Senha, user.SenhaHash))
                return Unauthorized("Credenciais inválidas.");

            var userResponse = new UserResponse
            {
                Id = user.Id,
                Nome = user.Nome,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(new
            {
                message = "Login realizado com sucesso.",
                user = userResponse
            });
        }

        // GET: api/users
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

            user.Role = request.Role;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/users/{id}
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
    }
}

using FashionStore.Data;
using FashionStore.Entities.Dtos;
using FashionStore.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FashionStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController:ControllerBase
    {
        private readonly FStoreDbContext _context;
        public UserController(FStoreDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUser()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == model.email);

            if (user == null || user.password != model.password)
            {
                return Unauthorized(new { Message = "Invalid email or password" });
            }
            var userResponse = new
            {
                id = user.id,
                email = user.email,
                name = user.name,
                address = user.address,
                phone = user.phone,
            };

            return Ok(new { Message = "Login successful", data = userResponse } );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (model.password != model.confirmPassword)
            {
                return BadRequest(new { Message = "Passwords do not match" });
            }

            var existingUser = await _context.Users.AnyAsync(u => u.email == model.email);
            if (existingUser)
            {
                return BadRequest(new { Message = "email already exists" });
            }

            var newUser = new User
            {
                id = Guid.NewGuid().ToString(),
                name = model.name,
                email = model.email,
                password = model.password,
                phone = model.phone,
                address = model.address
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Registration successful", User = newUser });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.id == id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Update các field
            user.name = model.name;
            user.phone = model.phone;
            user.address = model.address;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Trả về user (ẩn password)
            var result = new
            {
                id = user.id,
                email = user.email,
                name = user.name,
                phone = user.phone,
                address = user.address
            };

            return Ok(result);
        }
    }
}

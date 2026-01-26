using Backend.Data;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequestDto dto)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized("Invalid credentials");

            bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
            if (!isValid)
                return Unauthorized("Invalid credentials");

            var token = _jwtService.GenerateToken(user.UserId, user.Email);

            return Ok(new LoginResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email
            });
        }

    }
}

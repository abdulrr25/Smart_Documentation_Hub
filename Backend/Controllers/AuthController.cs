using System.Text.RegularExpressions;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
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
        private readonly EmailService _emailService;

        public AuthController(
            AppDbContext context,
            JwtService jwtService,
            EmailService emailService)
        {
            _context = context;
            _jwtService = jwtService;
            _emailService = emailService;
        }


        // 🔒 EMAIL VALIDATION
        private bool IsValidEmail(string email)
        {
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        // 🔐 PASSWORD VALIDATION
        private bool IsStrongPassword(string password)
        {
            var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
            return Regex.IsMatch(password, pattern);
        }

        // LOGIN (EMAIL OR USERNAME)
        [HttpPost("login")]
        public IActionResult Login(LoginRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Email/Username and Password are required");
            }

            var input = dto.Email.Trim().ToLower();

            var user = _context.Users
                .FirstOrDefault(u =>
                    u.Email == input ||
                    u.Name.ToLower() == input
                );

            if (user == null)
                return Unauthorized("Invalid credentials");

            bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);

            if (!isValid)
                return Unauthorized("Invalid credentials");

            var token = _jwtService.GenerateToken(
                user.UserId,
                user.Email,
                user.Name
            );

            return Ok(new LoginResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email
            });
        }

        // REGISTER
        [HttpPost("register")]
        public IActionResult Register(RegisterRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("All fields are required" );
            }

            var name = dto.Name.Trim();
            var email = dto.Email.Trim().ToLower();

            if (!IsValidEmail(email))
                return BadRequest("Invalid email format" );

            if (!IsStrongPassword(dto.Password))
                return BadRequest(new
                {
                    message = "Password must be at least 8 characters and include uppercase, lowercase, number, and special character"
                });

            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email == email);

            if (existingUser != null)
                return BadRequest("User already exists");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Name = name,
                Email = email,
                Password = hashedPassword
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var token = _jwtService.GenerateToken(
                user.UserId,
                user.Email,
                user.Name
            );

            return Ok(new LoginResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email
            });
        }

        // FORGOT PASSWORD
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required");

            var email = dto.Email.Trim().ToLower();
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                return Ok("If the email exists, a reset link will be sent" );

            var token = Guid.NewGuid().ToString();

            var resetToken = new PasswordResetToken
            {
                UserId = user.UserId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            var resetLink = $"http://localhost:3000/reset-password?token={token}";

            await _emailService.SendResetEmail(user.Email, resetLink);

            return Ok("If the email exists, a reset link will be sent");
        }

        // RESET PASSWORD
        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Token) ||
                string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return BadRequest("Invalid request");
            }

            if (!IsStrongPassword(dto.NewPassword))
            {
                return BadRequest(new
                {
                    message = "Password must be at least 8 characters and include uppercase, lowercase, number, and special character"
                });
            }

            var resetToken = _context.PasswordResetTokens
                .FirstOrDefault(t =>
                    t.Token == dto.Token &&
                    !t.IsUsed &&
                    t.ExpiresAt > DateTime.UtcNow);

            if (resetToken == null)
                return BadRequest("Invalid or expired token");

            var user = _context.Users
                .FirstOrDefault(u => u.UserId == resetToken.UserId);

            if (user == null)
                return BadRequest("Invalid token");

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            resetToken.IsUsed = true;

            _context.SaveChanges();

            return Ok("Password reset successful");
        }
    }
}
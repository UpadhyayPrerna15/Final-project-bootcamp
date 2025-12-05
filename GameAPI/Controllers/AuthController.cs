using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameAPI.Data;
using GameAPI.DTOs;
using GameAPI.Models;
using GameAPI.Services;

namespace GameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly GameDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AuthController> _logger;
        
        public AuthController(
            GameDbContext context, 
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            ILogger<AuthController> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }
        
        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return BadRequest(new { error = "Username already exists" });
            }
            
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest(new { error = "Email already exists" });
            }
            
            // Create new user
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = _passwordHasher.HashPassword(registerDto.Password),
                Role = "Player", // Default role
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("New user registered: {Username}", user.Username);
            
            // Generate token
            var token = _tokenService.GenerateToken(user);
            var expirationMinutes = int.Parse(Program.Configuration["JwtSettings:ExpirationMinutes"] ?? "60");
            
            return CreatedAtAction(nameof(Register), new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Expiration = DateTime.UtcNow.AddMinutes(expirationMinutes)
            });
        }
        
        /// <summary>
        /// Login with existing credentials
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            // Find user by username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);
            
            if (user == null || !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { error = "Invalid username or password" });
            }
            
            // Update last login
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("User logged in: {Username}", user.Username);
            
            // Generate token
            var token = _tokenService.GenerateToken(user);
            var expirationMinutes = int.Parse(Program.Configuration["JwtSettings:ExpirationMinutes"] ?? "60");
            
            return Ok(new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Expiration = DateTime.UtcNow.AddMinutes(expirationMinutes)
            });
        }
    }
}

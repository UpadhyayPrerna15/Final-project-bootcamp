using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameAPI.Data;
using GameAPI.DTOs;
using GameAPI.Models;
using System.Security.Claims;

namespace GameAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PlayersController : ControllerBase
    {
        private readonly GameDbContext _context;
        private readonly ILogger<PlayersController> _logger;
        
        public PlayersController(GameDbContext context, ILogger<PlayersController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
        
        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }
        
        /// <summary>
        /// Get all players (Admin only) or current user's players
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerResponseDto>>> GetPlayers(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            
            IQueryable<Player> query = _context.Players;
            
            if (!IsAdmin())
            {
                var userId = GetCurrentUserId();
                query = query.Where(p => p.UserId == userId);
            }
            
            var totalCount = await query.CountAsync();
            var players = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PlayerResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Level = p.Level,
                    Experience = p.Experience,
                    Gold = p.Gold,
                    Health = p.Health,
                    MaxHealth = p.MaxHealth,
                    Mana = p.Mana,
                    MaxMana = p.MaxMana,
                    CreatedAt = p.CreatedAt,
                    LastPlayed = p.LastPlayed,
                    UserId = p.UserId
                })
                .ToListAsync();
            
            Response.Headers["X-Total-Count"] = totalCount.ToString();
            Response.Headers["X-Page"] = page.ToString();
            Response.Headers["X-Page-Size"] = pageSize.ToString();
            
            return Ok(players);
        }
        
        /// <summary>
        /// Get a specific player by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerResponseDto>> GetPlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);
            
            if (player == null)
            {
                return NotFound(new { error = "Player not found" });
            }
            
            // Check if user owns this player or is admin
            if (!IsAdmin() && player.UserId != GetCurrentUserId())
            {
                return Forbid();
            }
            
            var response = new PlayerResponseDto
            {
                Id = player.Id,
                Name = player.Name,
                Level = player.Level,
                Experience = player.Experience,
                Gold = player.Gold,
                Health = player.Health,
                MaxHealth = player.MaxHealth,
                Mana = player.Mana,
                MaxMana = player.MaxMana,
                CreatedAt = player.CreatedAt,
                LastPlayed = player.LastPlayed,
                UserId = player.UserId
            };
            
            return Ok(response);
        }
        
        /// <summary>
        /// Create a new player
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PlayerResponseDto>> CreatePlayer(PlayerCreateDto playerDto)
        {
            var userId = GetCurrentUserId();
            
            var player = new Player
            {
                Name = playerDto.Name,
                UserId = userId,
                Level = 1,
                Experience = 0,
                Gold = 100,
                Health = 100,
                MaxHealth = 100,
                Mana = 50,
                MaxMana = 50,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("New player created: {PlayerName} for user {UserId}", player.Name, userId);
            
            var response = new PlayerResponseDto
            {
                Id = player.Id,
                Name = player.Name,
                Level = player.Level,
                Experience = player.Experience,
                Gold = player.Gold,
                Health = player.Health,
                MaxHealth = player.MaxHealth,
                Mana = player.Mana,
                MaxMana = player.MaxMana,
                CreatedAt = player.CreatedAt,
                LastPlayed = player.LastPlayed,
                UserId = player.UserId
            };
            
            return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, response);
        }
        
        /// <summary>
        /// Update an existing player
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(int id, PlayerUpdateDto playerDto)
        {
            var player = await _context.Players.FindAsync(id);
            
            if (player == null)
            {
                return NotFound(new { error = "Player not found" });
            }
            
            // Check if user owns this player or is admin
            if (!IsAdmin() && player.UserId != GetCurrentUserId())
            {
                return Forbid();
            }
            
            // Update only provided fields
            if (playerDto.Name != null) player.Name = playerDto.Name;
            if (playerDto.Level.HasValue) player.Level = playerDto.Level.Value;
            if (playerDto.Experience.HasValue) player.Experience = playerDto.Experience.Value;
            if (playerDto.Gold.HasValue) player.Gold = playerDto.Gold.Value;
            if (playerDto.Health.HasValue) player.Health = playerDto.Health.Value;
            if (playerDto.Mana.HasValue) player.Mana = playerDto.Mana.Value;
            
            player.LastPlayed = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Player updated: {PlayerId}", id);
            
            return NoContent();
        }
        
        /// <summary>
        /// Delete a player
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);
            
            if (player == null)
            {
                return NotFound(new { error = "Player not found" });
            }
            
            // Check if user owns this player or is admin
            if (!IsAdmin() && player.UserId != GetCurrentUserId())
            {
                return Forbid();
            }
            
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Player deleted: {PlayerId}", id);
            
            return NoContent();
        }
    }
}

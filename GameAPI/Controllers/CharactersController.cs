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
    public class CharactersController : ControllerBase
    {
        private readonly GameDbContext _context;
        private readonly ILogger<CharactersController> _logger;
        
        public CharactersController(GameDbContext context, ILogger<CharactersController> logger)
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
        
        private async Task<bool> UserOwnsPlayer(int playerId)
        {
            if (IsAdmin()) return true;
            var player = await _context.Players.FindAsync(playerId);
            return player != null && player.UserId == GetCurrentUserId();
        }
        
        /// <summary>
        /// Get all characters with optional filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterResponseDto>>> GetCharacters(
            [FromQuery] int? playerId = null,
            [FromQuery] string? characterClass = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            
            IQueryable<Character> query = _context.Characters.Include(c => c.Player);
            
            // Filter by player if specified
            if (playerId.HasValue)
            {
                if (!await UserOwnsPlayer(playerId.Value))
                {
                    return Forbid();
                }
                query = query.Where(c => c.PlayerId == playerId.Value);
            }
            else if (!IsAdmin())
            {
                // Non-admins can only see their own characters
                var userId = GetCurrentUserId();
                query = query.Where(c => c.Player.UserId == userId);
            }
            
            // Filter by character class if specified
            if (!string.IsNullOrEmpty(characterClass))
            {
                query = query.Where(c => c.CharacterClass.ToLower() == characterClass.ToLower());
            }
            
            var totalCount = await query.CountAsync();
            var characters = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CharacterResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    CharacterClass = c.CharacterClass,
                    Level = c.Level,
                    Experience = c.Experience,
                    Strength = c.Strength,
                    Intelligence = c.Intelligence,
                    Dexterity = c.Dexterity,
                    Vitality = c.Vitality,
                    Health = c.Health,
                    MaxHealth = c.MaxHealth,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    PlayerId = c.PlayerId
                })
                .ToListAsync();
            
            Response.Headers["X-Total-Count"] = totalCount.ToString();
            
            return Ok(characters);
        }
        
        /// <summary>
        /// Get a specific character by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CharacterResponseDto>> GetCharacter(int id)
        {
            var character = await _context.Characters.Include(c => c.Player).FirstOrDefaultAsync(c => c.Id == id);
            
            if (character == null)
            {
                return NotFound(new { error = "Character not found" });
            }
            
            if (!await UserOwnsPlayer(character.PlayerId))
            {
                return Forbid();
            }
            
            var response = new CharacterResponseDto
            {
                Id = character.Id,
                Name = character.Name,
                CharacterClass = character.CharacterClass,
                Level = character.Level,
                Experience = character.Experience,
                Strength = character.Strength,
                Intelligence = character.Intelligence,
                Dexterity = character.Dexterity,
                Vitality = character.Vitality,
                Health = character.Health,
                MaxHealth = character.MaxHealth,
                IsActive = character.IsActive,
                CreatedAt = character.CreatedAt,
                PlayerId = character.PlayerId
            };
            
            return Ok(response);
        }
        
        /// <summary>
        /// Create a new character
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CharacterResponseDto>> CreateCharacter(CharacterCreateDto characterDto)
        {
            if (!await UserOwnsPlayer(characterDto.PlayerId))
            {
                return Forbid();
            }
            
            var character = new Character
            {
                Name = characterDto.Name,
                CharacterClass = characterDto.CharacterClass,
                PlayerId = characterDto.PlayerId,
                Level = 1,
                Experience = 0,
                Strength = 10,
                Intelligence = 10,
                Dexterity = 10,
                Vitality = 10,
                Health = 100,
                MaxHealth = 100,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("New character created: {CharacterName}", character.Name);
            
            var response = new CharacterResponseDto
            {
                Id = character.Id,
                Name = character.Name,
                CharacterClass = character.CharacterClass,
                Level = character.Level,
                Experience = character.Experience,
                Strength = character.Strength,
                Intelligence = character.Intelligence,
                Dexterity = character.Dexterity,
                Vitality = character.Vitality,
                Health = character.Health,
                MaxHealth = character.MaxHealth,
                IsActive = character.IsActive,
                CreatedAt = character.CreatedAt,
                PlayerId = character.PlayerId
            };
            
            return CreatedAtAction(nameof(GetCharacter), new { id = character.Id }, response);
        }
        
        /// <summary>
        /// Update an existing character
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCharacter(int id, CharacterUpdateDto characterDto)
        {
            var character = await _context.Characters.FindAsync(id);
            
            if (character == null)
            {
                return NotFound(new { error = "Character not found" });
            }
            
            if (!await UserOwnsPlayer(character.PlayerId))
            {
                return Forbid();
            }
            
            // Update only provided fields
            if (characterDto.Name != null) character.Name = characterDto.Name;
            if (characterDto.Level.HasValue) character.Level = characterDto.Level.Value;
            if (characterDto.Experience.HasValue) character.Experience = characterDto.Experience.Value;
            if (characterDto.Strength.HasValue) character.Strength = characterDto.Strength.Value;
            if (characterDto.Intelligence.HasValue) character.Intelligence = characterDto.Intelligence.Value;
            if (characterDto.Dexterity.HasValue) character.Dexterity = characterDto.Dexterity.Value;
            if (characterDto.Vitality.HasValue) character.Vitality = characterDto.Vitality.Value;
            if (characterDto.Health.HasValue) character.Health = characterDto.Health.Value;
            if (characterDto.IsActive.HasValue) character.IsActive = characterDto.IsActive.Value;
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Character updated: {CharacterId}", id);
            
            return NoContent();
        }
        
        /// <summary>
        /// Delete a character
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            var character = await _context.Characters.FindAsync(id);
            
            if (character == null)
            {
                return NotFound(new { error = "Character not found" });
            }
            
            if (!await UserOwnsPlayer(character.PlayerId))
            {
                return Forbid();
            }
            
            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Character deleted: {CharacterId}", id);
            
            return NoContent();
        }
    }
}

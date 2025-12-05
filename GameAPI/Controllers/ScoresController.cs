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
    public class ScoresController : ControllerBase
    {
        private readonly GameDbContext _context;
        private readonly ILogger<ScoresController> _logger;
        
        public ScoresController(GameDbContext context, ILogger<ScoresController> logger)
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
        /// Get all scores with optional filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScoreResponseDto>>> GetScores(
            [FromQuery] int? playerId = null,
            [FromQuery] string? gameMode = null,
            [FromQuery] bool? highScoresOnly = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            
            IQueryable<Score> query = _context.Scores.Include(s => s.Player);
            
            // Filter by player if specified
            if (playerId.HasValue)
            {
                if (!await UserOwnsPlayer(playerId.Value))
                {
                    return Forbid();
                }
                query = query.Where(s => s.PlayerId == playerId.Value);
            }
            else if (!IsAdmin())
            {
                // Non-admins can only see their own scores
                var userId = GetCurrentUserId();
                query = query.Where(s => s.Player.UserId == userId);
            }
            
            // Filter by game mode if specified
            if (!string.IsNullOrEmpty(gameMode))
            {
                query = query.Where(s => s.GameMode.ToLower() == gameMode.ToLower());
            }
            
            // Filter by high scores if specified
            if (highScoresOnly.HasValue && highScoresOnly.Value)
            {
                query = query.Where(s => s.IsHighScore);
            }
            
            var totalCount = await query.CountAsync();
            var scores = await query
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.AchievedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new ScoreResponseDto
                {
                    Id = s.Id,
                    GameMode = s.GameMode,
                    Points = s.Points,
                    Kills = s.Kills,
                    Deaths = s.Deaths,
                    TimePlayed = s.TimePlayed,
                    DifficultyLevel = s.DifficultyLevel,
                    IsHighScore = s.IsHighScore,
                    AchievedAt = s.AchievedAt,
                    PlayerId = s.PlayerId
                })
                .ToListAsync();
            
            Response.Headers["X-Total-Count"] = totalCount.ToString();
            
            return Ok(scores);
        }
        
        /// <summary>
        /// Get leaderboard for a specific game mode
        /// </summary>
        [HttpGet("leaderboard/{gameMode}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetLeaderboard(
            string gameMode,
            [FromQuery] int top = 10)
        {
            if (top < 1 || top > 100) top = 10;
            
            var leaderboard = await _context.Scores
                .Include(s => s.Player)
                .Where(s => s.GameMode.ToLower() == gameMode.ToLower())
                .OrderByDescending(s => s.Points)
                .Take(top)
                .Select(s => new
                {
                    rank = 0, // Will be set later
                    playerName = s.Player.Name,
                    points = s.Points,
                    kills = s.Kills,
                    deaths = s.Deaths,
                    timePlayed = s.TimePlayed,
                    difficultyLevel = s.DifficultyLevel,
                    achievedAt = s.AchievedAt
                })
                .ToListAsync();
            
            // Add rank
            var rankedLeaderboard = leaderboard.Select((item, index) => new
            {
                rank = index + 1,
                item.playerName,
                item.points,
                item.kills,
                item.deaths,
                item.timePlayed,
                item.difficultyLevel,
                item.achievedAt
            });
            
            return Ok(rankedLeaderboard);
        }
        
        /// <summary>
        /// Get a specific score by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ScoreResponseDto>> GetScore(int id)
        {
            var score = await _context.Scores.Include(s => s.Player).FirstOrDefaultAsync(s => s.Id == id);
            
            if (score == null)
            {
                return NotFound(new { error = "Score not found" });
            }
            
            if (!await UserOwnsPlayer(score.PlayerId))
            {
                return Forbid();
            }
            
            var response = new ScoreResponseDto
            {
                Id = score.Id,
                GameMode = score.GameMode,
                Points = score.Points,
                Kills = score.Kills,
                Deaths = score.Deaths,
                TimePlayed = score.TimePlayed,
                DifficultyLevel = score.DifficultyLevel,
                IsHighScore = score.IsHighScore,
                AchievedAt = score.AchievedAt,
                PlayerId = score.PlayerId
            };
            
            return Ok(response);
        }
        
        /// <summary>
        /// Create a new score
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ScoreResponseDto>> CreateScore(ScoreCreateDto scoreDto)
        {
            if (!await UserOwnsPlayer(scoreDto.PlayerId))
            {
                return Forbid();
            }
            
            // Check if this is a high score for the player in this game mode
            var existingScores = await _context.Scores
                .Where(s => s.PlayerId == scoreDto.PlayerId && s.GameMode == scoreDto.GameMode)
                .ToListAsync();
            
            var isHighScore = !existingScores.Any() || scoreDto.Points > existingScores.Max(s => s.Points);
            
            // If this is a high score, update previous high scores
            if (isHighScore)
            {
                foreach (var oldScore in existingScores.Where(s => s.IsHighScore))
                {
                    oldScore.IsHighScore = false;
                }
            }
            
            var score = new Score
            {
                GameMode = scoreDto.GameMode,
                Points = scoreDto.Points,
                Kills = scoreDto.Kills,
                Deaths = scoreDto.Deaths,
                TimePlayed = scoreDto.TimePlayed,
                DifficultyLevel = scoreDto.DifficultyLevel,
                IsHighScore = isHighScore,
                PlayerId = scoreDto.PlayerId,
                AchievedAt = DateTime.UtcNow
            };
            
            _context.Scores.Add(score);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("New score created: {Points} points in {GameMode}", score.Points, score.GameMode);
            
            var response = new ScoreResponseDto
            {
                Id = score.Id,
                GameMode = score.GameMode,
                Points = score.Points,
                Kills = score.Kills,
                Deaths = score.Deaths,
                TimePlayed = score.TimePlayed,
                DifficultyLevel = score.DifficultyLevel,
                IsHighScore = score.IsHighScore,
                AchievedAt = score.AchievedAt,
                PlayerId = score.PlayerId
            };
            
            return CreatedAtAction(nameof(GetScore), new { id = score.Id }, response);
        }
        
        /// <summary>
        /// Delete a score
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScore(int id)
        {
            var score = await _context.Scores.FindAsync(id);
            
            if (score == null)
            {
                return NotFound(new { error = "Score not found" });
            }
            
            if (!await UserOwnsPlayer(score.PlayerId))
            {
                return Forbid();
            }
            
            _context.Scores.Remove(score);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Score deleted: {ScoreId}", id);
            
            return NoContent();
        }
    }
}

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
    public class ItemsController : ControllerBase
    {
        private readonly GameDbContext _context;
        private readonly ILogger<ItemsController> _logger;
        
        public ItemsController(GameDbContext context, ILogger<ItemsController> logger)
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
        
        private async Task<bool> UserOwnsPlayer(int? playerId)
        {
            if (!playerId.HasValue) return true;
            if (IsAdmin()) return true;
            var player = await _context.Players.FindAsync(playerId.Value);
            return player != null && player.UserId == GetCurrentUserId();
        }
        
        /// <summary>
        /// Get all items with optional filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemResponseDto>>> GetItems(
            [FromQuery] int? playerId = null,
            [FromQuery] string? itemType = null,
            [FromQuery] int? minRarity = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;
            
            IQueryable<Item> query = _context.Items.Include(i => i.Player);
            
            // Filter by player if specified
            if (playerId.HasValue)
            {
                if (!await UserOwnsPlayer(playerId.Value))
                {
                    return Forbid();
                }
                query = query.Where(i => i.PlayerId == playerId.Value);
            }
            else if (!IsAdmin())
            {
                // Non-admins can only see their own items
                var userId = GetCurrentUserId();
                query = query.Where(i => i.Player != null && i.Player.UserId == userId);
            }
            
            // Filter by item type if specified
            if (!string.IsNullOrEmpty(itemType))
            {
                query = query.Where(i => i.ItemType.ToLower() == itemType.ToLower());
            }
            
            // Filter by minimum rarity if specified
            if (minRarity.HasValue)
            {
                query = query.Where(i => i.Rarity >= minRarity.Value);
            }
            
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(i => i.Rarity)
                .ThenByDescending(i => i.AcquiredAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new ItemResponseDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    ItemType = i.ItemType,
                    AttackBonus = i.AttackBonus,
                    DefenseBonus = i.DefenseBonus,
                    Value = i.Value,
                    Rarity = i.Rarity,
                    IsEquipped = i.IsEquipped,
                    Quantity = i.Quantity,
                    AcquiredAt = i.AcquiredAt,
                    PlayerId = i.PlayerId
                })
                .ToListAsync();
            
            Response.Headers["X-Total-Count"] = totalCount.ToString();
            
            return Ok(items);
        }
        
        /// <summary>
        /// Get a specific item by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemResponseDto>> GetItem(int id)
        {
            var item = await _context.Items.Include(i => i.Player).FirstOrDefaultAsync(i => i.Id == id);
            
            if (item == null)
            {
                return NotFound(new { error = "Item not found" });
            }
            
            if (!await UserOwnsPlayer(item.PlayerId))
            {
                return Forbid();
            }
            
            var response = new ItemResponseDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                ItemType = item.ItemType,
                AttackBonus = item.AttackBonus,
                DefenseBonus = item.DefenseBonus,
                Value = item.Value,
                Rarity = item.Rarity,
                IsEquipped = item.IsEquipped,
                Quantity = item.Quantity,
                AcquiredAt = item.AcquiredAt,
                PlayerId = item.PlayerId
            };
            
            return Ok(response);
        }
        
        /// <summary>
        /// Create a new item
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ItemResponseDto>> CreateItem(ItemCreateDto itemDto)
        {
            if (itemDto.PlayerId.HasValue && !await UserOwnsPlayer(itemDto.PlayerId.Value))
            {
                return Forbid();
            }
            
            var item = new Item
            {
                Name = itemDto.Name,
                Description = itemDto.Description,
                ItemType = itemDto.ItemType,
                AttackBonus = itemDto.AttackBonus,
                DefenseBonus = itemDto.DefenseBonus,
                Value = itemDto.Value,
                Rarity = itemDto.Rarity,
                IsEquipped = false,
                Quantity = itemDto.Quantity,
                PlayerId = itemDto.PlayerId,
                AcquiredAt = DateTime.UtcNow
            };
            
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("New item created: {ItemName}", item.Name);
            
            var response = new ItemResponseDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                ItemType = item.ItemType,
                AttackBonus = item.AttackBonus,
                DefenseBonus = item.DefenseBonus,
                Value = item.Value,
                Rarity = item.Rarity,
                IsEquipped = item.IsEquipped,
                Quantity = item.Quantity,
                AcquiredAt = item.AcquiredAt,
                PlayerId = item.PlayerId
            };
            
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, response);
        }
        
        /// <summary>
        /// Update an existing item
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, ItemUpdateDto itemDto)
        {
            var item = await _context.Items.FindAsync(id);
            
            if (item == null)
            {
                return NotFound(new { error = "Item not found" });
            }
            
            if (!await UserOwnsPlayer(item.PlayerId))
            {
                return Forbid();
            }
            
            // Update only provided fields
            if (itemDto.Name != null) item.Name = itemDto.Name;
            if (itemDto.Description != null) item.Description = itemDto.Description;
            if (itemDto.IsEquipped.HasValue) item.IsEquipped = itemDto.IsEquipped.Value;
            if (itemDto.Quantity.HasValue) item.Quantity = itemDto.Quantity.Value;
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Item updated: {ItemId}", id);
            
            return NoContent();
        }
        
        /// <summary>
        /// Delete an item
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            
            if (item == null)
            {
                return NotFound(new { error = "Item not found" });
            }
            
            if (!await UserOwnsPlayer(item.PlayerId))
            {
                return Forbid();
            }
            
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Item deleted: {ItemId}", id);
            
            return NoContent();
        }
    }
}

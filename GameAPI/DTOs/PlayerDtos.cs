using System.ComponentModel.DataAnnotations;

namespace GameAPI.DTOs
{
    public class PlayerCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
    }
    
    public class PlayerUpdateDto
    {
        [StringLength(50, MinimumLength = 2)]
        public string? Name { get; set; }
        
        [Range(1, 100)]
        public int? Level { get; set; }
        
        [Range(0, int.MaxValue)]
        public int? Experience { get; set; }
        
        [Range(0, int.MaxValue)]
        public int? Gold { get; set; }
        
        [Range(1, 1000)]
        public int? Health { get; set; }
        
        [Range(0, 1000)]
        public int? Mana { get; set; }
    }
    
    public class PlayerResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Gold { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Mana { get; set; }
        public int MaxMana { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastPlayed { get; set; }
        public int UserId { get; set; }
    }
}

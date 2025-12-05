using System.ComponentModel.DataAnnotations;

namespace GameAPI.DTOs
{
    public class CharacterCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(30)]
        public string CharacterClass { get; set; } = string.Empty;
        
        [Required]
        public int PlayerId { get; set; }
    }
    
    public class CharacterUpdateDto
    {
        [StringLength(50, MinimumLength = 2)]
        public string? Name { get; set; }
        
        [Range(1, 100)]
        public int? Level { get; set; }
        
        [Range(0, int.MaxValue)]
        public int? Experience { get; set; }
        
        [Range(1, 1000)]
        public int? Strength { get; set; }
        
        [Range(1, 1000)]
        public int? Intelligence { get; set; }
        
        [Range(1, 1000)]
        public int? Dexterity { get; set; }
        
        [Range(1, 1000)]
        public int? Vitality { get; set; }
        
        [Range(1, int.MaxValue)]
        public int? Health { get; set; }
        
        public bool? IsActive { get; set; }
    }
    
    public class CharacterResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CharacterClass { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Dexterity { get; set; }
        public int Vitality { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PlayerId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    public class Character
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(30)]
        public string CharacterClass { get; set; } = string.Empty; // Warrior, Mage, Ranger, etc.
        
        [Range(1, 100)]
        public int Level { get; set; } = 1;
        
        [Range(0, int.MaxValue)]
        public int Experience { get; set; } = 0;
        
        [Range(1, 1000)]
        public int Strength { get; set; } = 10;
        
        [Range(1, 1000)]
        public int Intelligence { get; set; } = 10;
        
        [Range(1, 1000)]
        public int Dexterity { get; set; } = 10;
        
        [Range(1, 1000)]
        public int Vitality { get; set; } = 10;
        
        [Range(1, int.MaxValue)]
        public int Health { get; set; } = 100;
        
        [Range(1, int.MaxValue)]
        public int MaxHealth { get; set; } = 100;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign key
        public int PlayerId { get; set; }
        
        // Navigation property
        [ForeignKey("PlayerId")]
        public Player Player { get; set; } = null!;
    }
}

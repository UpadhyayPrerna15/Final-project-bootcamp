using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    public class Player
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        
        [Range(1, 100)]
        public int Level { get; set; } = 1;
        
        [Range(0, int.MaxValue)]
        public int Experience { get; set; } = 0;
        
        [Range(0, int.MaxValue)]
        public int Gold { get; set; } = 100;
        
        [Range(1, 1000)]
        public int Health { get; set; } = 100;
        
        [Range(1, 1000)]
        public int MaxHealth { get; set; } = 100;
        
        [Range(0, 1000)]
        public int Mana { get; set; } = 50;
        
        [Range(0, 1000)]
        public int MaxMana { get; set; } = 50;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastPlayed { get; set; }
        
        // Foreign key
        public int UserId { get; set; }
        
        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        
        public ICollection<Character> Characters { get; set; } = new List<Character>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<Score> Scores { get; set; } = new List<Score>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    public class Item
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(30)]
        public string ItemType { get; set; } = string.Empty; // Weapon, Armor, Potion, Consumable, etc.
        
        [Range(0, 1000)]
        public int AttackBonus { get; set; } = 0;
        
        [Range(0, 1000)]
        public int DefenseBonus { get; set; } = 0;
        
        [Range(0, int.MaxValue)]
        public int Value { get; set; } = 0;
        
        [Range(1, 10)]
        public int Rarity { get; set; } = 1; // 1-10, where 10 is legendary
        
        public bool IsEquipped { get; set; } = false;
        
        [Range(1, 999)]
        public int Quantity { get; set; } = 1;
        
        public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
        
        // Foreign key
        public int? PlayerId { get; set; }
        
        // Navigation property
        [ForeignKey("PlayerId")]
        public Player? Player { get; set; }
    }
}

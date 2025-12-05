using System.ComponentModel.DataAnnotations;

namespace GameAPI.DTOs
{
    public class ItemCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(30)]
        public string ItemType { get; set; } = string.Empty;
        
        [Range(0, 1000)]
        public int AttackBonus { get; set; } = 0;
        
        [Range(0, 1000)]
        public int DefenseBonus { get; set; } = 0;
        
        [Range(0, int.MaxValue)]
        public int Value { get; set; } = 0;
        
        [Range(1, 10)]
        public int Rarity { get; set; } = 1;
        
        [Range(1, 999)]
        public int Quantity { get; set; } = 1;
        
        public int? PlayerId { get; set; }
    }
    
    public class ItemUpdateDto
    {
        [StringLength(50, MinimumLength = 2)]
        public string? Name { get; set; }
        
        [StringLength(200)]
        public string? Description { get; set; }
        
        public bool? IsEquipped { get; set; }
        
        [Range(1, 999)]
        public int? Quantity { get; set; }
    }
    
    public class ItemResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public int AttackBonus { get; set; }
        public int DefenseBonus { get; set; }
        public int Value { get; set; }
        public int Rarity { get; set; }
        public bool IsEquipped { get; set; }
        public int Quantity { get; set; }
        public DateTime AcquiredAt { get; set; }
        public int? PlayerId { get; set; }
    }
}

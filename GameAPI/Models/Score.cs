using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameAPI.Models
{
    public class Score
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string GameMode { get; set; } = string.Empty; // Arena, Quest, Dungeon, etc.
        
        [Range(0, int.MaxValue)]
        public int Points { get; set; } = 0;
        
        [Range(0, int.MaxValue)]
        public int Kills { get; set; } = 0;
        
        [Range(0, int.MaxValue)]
        public int Deaths { get; set; } = 0;
        
        [Range(0, double.MaxValue)]
        public double TimePlayed { get; set; } = 0; // In seconds
        
        [Range(1, 100)]
        public int DifficultyLevel { get; set; } = 1;
        
        public bool IsHighScore { get; set; } = false;
        
        public DateTime AchievedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign key
        public int PlayerId { get; set; }
        
        // Navigation property
        [ForeignKey("PlayerId")]
        public Player Player { get; set; } = null!;
    }
}

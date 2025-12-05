using System.ComponentModel.DataAnnotations;

namespace GameAPI.DTOs
{
    public class ScoreCreateDto
    {
        [Required]
        [StringLength(50)]
        public string GameMode { get; set; } = string.Empty;
        
        [Range(0, int.MaxValue)]
        public int Points { get; set; } = 0;
        
        [Range(0, int.MaxValue)]
        public int Kills { get; set; } = 0;
        
        [Range(0, int.MaxValue)]
        public int Deaths { get; set; } = 0;
        
        [Range(0, double.MaxValue)]
        public double TimePlayed { get; set; } = 0;
        
        [Range(1, 100)]
        public int DifficultyLevel { get; set; } = 1;
        
        [Required]
        public int PlayerId { get; set; }
    }
    
    public class ScoreResponseDto
    {
        public int Id { get; set; }
        public string GameMode { get; set; } = string.Empty;
        public int Points { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public double TimePlayed { get; set; }
        public int DifficultyLevel { get; set; }
        public bool IsHighScore { get; set; }
        public DateTime AchievedAt { get; set; }
        public int PlayerId { get; set; }
    }
}

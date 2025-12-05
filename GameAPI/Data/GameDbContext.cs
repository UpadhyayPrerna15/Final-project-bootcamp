using Microsoft.EntityFrameworkCore;
using GameAPI.Models;

namespace GameAPI.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Score> Scores { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure User entity
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
                
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            
            // Configure relationships
            modelBuilder.Entity<Player>()
                .HasOne(p => p.User)
                .WithMany(u => u.Players)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Character>()
                .HasOne(c => c.Player)
                .WithMany(p => p.Characters)
                .HasForeignKey(c => c.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Player)
                .WithMany(p => p.Items)
                .HasForeignKey(i => i.PlayerId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<Score>()
                .HasOne(s => s.Player)
                .WithMany(p => p.Scores)
                .HasForeignKey(s => s.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Seed initial data
            SeedData(modelBuilder);
        }
        
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Users (passwords are hashed version of "password123")
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Username = "admin", 
                    Email = "admin@gameapi.com",
                    PasswordHash = "$2a$11$8VvXZKmBbYnZ6FhKqDvvF.rKGFd5YCZRYfLJ0L4p3vYVE7xBG6pYy", // password123
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                },
                new User 
                { 
                    Id = 2, 
                    Username = "player1", 
                    Email = "player1@gameapi.com",
                    PasswordHash = "$2a$11$8VvXZKmBbYnZ6FhKqDvvF.rKGFd5YCZRYfLJ0L4p3vYVE7xBG6pYy", // password123
                    Role = "Player",
                    CreatedAt = DateTime.UtcNow
                },
                new User 
                { 
                    Id = 3, 
                    Username = "player2", 
                    Email = "player2@gameapi.com",
                    PasswordHash = "$2a$11$8VvXZKmBbYnZ6FhKqDvvF.rKGFd5YCZRYfLJ0L4p3vYVE7xBG6pYy", // password123
                    Role = "Player",
                    CreatedAt = DateTime.UtcNow
                }
            );
            
            // Seed Players
            modelBuilder.Entity<Player>().HasData(
                new Player 
                { 
                    Id = 1, 
                    Name = "DragonSlayer", 
                    Level = 15, 
                    Experience = 4500,
                    Gold = 2500,
                    Health = 150,
                    MaxHealth = 150,
                    Mana = 100,
                    MaxMana = 100,
                    UserId = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new Player 
                { 
                    Id = 2, 
                    Name = "ShadowHunter", 
                    Level = 10, 
                    Experience = 2000,
                    Gold = 1200,
                    Health = 120,
                    MaxHealth = 120,
                    Mana = 80,
                    MaxMana = 80,
                    UserId = 3,
                    CreatedAt = DateTime.UtcNow
                }
            );
            
            // Seed Characters
            modelBuilder.Entity<Character>().HasData(
                new Character 
                { 
                    Id = 1, 
                    Name = "Thorin", 
                    CharacterClass = "Warrior",
                    Level = 15,
                    Experience = 4500,
                    Strength = 25,
                    Intelligence = 10,
                    Dexterity = 15,
                    Vitality = 30,
                    Health = 300,
                    MaxHealth = 300,
                    PlayerId = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new Character 
                { 
                    Id = 2, 
                    Name = "Gandalf", 
                    CharacterClass = "Mage",
                    Level = 12,
                    Experience = 3000,
                    Strength = 8,
                    Intelligence = 35,
                    Dexterity = 12,
                    Vitality = 15,
                    Health = 180,
                    MaxHealth = 180,
                    PlayerId = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new Character 
                { 
                    Id = 3, 
                    Name = "Legolas", 
                    CharacterClass = "Ranger",
                    Level = 10,
                    Experience = 2000,
                    Strength = 15,
                    Intelligence = 12,
                    Dexterity = 30,
                    Vitality = 20,
                    Health = 220,
                    MaxHealth = 220,
                    PlayerId = 2,
                    CreatedAt = DateTime.UtcNow
                }
            );
            
            // Seed Items
            modelBuilder.Entity<Item>().HasData(
                new Item 
                { 
                    Id = 1, 
                    Name = "Excalibur", 
                    Description = "Legendary sword of immense power",
                    ItemType = "Weapon",
                    AttackBonus = 50,
                    DefenseBonus = 10,
                    Value = 5000,
                    Rarity = 10,
                    IsEquipped = true,
                    Quantity = 1,
                    PlayerId = 1,
                    AcquiredAt = DateTime.UtcNow
                },
                new Item 
                { 
                    Id = 2, 
                    Name = "Iron Armor", 
                    Description = "Sturdy armor for protection",
                    ItemType = "Armor",
                    AttackBonus = 0,
                    DefenseBonus = 30,
                    Value = 1500,
                    Rarity = 5,
                    IsEquipped = true,
                    Quantity = 1,
                    PlayerId = 1,
                    AcquiredAt = DateTime.UtcNow
                },
                new Item 
                { 
                    Id = 3, 
                    Name = "Health Potion", 
                    Description = "Restores 50 health points",
                    ItemType = "Consumable",
                    AttackBonus = 0,
                    DefenseBonus = 0,
                    Value = 50,
                    Rarity = 2,
                    IsEquipped = false,
                    Quantity = 10,
                    PlayerId = 1,
                    AcquiredAt = DateTime.UtcNow
                },
                new Item 
                { 
                    Id = 4, 
                    Name = "Elven Bow", 
                    Description = "Swift and accurate bow",
                    ItemType = "Weapon",
                    AttackBonus = 35,
                    DefenseBonus = 0,
                    Value = 2000,
                    Rarity = 7,
                    IsEquipped = true,
                    Quantity = 1,
                    PlayerId = 2,
                    AcquiredAt = DateTime.UtcNow
                }
            );
            
            // Seed Scores
            modelBuilder.Entity<Score>().HasData(
                new Score 
                { 
                    Id = 1, 
                    GameMode = "Arena", 
                    Points = 15000,
                    Kills = 50,
                    Deaths = 5,
                    TimePlayed = 3600,
                    DifficultyLevel = 5,
                    IsHighScore = true,
                    PlayerId = 1,
                    AchievedAt = DateTime.UtcNow
                },
                new Score 
                { 
                    Id = 2, 
                    GameMode = "Dungeon", 
                    Points = 8000,
                    Kills = 30,
                    Deaths = 3,
                    TimePlayed = 2400,
                    DifficultyLevel = 3,
                    IsHighScore = false,
                    PlayerId = 1,
                    AchievedAt = DateTime.UtcNow
                },
                new Score 
                { 
                    Id = 3, 
                    GameMode = "Arena", 
                    Points = 12000,
                    Kills = 40,
                    Deaths = 8,
                    TimePlayed = 3000,
                    DifficultyLevel = 4,
                    IsHighScore = true,
                    PlayerId = 2,
                    AchievedAt = DateTime.UtcNow
                }
            );
        }
    }
}

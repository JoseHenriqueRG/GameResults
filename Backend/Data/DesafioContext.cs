using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class DesafioContext : DbContext
    {
        
        public DesafioContext (DbContextOptions<DesafioContext> options)
            : base(options)
        {
        }
                
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameResult>().HasKey(gr => new { gr.PlayerId, gr.GameId, gr.Timestamp });

            modelBuilder.Entity<Game>().HasData(
                new Game
                {
                    GameId = 1,
                    Name = "Point game"
                }
            );
        }
        
        public DbSet<GameResult> GameResult { get; set; }
        
        public DbSet<Models.Player> Player { get; set; }

        public DbSet<Models.Game> Game { get; set; }
    }
}

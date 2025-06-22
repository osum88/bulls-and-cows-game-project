using Azure;
using bulls_and_cows_game_project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace bulls_and_cows_game_project.Data
{
    public class ApplicationDbContext : IdentityDbContext<Player>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<Guess> Guesses { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<GameSessionTag> GameSessionTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<GameSessionTag>()
                .HasKey(gst => new { gst.GameSessionId, gst.TagId });

            builder.Entity<GameSession>()
                .HasOne(gs => gs.Player)
                .WithMany(u => u.GameSessions)
                .HasForeignKey(gs => gs.PlayerId);
        }
    }
}

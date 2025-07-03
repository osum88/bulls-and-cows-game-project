using Azure;
using bulls_and_cows_game_project.Models;
using Humanizer;
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

            builder.Entity<GameSessionTag>()                            //konfigurace vztahu M:N mezi GameSession a Tag pomocí GameSessionTag
                .HasKey(gst => new { gst.GameSessionId, gst.TagId });   // slozeny primarni klic (GameSessionId a TagId)

            builder.Entity<GameSession>()                               // konfigurace vztahu 1:M mezi Player a GameSession                           
                .HasOne(gs => gs.Player)                                // GameSession ma jeden Player
                .WithMany(u => u.GameSessions)                          // a ma mnoho GameSessions.
                .HasForeignKey(gs => gs.PlayerId);                      // cizi klic v tabulce GameSession, ktery odkazuje na ID Player
        }
    }
}

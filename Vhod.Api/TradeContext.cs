using Microsoft.EntityFrameworkCore;
using Vhod.Models;

namespace Vhod.Api
{
    public class TradeContext : DbContext
    {
        public TradeContext(DbContextOptions<TradeContext> options) : base(options) { }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Note> notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("User");
                e.HasKey(u => u.User_ID);
            });

            modelBuilder.Entity<Role>(e =>
            {
                e.ToTable("Role");
                e.HasKey(r => r.Role_ID);
            });

            modelBuilder.Entity<Note>(e =>
            {
                e.ToTable("notes");
                e.HasKey(n => n.Id);
            });
        }
    }
}
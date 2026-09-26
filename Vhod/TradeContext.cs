using System.Data.Entity;
using Vhod.Models;

namespace Vhod
{
    public class TradeContext : DbContext
    {
        public TradeContext() : base("name=Trade2Connection")
        {
            Database.SetInitializer<TradeContext>(null);
        }

        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Note> Note { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.User_ID);
            modelBuilder.Entity<Role>().HasKey(r => r.Role_ID);
            modelBuilder.Entity<Note>().HasKey(n => n.Id);

            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Note>().ToTable("notes");

            base.OnModelCreating(modelBuilder);
        }
    }
}
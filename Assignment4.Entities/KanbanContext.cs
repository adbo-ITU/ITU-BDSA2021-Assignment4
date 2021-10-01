using Microsoft.EntityFrameworkCore;

namespace Assignment4.Entities
{
    public class KanbanContext : DbContext
    {
        public KanbanContext(DbContextOptions<KanbanContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder
                .Entity<Tag>()
                .HasIndex(e => e.Name)
                .IsUnique();
        }
    }
}

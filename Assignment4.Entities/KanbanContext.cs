using Assignment4.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Assignment4.Entities
{
    public class KanbanContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Tag> Tags { get; set; }

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

            modelBuilder
                .Entity<Task>()
                .Property(e => e.State)
                .HasConversion(new EnumToStringConverter<State>());
        }

        public void RemoveAllData()
        {
            using var transaction = Database.BeginTransaction();
            Users.RemoveRange(Users);
            Tasks.RemoveRange(Tasks);
            Tags.RemoveRange(Tags);
            SaveChanges();
            transaction.Commit();
        }
    }
}

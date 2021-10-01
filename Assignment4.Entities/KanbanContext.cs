using Microsoft.EntityFrameworkCore;

namespace Assignment4.Entities
{
    public class KanbanContext : DbContext
    {
        public KanbanContext(DbContextOptions<KanbanContext> options) : base(options) { }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder
        //         .Entity<Character>()
        //         .Property(e => e.Gender)
        //         .HasConversion(new EnumToStringConverter<Gender>());
        // }
    }
}

using System.IO;
using Assignment4.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Assignment4
{
    public class KanbanContextFactory : IDesignTimeDbContextFactory<KanbanContext>
    {
        public KanbanContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<Program>()
                .Build();

            var connectionString = configuration.GetConnectionString("bdsa-kanban");

            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>()
                .UseNpgsql(connectionString);

            return new KanbanContext(optionsBuilder.Options);
        }
    }
}

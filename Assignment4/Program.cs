using System;
using System.IO;
using Assignment4.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

[assembly: UserSecretsId("04ac5ef1-ddd2-4ac5-bcc7-e393baea2798")]
namespace Assignment4
{
    class Program
    {
        static void Main(string[] args)
        {
            // using var repository = new TaskRepository
            // {
            //     context = GetContext(),
            // };

            // KanbanContextFactory.Seed(repository);
            // repository.RemoveAllData();
        }

        public static KanbanContext GetContext()
        {
            var configuration = LoadConfiguration();
            var connectionString = configuration.GetConnectionString("bdsa-kanban");
            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseNpgsql(connectionString);
            return new KanbanContext(optionsBuilder.Options);
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<Program>();

            return builder.Build();
        }
    }
}

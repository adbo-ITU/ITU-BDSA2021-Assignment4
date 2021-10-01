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
            var configuration = LoadConfiguration();
            var connectionString = configuration.GetConnectionString("bdsa-kanban");

            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseNpgsql(connectionString);
            using var context = new KanbanContext(optionsBuilder.Options);

            // var hulk = new Character
            // {
            //     GivenName = "Bruce",
            //     Surname = "Banner",
            //     AlterEgo = "The Hulk",
            //     City = new City { Name = "New York" },
            //     Powers = new[] {
            //         new Power { Name = "Green"},
            //         new Power { Name = "Angry"}
            //     }
            // };

            // context.Characters.Add(hulk);
            // context.SaveChanges();

            // ComicsContextFactory.Seed(context);

            // var chars = from c in context.Characters
            //             where c.AlterEgo.Contains("a")
            //             select new
            //             {
            //                 c.GivenName,
            //                 c.Surname,
            //                 c.City.Name,
            //                 Powers = string.Join(", ", c.Powers.Select(p => p.Name))
            //             };
        }

        static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<Program>();

            return builder.Build();
        }
    }
}

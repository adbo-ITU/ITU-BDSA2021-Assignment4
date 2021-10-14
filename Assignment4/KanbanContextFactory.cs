using System.IO;
using Assignment4.Core;
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

        public static void Seed(TaskRepository repository)
        {
            // using var transaction = repository.context.Database.BeginTransaction();

            // repository.CreateUser("Philip", "phcr@itu.dk");
            // repository.CreateUser("Mads", "coha@itu.dk");
            // repository.CreateUser("Adrian", "adbo@itu.dk");

            // // var breakfast = new Tag { Name = "Breakfast", Id = 1 };
            // // var lunch = new Tag { Name = "Lunch", Id = 2 };
            // // var dinner = new Tag { Name = "Dinner", Id = 3 };

            // // var philip = new User { Id = 1, Email = "phcr@itu.dk", Name = "Philip" };
            // // var mads = new User { Id = 2, Email = "coha@itu.dk", Name = "Mads" };
            // // var adrian = new User { Id = 3, Email = "adbo@itu.dk", Name = "Adrian" };

            // // var doChili = new Task { Id = 1, Title = "Chili con carne", AssignedTo = philip, Description = "Make some crazy delicious chili con carne!!!", State = State.Active, Tags = new[] { dinner } };
            // // var doPizza = new Task { Id = 2, Title = "Pizza", AssignedTo = mads, Description = "It's a me, Mario!", State = State.New, Tags = new[] { breakfast, dinner } };
            // // var doHangoverSmoothie = new Task { Id = 3, Title = "Hangover smoothie", AssignedTo = mads, Description = "Juice, chokoladestykker, frosne bananer, yoghurt", State = State.Resolved, Tags = new[] { breakfast, dinner } };

            // // context.Tasks.AddRange(
            // //     doChili,
            // //     doPizza,
            // //     doHangoverSmoothie
            // // );

            // // context.SaveChanges();

            // transaction.Commit();
        }
    }
}

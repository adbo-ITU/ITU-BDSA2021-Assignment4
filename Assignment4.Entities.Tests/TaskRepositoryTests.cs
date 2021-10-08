using System;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Assignment4;
using System.IO;
using System.Linq;
using Assignment4.Core;
using System.Collections.Generic;

namespace Assignment4.Entities.Tests
{
    public class TaskRepositoryTests : IDisposable
    {
        // private readonly TaskRepository _repo;

        // public CityRepositoryTests()
        // {
        //     var connection = new SqliteConnection("Filename=:memory:");
        //     connection.Open();
        //     var builder = new DbContextOptionsBuilder<ComicsContext>();
        //     builder.UseSqlite(connection);
        //     var context = new ComicsContext(builder.Options);
        //     context.Database.EnsureCreated();
        //     context.Cities.Add(new City { Name = "Metropolis" });
        //     context.SaveChanges();

        //     _context = context;
        //     _repo = new CityRepository(_context);
        // }

        // public TaskRepositoryTests()
        // {
        //     var configuration = new ConfigurationBuilder()
        //         .SetBasePath(Directory.GetCurrentDirectory())
        //         .AddUserSecrets<TaskRepositoryTests>()
        //         .Build();
        //     var connectionString = configuration.GetConnectionString("bdsa-kanban");
        //     var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseNpgsql(connectionString);
        //     context = new KanbanContext(optionsBuilder.Options);
        //     repository = new TaskRepository
        //     {
        //         context = context
        //     };
        // }

        public void Dispose()
        {
            // repository.Dispose();
        }

        // [Fact]
        // public void Add_user_with_duplicate_email_errors()
        // {
        //     repository.RemoveAllData();
        //     repository.CreateUser("Rasmus", "coha@itu.dk");
        //     Assert.Throws<Microsoft.EntityFrameworkCore.DbUpdateException>(() => repository.CreateUser("Mads", "coha@itu.dk"));
        // }

        // [Fact]
        // public void All_returns_all()
        // {
        //     repository.RemoveAllData();

        //     var philipId = repository.CreateUser("Philip", "phcr@itu.dk");
        //     var madsId = repository.CreateUser("Mads", "coha@itu.dk");
        //     var adrianId = repository.CreateUser("Adrian", "adbo@itu.dk");

        //     repository.Create(new TaskDTO
        //     {
        //         AssignedToId = adrianId,
        //     });


        //     // var all = repository.All();
        //     // var orderedByName = all.OrderBy(task => task.Id).ToList();


        //     // var expectedTasks = new[] { doChili, doPizza, doHangoverSmoothie };

        //     // Assert.Equal(expectedTasks.Length, orderedById.Count);

        //     // for (int i = 0; i < expectedTasks.Length; i++)
        //     // {
        //     //     var a = expectedTasks[i];
        //     //     var b = orderedById[i];
        //     //     Assert.Equal(a.Id, b.Id);
        //     //     Assert.Equal(a.Title, b.Title);
        //     //     Assert.Equal(a.Description, b.Description);
        //     //     Assert.Equal(a.AssignedToId, b.AssignedToId);
        //     //     Assert.Equal(a.State, b.State);

        //     //     var aTags = a.Tags.ToList();
        //     //     var bTags = b.Tags.ToList();
        //     //     Assert.Equal(aTags, bTags);
        //     // }
        // }

        // // [Fact]
        // // public void Create_returns_id_and_adds_to_database()
        // // {
        // //     repository.RemoveAllData();
        // //     repository.Create(new TaskDTO
        // //     {
        // //         Title = "Test task",
        // //         Description = "Test description",
        // //         State = State.New,
        // //         Tags = (new[] { "test tag" }).ToList(),
        // //     });
        // // }
    }
}

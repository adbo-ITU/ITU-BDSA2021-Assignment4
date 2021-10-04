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
        KanbanContext context;
        TaskRepository repository;

        TaskDTO doChili, doPizza, doHangoverSmoothie;

        public TaskRepositoryTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<TaskRepositoryTests>()
                .Build();
            var connectionString = configuration.GetConnectionString("bdsa-kanban");
            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseNpgsql(connectionString);
            context = new KanbanContext(optionsBuilder.Options);
            repository = new TaskRepository
            {
                context = context
            };

            // Add default tasks
            var breakfast = new Tag { Name = "Breakfast", Id = 1 };
            var lunch = new Tag { Name = "Lunch", Id = 2 };
            var dinner = new Tag { Name = "Dinner", Id = 3 };

            var philip = new User { Id = 1, Email = "phcr@itu.dk", Name = "Philip" };
            var mads = new User { Id = 2, Email = "coha@itu.dk", Name = "Mads" };
            var adrian = new User { Id = 3, Email = "adbo@itu.dk", Name = "Adrian" };

            var doChiliTask = new Task { Id = 1, Title = "Chili con carne", AssignedTo = philip, Description = "Make some crazy delicious chili con carne!!!", State = State.Active, Tags = new[] { dinner }, AssignedToId = 1 };
            var doPizzaTask = new Task { Id = 2, Title = "Pizza", AssignedTo = mads, Description = "It's a me, Mario!", State = State.New, Tags = new[] { breakfast, dinner }, AssignedToId = 2 };
            var doHangoverSmoothieTask = new Task { Id = 3, Title = "Hangover smoothie", AssignedTo = mads, Description = "Juice, chokoladestykker, frosne bananer, yoghurt", State = State.Resolved, Tags = new[] { breakfast, dinner }, AssignedToId = 2 };

            doChili = doChiliTask.toTaskDTO();
            doPizza = doPizzaTask.toTaskDTO();
            doHangoverSmoothie = doHangoverSmoothieTask.toTaskDTO();
        }

        public void Dispose()
        {
            repository.Dispose();
        }

        [Fact]
        public void Add_user_with_duplicate_email_errors()
        {
            var rasmus = new User { Id = 53211, Email = "coha@itu.dk", Name = "Rasmus" };

            context.Users.Add(rasmus);

            Assert.Throws<Microsoft.EntityFrameworkCore.DbUpdateException>(() => context.SaveChanges());
        }

        [Fact]
        public void All_returns_all()
        {
            var all = repository.All();
            var orderedById = all.OrderBy(task => task.Id).ToList();

            var expectedTasks = new[] { doChili, doPizza, doHangoverSmoothie };

            Assert.True(orderedById.Count == expectedTasks.Length);

            for (int i = 0; i < expectedTasks.Length; i++)
            {
                Assert.True(TaskDTO.CustomEquals(expectedTasks[i], orderedById[i]));
            }
        }
    }
}

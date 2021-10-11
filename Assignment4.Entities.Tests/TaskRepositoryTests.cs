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
    [Collection("Sequential")]
    public class TaskRepositoryTests : IDisposable
    {
        private readonly KanbanContext _context;
        private readonly TaskRepository _repo;

        public TaskRepositoryTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<TaskRepositoryTests>()
                .Build();
            var connectionString = configuration.GetConnectionString("bdsa-kanban");
            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseNpgsql(connectionString);
            _context = new KanbanContext(optionsBuilder.Options);
            _context.RemoveAllData();
            _repo = new TaskRepository(_context);
        }

        public void Dispose()
        {
            _context.RemoveAllData();
            _context.Dispose();
        }

        private void AssertEqualTasks(TaskDTO a, TaskDTO b)
        {
            Assert.Equal(a.Id, b.Id);
            Assert.Equal(a.Title, b.Title);
            Assert.Equal(a.AssignedToName, b.AssignedToName);
            Assert.Equal(a.State, b.State);
            Assert.Equal(a.Tags.OrderBy(tag => tag), b.Tags.OrderBy(tag => tag));
        }

        [Fact]
        public void ReadAll_returns_all_tasks()
        {
            // Arrange
            var butcher = new User { Name = "Geralt", Email = "butcher@blaviken.km" };
            var newTasks = new Task[] {
                new Task { Title = "Hygge med Bamse", Description = "👀", State = State.New, Tags = new HashSet<Tag>(new[] { new Tag { Name = "hygge" } }), AssignedTo = butcher },
                new Task { Title = "Hygge med Kylling", Description = "chicken nuggets mm", State = State.Closed, Tags = new HashSet<Tag>(new[] { new Tag { Name = "dinner" } }) },
                new Task { Title = "Hygge med Ælling", Description = "i love anderilette", State = State.Resolved, AssignedTo = butcher },
            };
            _context.Tasks.AddRange(newTasks);
            _context.SaveChanges();

            // Act
            var all = _repo.ReadAll().OrderBy(task => task.Title);

            // Assert
            Assert.Collection(all,
                task => AssertEqualTasks(new TaskDTO(newTasks[2].Id, "Hygge med Ælling", "Geralt", new string[] { }, State.Resolved), task),
                task => AssertEqualTasks(new TaskDTO(newTasks[0].Id, "Hygge med Bamse", "Geralt", new[] { "hygge" }, State.New), task),
                task => AssertEqualTasks(new TaskDTO(newTasks[1].Id, "Hygge med Kylling", null, new[] { "dinner" }, State.Closed), task)
            );
        }
    }
}

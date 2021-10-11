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

        [Fact]
        public void ReadAllRemoved_returns_all_removed_tasks()
        {
            // Arrange
            var butcher = new User { Name = "Geralt", Email = "butcher@blaviken.km" };
            var newTasks = new Task[] {
                new Task { Title = "Hygge med Bamse", Description = "👀", State = State.Removed, Tags = new HashSet<Tag>(new[] { new Tag { Name = "hygge" } }), AssignedTo = butcher },
                new Task { Title = "Hygge med Kylling", Description = "chicken nuggets mm", State = State.Closed, Tags = new HashSet<Tag>(new[] { new Tag { Name = "dinner" } }) },
                new Task { Title = "Hygge med Ælling", Description = "i love anderilette", State = State.Removed, AssignedTo = butcher },
            };
            _context.Tasks.AddRange(newTasks);
            _context.SaveChanges();

            // Act
            var all = _repo.ReadAllRemoved().OrderBy(task => task.Title);

            // Assert
            Assert.Collection(all,
                task => AssertEqualTasks(new TaskDTO(newTasks[2].Id, "Hygge med Ælling", "Geralt", new string[] { }, State.Removed), task),
                task => AssertEqualTasks(new TaskDTO(newTasks[0].Id, "Hygge med Bamse", "Geralt", new[] { "hygge" }, State.Removed), task)
            );
        }

        [Fact]
        public void ReadAllByTag_returns_all_tasks_with_given_tag()
        {
            // Arrange
            var butcher = new User { Name = "Geralt", Email = "butcher@blaviken.km" };
            var hyggeTag = new Tag { Name = "hygge" };
            var newTasks = new Task[] {
                new Task { Title = "Hygge med Bamse", Description = "👀", State = State.Removed, Tags = new HashSet<Tag>(new[] { hyggeTag }), AssignedTo = butcher },
                new Task { Title = "Hygge med Kylling", Description = "chicken nuggets mm", State = State.Closed, Tags = new HashSet<Tag>(new[] { new Tag { Name = "dinner" }, hyggeTag }) },
                new Task { Title = "Hygge med Ælling", Description = "i love anderilette", State = State.Removed, AssignedTo = butcher },
            };
            _context.Tasks.AddRange(newTasks);
            _context.SaveChanges();

            // Act
            var all = _repo.ReadAllByTag("hygge").OrderBy(task => task.Title);

            // Assert
            Assert.Collection(all,
                task => AssertEqualTasks(new TaskDTO(newTasks[0].Id, "Hygge med Bamse", "Geralt", new[] { "hygge" }, State.Removed), task),
                task => AssertEqualTasks(new TaskDTO(newTasks[1].Id, "Hygge med Kylling", null, new[] { "hygge", "dinner" }, State.Closed), task)
            );
        }

        [Fact]
        public void ReadAllByUser_returns_all_tasks_assigned_to_given_user()
        {
            // Arrange
            var butcher = new User { Name = "Geralt", Email = "butcher@blaviken.km" };
            var hyggeTag = new Tag { Name = "hygge" };
            var newTasks = new Task[] {
                new Task { Title = "Hygge med Bamse", Description = "👀", State = State.Removed, Tags = new HashSet<Tag>(new[] { hyggeTag }), AssignedTo = butcher },
                new Task { Title = "Hygge med Kylling", Description = "chicken nuggets mm", State = State.Closed, Tags = new HashSet<Tag>(new[] { new Tag { Name = "dinner" }, hyggeTag }) },
                new Task { Title = "Hygge med Ælling", Description = "i love anderilette", State = State.Active, AssignedTo = butcher },
            };
            _context.Tasks.AddRange(newTasks);
            _context.SaveChanges();

            // Act
            var all = _repo.ReadAllByUser(butcher.Id).OrderBy(task => task.Title);

            // Assert
            Assert.Collection(all,
                task => AssertEqualTasks(new TaskDTO(newTasks[2].Id, "Hygge med Ælling", "Geralt", new string[] { }, State.Active), task),
                task => AssertEqualTasks(new TaskDTO(newTasks[0].Id, "Hygge med Bamse", "Geralt", new[] { "hygge" }, State.Removed), task)
            );
        }

        [Fact]
        public void ReadAllByState_returns_all_tasks_with_state()
        {
            // Arrange
            var butcher = new User { Name = "Geralt", Email = "butcher@blaviken.km" };
            var hyggeTag = new Tag { Name = "hygge" };
            var newTasks = new Task[] {
                new Task { Title = "Hygge med Bamse", Description = "👀", State = State.Active, Tags = new HashSet<Tag>(new[] { hyggeTag }), AssignedTo = butcher },
                new Task { Title = "Hygge med Kylling", Description = "chicken nuggets mm", State = State.Closed, Tags = new HashSet<Tag>(new[] { new Tag { Name = "dinner" }, hyggeTag }) },
                new Task { Title = "Hygge med Ælling", Description = "i love anderilette", State = State.Active, AssignedTo = butcher },
            };
            _context.Tasks.AddRange(newTasks);
            _context.SaveChanges();

            // Act
            var all = _repo.ReadAllByState(State.Active).OrderBy(task => task.Title);

            // Assert
            Assert.Collection(all,
                task => AssertEqualTasks(new TaskDTO(newTasks[2].Id, "Hygge med Ælling", "Geralt", new string[] { }, State.Active), task),
                task => AssertEqualTasks(new TaskDTO(newTasks[0].Id, "Hygge med Bamse", "Geralt", new[] { "hygge" }, State.Active), task)
            );
        }

        [Fact]
        public void Read_returns_task_with_id()
        {
            // Arrange
            var newTask = new Task { Title = "Hygge med Bamse", Description = "👀", State = State.Active, Tags = new HashSet<Tag>(new[] { new Tag { Name = "hygge" } }), AssignedTo = new User { Name = "Geralt", Email = "butcher@blaviken.km" }, Created = DateTime.SpecifyKind(new DateTime(2008, 3, 1, 7, 0, 0), DateTimeKind.Utc), StateUpdated = DateTime.SpecifyKind(new DateTime(2009, 3, 1, 7, 0, 0), DateTimeKind.Utc) };
            _context.Tasks.Add(newTask);
            _context.SaveChanges();

            // Act
            var task = _repo.Read(newTask.Id);

            // Assert
            Assert.Equal(newTask.Id, task.Id);
            Assert.Equal(newTask.Title, task.Title);
            Assert.Equal(newTask.Description, task.Description);
            Assert.Equal(newTask.Created, task.Created, precision: TimeSpan.FromSeconds(5));
            Assert.Equal(newTask.AssignedTo.Name, task.AssignedToName);
            Assert.Equal(new[] { "hygge" }, task.Tags.ToList());
            Assert.Equal(newTask.State, task.State);
            Assert.Equal(newTask.StateUpdated, task.StateUpdated, precision: TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Create_returns_response_and_id_and_adds_to_database()
        {
            // Arrange
            // Act
            var (response, taskId) = _repo.Create(new TaskCreateDTO
            {
                Title = "Hygge med Bamse",
                AssignedToId = null,
                Description = "bum bum bummelum",
                Tags = new HashSet<string>(new[] { "hygge" }),
            });

            var task = _context.Tasks.Find(taskId);

            // Assert
            Assert.Equal(Response.Created, response);
            Assert.Equal("Hygge med Bamse", task.Title);
            Assert.Null(task.AssignedToId);
            Assert.Equal("bum bum bummelum", task.Description);
            Assert.Equal(new[] { "hygge" }, _repo.ReadTaskTags(task));
            Assert.Equal(State.New, task.State);
            Assert.Equal(DateTime.UtcNow, task.Created, precision: TimeSpan.FromSeconds(5));
            Assert.Equal(DateTime.UtcNow, task.StateUpdated, precision: TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Update_returns_response_and_updates_task()
        {
            // Arrange
            var newTask = new Task { Title = "Hygge med Bamse", Description = "👀", State = State.Active, Tags = new HashSet<Tag>(new[] { new Tag { Name = "hygge" } }), AssignedTo = new User { Name = "Geralt", Email = "butcher@blaviken.km" }, Created = DateTime.SpecifyKind(new DateTime(2008, 3, 1, 7, 0, 0), DateTimeKind.Utc), StateUpdated = DateTime.SpecifyKind(new DateTime(2009, 3, 1, 7, 0, 0), DateTimeKind.Utc) };
            _context.Tasks.Add(newTask);
            _context.SaveChanges();

            // Act
            var response = _repo.Update(new TaskUpdateDTO
            {
                Id = newTask.Id,
                Title = "haha",
                Description = "lulz",
                State = State.Resolved,
                AssignedToId = null,
                Tags = new HashSet<string>(new[] { "idk" }),
            });
            var task = _context.Tasks.Find(newTask.Id);

            // Assert
            Assert.Equal(Response.Updated, response);
            Assert.Equal(newTask.Id, task.Id);
            Assert.Equal("haha", task.Title);
            Assert.Equal("lulz", task.Description);
            Assert.Equal(new DateTime(2008, 3, 1, 7, 0, 0), task.Created);
            Assert.Null(task.AssignedToId);
            // FIXME: UNCOMMENT WHEN FIXED
            // Assert.Equal(new[] { "idk" }, _repo.ReadTaskTags(task));
            Assert.Equal(State.Resolved, task.State);
            Assert.Equal(DateTime.UtcNow, task.StateUpdated, precision: TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Delete_active_sets_task_to_removed()
        {
            // Arrange
            var newTask = new Task { Title = "Hygge med Bamse", Description = "👀", State = State.Active, Tags = new HashSet<Tag>(new[] { new Tag { Name = "hygge" } }), AssignedTo = new User { Name = "Geralt", Email = "butcher@blaviken.km" }, Created = DateTime.SpecifyKind(new DateTime(2008, 3, 1, 7, 0, 0), DateTimeKind.Utc), StateUpdated = DateTime.SpecifyKind(new DateTime(2009, 3, 1, 7, 0, 0), DateTimeKind.Utc) };
            _context.Tasks.Add(newTask);
            _context.SaveChanges();

            // Act
            var response = _repo.Delete(newTask.Id);
            var task = _context.Tasks.Find(newTask.Id);

            // Assert
            Assert.Equal(Response.Updated, response);
            Assert.NotNull(task);
            Assert.Equal(State.Removed, task.State);
            Assert.Equal(DateTime.UtcNow, task.StateUpdated, precision: TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Delete_closed_returns_conflict()
        {
            // Arrange
            var newTask = new Task { Title = "Hygge med Bamse", Description = "👀", State = State.Closed, Tags = new HashSet<Tag>(new[] { new Tag { Name = "hygge" } }), AssignedTo = new User { Name = "Geralt", Email = "butcher@blaviken.km" }, Created = DateTime.SpecifyKind(new DateTime(2008, 3, 1, 7, 0, 0), DateTimeKind.Utc), StateUpdated = DateTime.SpecifyKind(new DateTime(2009, 3, 1, 7, 0, 0), DateTimeKind.Utc) };
            _context.Tasks.Add(newTask);
            _context.SaveChanges();

            // Act
            var response = _repo.Delete(newTask.Id);
            var task = _context.Tasks.Find(newTask.Id);

            // Assert
            Assert.Equal(Response.Conflict, response);
            Assert.NotNull(task);
        }

        [Fact]
        public void Delete_new_removes_from_database()
        {
            // Arrange
            var newTask = new Task { Title = "Hygge med Bamse", Description = "👀", State = State.New, Tags = new HashSet<Tag>(new[] { new Tag { Name = "hygge" } }), AssignedTo = new User { Name = "Geralt", Email = "butcher@blaviken.km" }, Created = DateTime.SpecifyKind(new DateTime(2008, 3, 1, 7, 0, 0), DateTimeKind.Utc), StateUpdated = DateTime.SpecifyKind(new DateTime(2009, 3, 1, 7, 0, 0), DateTimeKind.Utc) };
            _context.Tasks.Add(newTask);
            _context.SaveChanges();

            // Act
            var response = _repo.Delete(newTask.Id);
            var task = _context.Tasks.Find(newTask.Id);

            // Assert
            Assert.Equal(Response.Deleted, response);
            Assert.Null(task);
        }
    }
}

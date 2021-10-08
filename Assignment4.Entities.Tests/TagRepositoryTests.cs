using System;
using Assignment4.Core;
using Assignment4.Entities;
using Assignment4;
using Microsoft.Data.Sqlite;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;

namespace Assignment4.Entities.Tests
{
    public class TagRepositoryTests : IDisposable
    {
        private readonly KanbanContext _context;
        private readonly TagRepository _repo;

        public TagRepositoryTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<TaskRepositoryTests>()
                .Build();
            var connectionString = configuration.GetConnectionString("bdsa-kanban");
            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseNpgsql(connectionString);
            _context = new KanbanContext(optionsBuilder.Options);
            _context.RemoveAllData();
            _repo = new TagRepository(_context);
        }

        public void Dispose()
        {
            _context.RemoveAllData();
            _context.Dispose();
        }

        public int CreateAssignedTag()
        {
            var newTask = new Task
            {
                Title = "Hygge med Bamse",
                Description = "👀",
                State = State.New,
                Tags = new HashSet<Tag>(new[] { new Tag { Name = "hygge" } }),
            };
            _context.Tasks.Add(newTask);
            _context.SaveChanges();
            return newTask.Tags.First().Id;
        }

        [Fact]
        public void Delete_given_assigned_removes_tag_on_force()
        {
            // Arrange
            var tagId = CreateAssignedTag();

            // Act
            var response = _repo.Delete(tagId, force: true);

            // Assert
            Assert.Equal(Response.Deleted, response);
        }

        [Fact]
        public void Delete_given_assigned_without_force_returns_conflict()
        {
            // Arrange
            var tagId = CreateAssignedTag();

            // Act
            var response = _repo.Delete(tagId);

            // Assert
            Assert.Equal(Response.Conflict, response);
        }

        [Fact]
        public void Delete_given_id_not_existing_returns_NotFound()
        {
            // Arrange
            // Act
            var response = _repo.Delete(123);

            // Assert
            Assert.Equal(Response.NotFound, response);
        }

        [Fact]
        public void Create_already_existing_returns_Conflict()
        {
            // Arrange
            _context.Tags.Add(new Tag { Name = "Hygge med Bjørn" });
            _context.SaveChanges();

            // Act
            var (response, tagId) = _repo.Create(new TagCreateDTO { Name = "Hygge med Bjørn" });

            // Assert
            Assert.Equal(Response.Conflict, response);
        }

        [Fact]
        public void Create_given_new_name_returns_created()
        {
            // Arrange
            // Act
            var (response, tagId) = _repo.Create(new TagCreateDTO { Name = "Hygge med Bjørn" });

            // Assert
            Assert.Equal(Response.Created, response);
        }

        [Fact]
        public void Read_given_existing_id_returns_same_tag()
        {
            // Arrange
            var newTag = new Tag { Name = "Ooh la la" };
            _context.Tags.Add(newTag);
            _context.SaveChanges();
            var expectedTag = new TagDTO(newTag.Id, "Ooh la la");

            // Act
            var tag = _repo.Read(newTag.Id);

            // Assert
            Assert.Equal(expectedTag, tag);
        }

        [Fact]
        public void Read_given_non_existing_id_returns_null()
        {
            // Arrange
            // Act
            var tag = _repo.Read(123);

            // Assert
            Assert.Null(tag);
        }
    }
}

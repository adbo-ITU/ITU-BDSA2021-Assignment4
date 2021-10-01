using System;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Assignment4;
using System.IO;

namespace Assignment4.Entities.Tests
{
    public class TaskRepositoryTests : IDisposable
    {
        KanbanContext context;

        public TaskRepositoryTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<TaskRepositoryTests>()
                .Build();
            var connectionString = configuration.GetConnectionString("bdsa-kanban");
            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseNpgsql(connectionString);
            context = new KanbanContext(optionsBuilder.Options);
        }

        public void Dispose()
        {
            context.Dispose();
        }

        [Fact]
        public void Add_user_with_duplicate_email_errors()
        {
            var rasmus = new User { Id = 53211, Email = "coha@itu.dk", Name = "Rasmus" };

            context.Users.Add(rasmus);

            Assert.Throws<Microsoft.EntityFrameworkCore.DbUpdateException>(() => context.SaveChanges());
        }
    }
}

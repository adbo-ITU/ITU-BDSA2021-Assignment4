using System;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Assignment4;
using System.IO;
using System.Linq;
using Assignment4.Core;
using System.Collections.Generic;
using Assignment4.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Assignment4.Entities.Tests
{
    [Collection("Sequential")]
    public class UserRepositoryTests
    {
        private readonly KanbanContext _context;
        private readonly UserRepository _repo;
        public UserRepositoryTests()
        {
            
         var configuration = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddUserSecrets<TaskRepositoryTests>()
         .Build();
         
         var connectionString = configuration.GetConnectionString("bdsa-kanban");
         var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseNpgsql(connectionString);
            _context = new KanbanContext(optionsBuilder.Options);
            _context.RemoveAllData();
            _repo = new UserRepository(_context);


  
        }

        public void Dispose()
        {
            _context.RemoveAllData();
            _context.Dispose();
        }
        public int CreateUser()
        {
            var newUser = new User { Name = "ole", Email = "ole@itu.dk" };
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return newUser.Id;
        }

        [Fact]
        public void Delete_a_user_using_force_returns_Deleted()
        {
            //Given
            var id = CreateUser();
            
       
            //When
            var response = _repo.Delete(id);
            //Then

            Assert.Equal(Response.Conflict,response);
        }

        [Fact]
        public void Delete_a_user_not_using_force_returns_Conflict()
        {
             //Given
            var id = CreateUser();
            
       
            //When
            var response = _repo.Delete(id,force: true);
            //Then

            Assert.Equal(Response.Deleted,response);
        }
        [Fact]
        public void Delete_a_user_not_exsting_returns_NotFound()
        {
            var id = CreateUser()+1;
            
       
            //When
            var response = _repo.Delete(id);
            //Then

            Assert.Equal(Response.NotFound,response);
        }

         [Fact]
        public void Create_a_user()
        {

            //Act
           var (respons,userId) = _repo.Create(new UserCreateDTO{Name = "ole", Email = "ole@itu.dk"});
            // Assert
            Assert.Equal(Response.Created,respons);

        }
         public void Create_a_allready_exsisting_user()
        {
               // Arrange
            var newUser = new User { Name = "ole", Email = "ole@itu.dk" };
             _context.Users.Add(newUser);
            _context.SaveChanges();

            //Act
           var (respons,userId) = _repo.Create(new UserCreateDTO{Name = "olga", Email = "ole@itu.dk"});
            // Assert
            Assert.Equal(Response.Conflict,respons);


        }
    
    }
}

using System.Collections.Generic;
using System.IO;
using Assignment4.Core;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Assignment4.Entities
{
    public class UserRepository : IUserRepository
    {
        private KanbanContext _context;

        public UserRepository(KanbanContext context)
        {
            _context = context;
        }


        public (Response Response, int UserId) Create(UserCreateDTO user)
        {
            var existingUserWithEmail = from u in _context.Users
                                        where u.Email == user.Email
                                        select u.Id;
            if (existingUserWithEmail.Any()){
                return ( Response.Conflict,existingUserWithEmail.First());
            }

            var newUser = new User { Name = user.Name, Email = user.Email };
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return( Response.Created,newUser.Id);

        }

        public Response Delete(int UserId, bool force = false)
        {
            var user = _context.Users.Find(UserId);
            if(user == null) return Response.NotFound;
            if(!force) return Response.Conflict;
            _context.Users.Remove(user);
            return Response.Deleted;
        }

        public UserDTO Read(int UserId)
        {
           var user = _context.Users.Find(UserId);
           return user != null ? new UserDTO(user.Id,user.Name,user.Email): null;
        }

        public IReadOnlyCollection<UserDTO> ReadAll()
        {
            return _context.Users.Select(user => new UserDTO(user.Id,user.Name,user.Email)).ToList().AsReadOnly();

        }
        public Response Update(UserUpdateDTO user)
        {
            throw new System.NotImplementedException();
        }
    }
}

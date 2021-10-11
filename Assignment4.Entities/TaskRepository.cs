using System;
using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        private KanbanContext _context;

        public TaskRepository(KanbanContext context)
        {
            _context = context;
        }

        public void Dispose() => _context.Dispose();

        (Response Response, int TaskId) ITaskRepository.Create(TaskCreateDTO task)
        {
            throw new NotImplementedException();
            // var newTask = new Task
            // {
            //     Title = task.Title,
            //     AssignedTo = _context.Users.SingleOrDefault(u => u.Id == task.AssignedToId),
            //     Description = task.Description,
            //     State = task.State,
            //     Tags = task.Tags.Select(tagName =>
            //     {
            //         var tagsWithName = from t in _context.Tags
            //                            where t.Name == tagName
            //                            select t;

            //         return tagsWithName.Any() ? tagsWithName.First() : new Tag { Name = tagName };
            //     }).ToList(),
            // };

            // _context.Tasks.Add(newTask);
            // _context.SaveChanges();

            // return newTask.Id;
        }

        public IReadOnlyCollection<TaskDTO> ReadAll()
        {
            var tasks = _context.Tasks.ToList();
            var taskDtos = new List<TaskDTO>();
            foreach (var task in tasks)
            {
                var tags = _context
                    .Entry(task)
                    .Collection(t => t.Tags)
                    .Query()
                    .OrderBy(t => t.Name)
                    .Select(t => t.Name)
                    .ToList();

                taskDtos.Add(new TaskDTO(task.Id, task.Title, task.AssignedTo?.Name, tags.AsReadOnly(), task.State));
            }

            return taskDtos.AsReadOnly();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByState(State state)
        {
            throw new NotImplementedException();
        }

        public TaskDetailsDTO Read(int taskId)
        {
            throw new NotImplementedException();
        }

        public Response Update(TaskUpdateDTO task)
        {
            throw new NotImplementedException();
        }

        public Response Delete(int taskId)
        {
            throw new NotImplementedException();
        }

        public int CreateUser(string name, string email)
        {
            var newUser = new User
            {
                Name = name,
                Email = email,
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return newUser.Id;
        }
    }
}

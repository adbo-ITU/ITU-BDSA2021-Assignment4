using System;
using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;
using System.Collections.ObjectModel;

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

        private IReadOnlyCollection<string> ReadTaskTags(Task task) => _context
            .Entry(task)
            .Collection(t => t.Tags)
            .Query()
            .Select(t => t.Name)
            .ToList();

        private TaskDTO TaskDTOFromTask(Task task) => new TaskDTO(
            task.Id, task.Title, task.AssignedTo?.Name, ReadTaskTags(task), task.State);

        public IReadOnlyCollection<TaskDTO> ReadAll()
        {
            var tasks = from task in _context.Tasks
                        select task;
            var taskDtos = from task in tasks.ToList()
                           select TaskDTOFromTask(task);

            return taskDtos.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
        {
            var removed = from task in _context.Tasks
                          where task.State == State.Removed
                          select task;
            var taskDtos = from task in removed.ToList()
                           select TaskDTOFromTask(task);

            return taskDtos.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag)
        {
            var foundTag = _context.Tags.FirstOrDefault(t => t.Name == tag);

            if (foundTag == null)
                return null;

            var tasks = _context
                .Entry(foundTag)
                .Collection(t => t.tasks)
                .Query();
            var taskDtos = from task in tasks.ToList()
                           select TaskDTOFromTask(task);

            return taskDtos.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
        {
            var foundUser = _context.Users.Find(userId);

            if (foundUser == null)
                return null;

            var tasks = _context
                .Entry(foundUser)
                .Collection(t => t.tasks)
                .Query();
            var taskDtos = from task in tasks.ToList()
                           select TaskDTOFromTask(task);

            return taskDtos.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<TaskDTO> ReadAllByState(State state)
        {
            var withState = from task in _context.Tasks
                            where task.State == state
                            select task;
            var taskDtos = from task in withState.ToList()
                           select TaskDTOFromTask(task);

            return taskDtos.ToList().AsReadOnly();
        }

        public TaskDetailsDTO Read(int taskId)
        {
            var task = _context.Tasks.Find(taskId);

            if (task == null)
                return null;

            return new TaskDetailsDTO(task.Id, task.Title, task.Description, task.Created, task.AssignedTo?.Name, ReadTaskTags(task), task.State, task.StateUpdated);
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

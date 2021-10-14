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

        public IReadOnlyCollection<string> ReadTaskTags(Task task) => _context
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

        public (Response Response, int TaskId) Create(TaskCreateDTO task)
        {
            var now = DateTime.UtcNow;
            var user = _context.Users.SingleOrDefault(u => u.Id == task.AssignedToId);

            if (task.AssignedToId != null && user == null)
                return (Response.BadRequest, -1);

            var newTask = new Task
            {
                Title = task.Title,
                AssignedTo = user,
                Description = task.Description,
                State = State.New,
                Tags = task.Tags.Select(tagName =>
                {
                    var tagsWithName = from t in _context.Tags
                                       where t.Name == tagName
                                       select t;

                    return tagsWithName.Any() ? tagsWithName.First() : new Tag { Name = tagName };
                }).ToList(),
                Created = now,
                StateUpdated = now,
            };

            _context.Tasks.Add(newTask);
            _context.SaveChanges();

            return (Response.Created, newTask.Id);
        }

        public Response Update(TaskUpdateDTO task)
        {
            var upTask = _context.Tasks.Find(task.Id);
            var assignedToUser = _context.Users.SingleOrDefault(u => u.Id == task.AssignedToId);

            if (upTask == null)
                return Response.NotFound;
            if (task.AssignedToId != null && assignedToUser == null)
                return Response.BadRequest;

            upTask.Title = task.Title;
            upTask.Description = task.Description;
            upTask.AssignedTo = assignedToUser;
            upTask.AssignedToId = task.AssignedToId;

            if (upTask.State != task.State)
            {
                upTask.State = task.State;
                upTask.StateUpdated = DateTime.UtcNow;
            }

            upTask.Tags = task.Tags.Select(tagName =>
                {
                    var tagsWithName = from t in _context.Tags
                                       where t.Name == tagName
                                       select t;

                    return tagsWithName.Any() ? tagsWithName.First() : new Tag { Name = tagName };
                }).ToList();

            _context.SaveChanges();

            return Response.Updated;
        }

        public Response Delete(int taskId)
        {
            var task = _context.Tasks.Find(taskId);

            if (task == null)
                return Response.NotFound;

            switch (task.State)
            {
                case State.Active:
                    task.State = State.Removed;
                    task.StateUpdated = DateTime.UtcNow;
                    _context.SaveChanges();
                    return Response.Updated;
                case State.New:
                    _context.Tasks.Remove(task);
                    _context.SaveChanges();
                    return Response.Deleted;
                default:
                    return Response.Conflict;
            }
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

using System;
using System.Collections.Generic;
using Assignment4.Core;
using System.Linq;
using System.Collections.ObjectModel;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        public KanbanContext context { get; init; }

        public IReadOnlyCollection<TaskDTO> All()
        {
            var tasks = context.Tasks.ToList();
            var taskDtos = new List<TaskDTO>();
            foreach (var task in tasks)
            {
                var tags = context
                    .Entry(task)
                    .Collection(t => t.Tags)
                    .Query()
                    .OrderBy(t => t.Name)
                    .Select(t => t.Name)
                    .ToList();
                var taskDto = task.toTaskDTO(tags);
                taskDtos.Add(taskDto);
            }

            return new ReadOnlyCollection<TaskDTO>(taskDtos);
        }

        public int Create(TaskDTO task)
        {
            throw new NotImplementedException();
        }

        public void Delete(int taskId)
        {
            throw new NotImplementedException();
        }

        public void Dispose() => context.Dispose();

        public TaskDetailsDTO FindById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(TaskDTO task)
        {
            throw new NotImplementedException();
        }
    }
}

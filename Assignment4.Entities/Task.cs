using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Assignment4.Core;
using System.Linq;
using System.Collections.ObjectModel;

namespace Assignment4.Entities
{
    public class Task
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public User AssignedTo { get; set; }

        public int AssignedToId { get; set; }

        public string Description { get; set; }

        [Required]
        public State State { get; set; }

        public ICollection<Tag> Tags { get; set; }

        public TaskDTO toTaskDTO() => new TaskDTO
        {
            AssignedToId = AssignedToId,
            Description = Description,
            Id = Id,
            State = State,
            Tags = new ReadOnlyCollection<string>(Tags.Select(t => t.Name).ToList()),
            Title = Title,
        };

        public override string ToString()
        {
            return $"Title: {Title}"
                + $"\nDescription: {Description}"
            + $"\nAssignedToId: {AssignedToId}"
            + $"\nState: {State}"
            + $"\nId: {Id}";
        }
    }
}

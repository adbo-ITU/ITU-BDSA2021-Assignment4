using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment4.Core
{
    public record TaskDTO
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public int? AssignedToId { get; init; }
        public IReadOnlyCollection<string> Tags { get; init; }
        public State State { get; init; }

        public static bool CustomEquals(TaskDTO a, TaskDTO b)
        {
            if (a.Tags.Count != b.Tags.Count) return false;

            if (!a.Tags.ToList().SequenceEqual(b.Tags.ToList()))
            {
                return false;
            }

            if (a.Id != b.Id || a.Title != b.Title || a.Description != b.Description || a.AssignedToId != b.AssignedToId || a.State != b.State)
            {
                return false;
            }

            return true;
        }
    }
}
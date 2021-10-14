using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Assignment4.Core;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Assignment4.Entities
{
    public class Task
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public User AssignedTo { get; set; }

        public int? AssignedToId { get; set; }

        public string Description { get; set; }

        [Required]
        public State State { get; set; }

        public DateTime Created { get; set; }

        public DateTime StateUpdated { get; set; }

        public ICollection<Tag> Tags { get; set; }
    };
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public enum TaskPriority
    {
        Normal,
        Important
    }

    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        public required string Title { get; set; }

        public required string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsCompleted { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        public required string ExecutorFirstName { get; set; }
        public required string ExecutorLastName { get; set; }
        public required string ExecutorEmail { get; set; }
        public string? ExecutorPhoto { get; set; }

        public TaskPriority Priority { get; set; }
    }
}

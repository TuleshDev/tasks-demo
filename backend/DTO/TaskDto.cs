namespace Backend.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsCompleted { get; set; }

        public int OwnerId { get; set; }

        public required string ExecutorFirstName { get; set; }
        public required string ExecutorLastName { get; set; }
        public required string ExecutorEmail { get; set; }
        public string? ExecutorPhoto { get; set; }

        public required string Priority { get; set; }
    }
}

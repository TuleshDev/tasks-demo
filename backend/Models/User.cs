using System.Collections.Generic;

namespace Backend.Models
{
    public enum UserRole
    {
        Admin,
        User
    }

    public class User
    {
        public int Id { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public UserRole Role { get; set; }

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}

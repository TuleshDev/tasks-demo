using Backend.Models;
using Backend.DTOs;

namespace Backend.Mapping
{
    public static class DtoMapper
    {
        public static UserDto ToDto(User user) => new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role.ToString().ToLower()
        };

        public static TaskDto ToDto(TaskItem task) => new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            OwnerId = task.OwnerId,
            ExecutorFirstName = task.ExecutorFirstName,
            ExecutorLastName = task.ExecutorLastName,
            ExecutorEmail = task.ExecutorEmail,
            ExecutorPhoto = task.ExecutorPhoto,
            Priority = task.Priority == TaskPriority.Normal ? "Обычный" : "Важно"
        };
    }
}

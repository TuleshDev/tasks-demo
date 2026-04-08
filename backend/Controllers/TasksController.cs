using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Mapping;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /api/tasks
        [HttpGet]
        public async Task<ActionResult<object>> GetTasks(
            [FromQuery] string status = "all",
            [FromQuery] string search = "",
            [FromQuery] string sort = "",
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var query = _context.Tasks.AsQueryable();

            if (status == "active")
                query = query.Where(t => !t.IsCompleted);
            else if (status == "completed")
                query = query.Where(t => t.IsCompleted);

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(s) ||
                    t.Description.ToLower().Contains(s));
            }

            query = sort switch
            {
                "createdAtAsc" => query.OrderBy(t => t.CreatedAt),
                "createdAtDesc" => query.OrderByDescending(t => t.CreatedAt),
                "dueDateAsc" => query.OrderBy(t => t.DueDate),
                "dueDateDesc" => query.OrderByDescending(t => t.DueDate),
                "title" => query.OrderBy(t => t.Title),
                "priority" => query.OrderBy(t => t.Priority),
                _ => query
            };

            var total = await query.CountAsync();
            var tasks = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            var dtoList = tasks.Select(t =>
            {
                var dto = DtoMapper.ToDto(t);
                dto.ExecutorPhoto ??= "/uploads/default.png";
                return dto;
            });

            return Ok(new { data = dtoList, total, page, limit });
        }

        // POST: /api/tasks
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);

            var currentUser = await _context.Users.FindAsync(userId);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var entry = await _context.Tasks.AddAsync(new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                DueDate = dto.DueDate.ToUniversalTime(),
                Priority = dto.Priority == "Важно" ? TaskPriority.Important : TaskPriority.Normal,
                IsCompleted = false,
                OwnerId = currentUser.Id,
                ExecutorFirstName = dto.ExecutorFirstName,
                ExecutorLastName = dto.ExecutorLastName,
                ExecutorEmail = dto.ExecutorEmail,
                ExecutorPhoto = dto.ExecutorPhoto ?? "/uploads/default.png"
            });

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTasks), new { id = entry.Entity.Id }, DtoMapper.ToDto(entry.Entity));
        }

        // PUT: /api/tasks/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<TaskDto>> UpdateTask(int id, [FromBody] TaskDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);

            var currentUser = await _context.Users.FindAsync(userId);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound(new { error = "Задача не найдена" });

            if (currentUser.Role != UserRole.Admin && task.OwnerId != currentUser.Id)
            {
                return Forbid();
            }

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.CreatedAt = dto.CreatedAt.ToUniversalTime();
            task.DueDate = dto.DueDate.ToUniversalTime();
            task.Priority = dto.Priority == "Важно" ? TaskPriority.Important : TaskPriority.Normal;
            task.IsCompleted = dto.IsCompleted;
            task.ExecutorFirstName = dto.ExecutorFirstName;
            task.ExecutorLastName = dto.ExecutorLastName;
            task.ExecutorEmail = dto.ExecutorEmail;
            task.ExecutorPhoto = dto.ExecutorPhoto ?? "/uploads/default.png";

            await _context.SaveChangesAsync();

            return Ok(DtoMapper.ToDto(task));
        }

        // DELETE: /api/tasks/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteTask(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);

            var currentUser = await _context.Users.FindAsync(userId);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound(new { error = "Задача не найдена" });

            if (currentUser.Role != UserRole.Admin && task.OwnerId != currentUser.Id)
            {
                return Forbid();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }
}

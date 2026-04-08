using Backend.Data;
using Backend.Models;

namespace Backend.Data
{
    public static class DbInitializer
    {
        private static DateTime ParseUtc(string date)
        {
            return DateTime.SpecifyKind(DateTime.Parse(date), DateTimeKind.Utc);
        }

        public static void Seed(AppDbContext context, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment()) return;

            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Email = "admin@test.com",
                        Password = "123456",
                        Role = UserRole.Admin
                    },
                    new User
                    {
                        Email = "user@test.com",
                        Password = "123456",
                        Role = UserRole.User
                    }
                );
                context.SaveChanges();
            }

            if (!context.Tasks.Any())
            {
                context.Tasks.AddRange(
                    new TaskItem { Title = "Сделать логин", Description = "Форма email/password", CreatedAt = ParseUtc("2026-02-12"), DueDate = ParseUtc("2026-02-15"), IsCompleted = false, OwnerId = 1, ExecutorFirstName = "Иван", ExecutorLastName = "Иванов", ExecutorEmail = "ivanov@example.com", Priority = TaskPriority.Normal },
                    new TaskItem { Title = "Список задач", Description = "Фильтрация и сортировка", CreatedAt = ParseUtc("2026-02-15"), DueDate = ParseUtc("2026-02-18"), IsCompleted = true, OwnerId = 2, ExecutorFirstName = "Петр", ExecutorLastName = "Петров", ExecutorEmail = "petrov@example.com", Priority = TaskPriority.Important },
                    new TaskItem { Title = "Купить хлеб", Description = "Сходить в магазин за хлебом", CreatedAt = ParseUtc("2026-04-02"), DueDate = ParseUtc("2026-04-05"), IsCompleted = false, OwnerId = 2, ExecutorFirstName = "Иван", ExecutorLastName = "Иванов", ExecutorEmail = "ivan@example.com", Priority = TaskPriority.Normal },
                    new TaskItem { Title = "Позвонить маме", Description = "Уточнить планы на выходные", CreatedAt = ParseUtc("2026-04-03"), DueDate = ParseUtc("2026-04-06"), IsCompleted = false, OwnerId = 1, ExecutorFirstName = "Мария", ExecutorLastName = "Петрова", ExecutorEmail = "maria@example.com", Priority = TaskPriority.Normal },
                    new TaskItem { Title = "Убрать комнату", Description = "Пропылесосить и вытереть пыль", CreatedAt = ParseUtc("2026-04-04"), DueDate = ParseUtc("2026-04-07"), IsCompleted = true, OwnerId = 2, ExecutorFirstName = "Сергей", ExecutorLastName = "Сидоров", ExecutorEmail = "sergey@example.com", Priority = TaskPriority.Normal },
                    new TaskItem { Title = "Оплатить интернет", Description = "Через онлайн-банк", CreatedAt = ParseUtc("2026-04-05"), DueDate = ParseUtc("2026-04-08"), IsCompleted = false, OwnerId = 1, ExecutorFirstName = "Анна", ExecutorLastName = "Кузнецова", ExecutorEmail = "anna@example.com", Priority = TaskPriority.Normal },
                    new TaskItem { Title = "Сделать зарядку", Description = "Утром 15 минут", CreatedAt = ParseUtc("2026-04-06"), DueDate = ParseUtc("2026-04-09"), IsCompleted = true, OwnerId = 2, ExecutorFirstName = "Олег", ExecutorLastName = "Морозов", ExecutorEmail = "oleg@example.com", Priority = TaskPriority.Normal },
                    new TaskItem { Title = "Прочитать книгу", Description = "Закончить главу", CreatedAt = ParseUtc("2026-04-8"), DueDate = ParseUtc("2026-04-10"), IsCompleted = false, OwnerId = 1, ExecutorFirstName = "Елена", ExecutorLastName = "Смирнова", ExecutorEmail = "elena@example.com", Priority = TaskPriority.Normal },
                    new TaskItem { Title = "Сходить в аптеку", Description = "Купить витамины", CreatedAt = ParseUtc("2026-04-8"), DueDate = ParseUtc("2026-04-11"), IsCompleted = false, OwnerId = 2, ExecutorFirstName = "Дмитрий", ExecutorLastName = "Фёдоров", ExecutorEmail = "dmitriy@example.com", Priority = TaskPriority.Normal },
                    new TaskItem { Title = "Погулять в парке", Description = "Вечером после работы", CreatedAt = ParseUtc("2026-04-9"), DueDate = ParseUtc("2026-04-12"), IsCompleted = true, OwnerId = 1, ExecutorFirstName = "Наталья", ExecutorLastName = "Васильева", ExecutorEmail = "natalya@example.com", Priority = TaskPriority.Normal },
                    new TaskItem { Title = "Сделать домашнее задание", Description = "По математике", CreatedAt = ParseUtc("2026-04-10"), DueDate = ParseUtc("2026-04-13"), IsCompleted = false, OwnerId = 2, ExecutorFirstName = "Алексей", ExecutorLastName = "Попов", ExecutorEmail = "alexey@example.com", Priority = TaskPriority.Normal },
                    new TaskItem { Title = "Приготовить ужин", Description = "Суп и салат", CreatedAt = ParseUtc("2026-04-11"), DueDate = ParseUtc("2026-04-14"), IsCompleted = true, OwnerId = 1, ExecutorFirstName = "Ирина", ExecutorLastName = "Новикова", ExecutorEmail = "irina@example.com", Priority = TaskPriority.Normal },
                    new TaskItem { Title = "Сходить на почту", Description = "Забрать посылку", CreatedAt = ParseUtc("2026-04-12"), DueDate = ParseUtc("2026-04-15"), IsCompleted = false, OwnerId = 2, ExecutorFirstName = "Павел", ExecutorLastName = "Зайцев", ExecutorEmail = "pavel@example.com", Priority = TaskPriority.Normal },
                    new TaskItem { Title = "Выбросить мусор", Description = "Вечером", CreatedAt = ParseUtc("2026-04-13"), DueDate = ParseUtc("2026-04-16"), IsCompleted = true, OwnerId = 1, ExecutorFirstName = "Светлана", ExecutorLastName = "Михайлова", ExecutorEmail = "svetlana@example.com", Priority = TaskPriority.Normal },
                    new TaskItem { Title = "Записаться к врачу", Description = "Терапевт", CreatedAt = ParseUtc("2026-04-14"), DueDate = ParseUtc("2026-04-17"), IsCompleted = false, OwnerId = 2, ExecutorFirstName = "Виктор", ExecutorLastName = "Соловьёв", ExecutorEmail = "victor@example.com", Priority = TaskPriority.Normal },
                    new TaskItem { Title = "Купить молоко", Description = "В магазине у дома", CreatedAt = ParseUtc("2026-04-15"), DueDate = ParseUtc("2026-04-18"), IsCompleted = false, OwnerId = 1, ExecutorFirstName = "Татьяна", ExecutorLastName = "Орлова", ExecutorEmail = "tatyana@example.com", Priority = TaskPriority.Normal }
                );
                context.SaveChanges();
            }
        }
    }
}

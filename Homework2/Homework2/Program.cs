using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System;

namespace Homework2
{
    internal class Program
    {
        public const string ProgrammVersionInfo = "версия 1.0, 17.02.2026";

        static void Main(string[] args)
        {
            try
            {
                var toDoService = new ToDoService(
                    ToDoService.TaskCountLimitMin,
                    ToDoService.TaskCountLimitMax,
                    ToDoService.TaskLengthLimitMin,
                    ToDoService.TaskLengthLimitMax
                    );
                var handler = new UpdateHandler(new UserService(), toDoService);
                var botClient = new ConsoleBotClient();
                botClient.StartReceiving(handler);
            }
            catch (Exception exp)
            {
                Console.WriteLine($"Произошла непредвиденная ошибка:");
                Console.WriteLine($"Тип: {exp.GetType().FullName}");
                Console.WriteLine($"Исключение: {exp.Message}");
                Console.WriteLine($"Трассировка стека: {exp.StackTrace}");
                Console.WriteLine($"Информация об исключении: {exp.InnerException}");
            }
        }
    }
}
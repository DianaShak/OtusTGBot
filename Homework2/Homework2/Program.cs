using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Threading;

namespace Homework2
{
    internal class Program
    {
        public const string ProgrammVersionInfo = "версия 1.0, 17.02.2026";

        static void Main(string[] args)
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                //CancellationToken token = cts.Token;
                
                var toDoRepository = new InMemoryToDoRepository();
                var userRepository = new InMemoryUserRepository();
                var toDoService = new ToDoService(
                    toDoRepository,
                    ToDoService.TaskCountLimitMin,
                    ToDoService.TaskCountLimitMax,
                    ToDoService.TaskLengthLimitMin,
                    ToDoService.TaskLengthLimitMax
                    );
                var toDoReportService = new ToDoReportService(toDoRepository, userRepository);
                var userService = new UserService(userRepository);
                var handler = new UpdateHandler(userService, toDoService, toDoReportService);
                var botClient = new ConsoleBotClient();
                botClient.StartReceiving(handler, cts.Token);
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
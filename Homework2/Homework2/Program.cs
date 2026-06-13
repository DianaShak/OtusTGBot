using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Homework2
{
    internal class Program
    {
        public const string ProgrammVersionInfo = "версия 1.0, 17.02.2026";

        static async Task Main(string[] args)
        {
            string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN", EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Bot token not found. Please set the TELEGRAM_BOT_TOKEN environment variable.");
                return;
            }

            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                
                var toDoRepository = new FileToDoRepository("C:\\Users\\Diana\\Desktop\\botData\\ToDoItems");
                var userRepository = new FileUserRepository("C:\\Users\\Diana\\Desktop\\botData\\ToDoUsers");
                var toDoService = new ToDoService(
                    toDoRepository,
                    ToDoService.TaskCountLimitMin,
                    ToDoService.TaskCountLimitMax,
                    ToDoService.TaskLengthLimitMin,
                    ToDoService.TaskLengthLimitMax
                    );
                var toDoReportService = new ToDoReportService(toDoRepository, userRepository, cts);
                var userService = new UserService(userRepository);
                var handler = new UpdateHandler(userService, toDoService, toDoReportService);
                var botClient = new TelegramBotClient(token);
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = [UpdateType.Message],
                    DropPendingUpdates = true
                };
                botClient.StartReceiving(handler, receiverOptions);

                Console.WriteLine("Нажмите клавишу A для выхода. Для получения информации о Telegram-боте нажмите любую другую клавишу.");

                while (Console.ReadKey(true).Key != ConsoleKey.A)
                {
                    var me = await botClient.GetMe();
                    Console.WriteLine($"{me.FirstName} запущен!");
                }
                Console.WriteLine("Вы нажали A. Программа завершена.");
                cts.Cancel();
                //Task.Delay(-1); // Устанавливаем бесконечную задержку
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
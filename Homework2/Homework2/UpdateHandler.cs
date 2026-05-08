using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Homework2
{

    public class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly IToDoReportService _toDoReportService;

        public UpdateHandler(IUserService userService, IToDoService toDoService, IToDoReportService toDoReportService)
        {
            _userService = userService;
            _toDoService = toDoService;
            _toDoReportService = toDoReportService;
        }
       
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            try
            {
                var splitInput = update.Message.Text?.Split(' ');
                ToDoUser user;
                try
                {
                    user = await _userService.GetUser(update.Message.From.Id, ct);
                   
                }
                catch (Exception e)
                {
                    user = null;
                }
                if (user == null && splitInput[0] != "/start")
                {
                    await botClient.SendMessage(update.Message.Chat,
                        "Вы не зарегистрированы, " +
                        "Вам доступны команды /help, /info и /start для регистрации.",
                        ct);
                    return;
                }


                /*//string taskLength = string.Empty;
                //if (taskCountLimit == 0)
                //{
                //    botClient.SendMessage(update.Message.Chat, "Введите максимально допустимое количество задач (от 1 до 100):");
                //    var maxNumber = Console.ReadLine();
                //    taskCountLimit = ParseAndValidateInt(maxNumber);
                //    botClient.SendMessage(update.Message.Chat, $"Максимальное количество задач: {taskCountLimit}");
                //}

                //if (taskLengthLimit == 0)
                //{
                //    botClient.SendMessage(update.Message.Chat, "Введите максимально допустимую длину задачи (от 1 до 100):");
                //    var taskLength = Console.ReadLine();
                //    taskLengthLimit = ParseAndValidateInt(taskLength);
                //    botClient.SendMessage(update.Message.Chat, $"Максимальная длина задачи: {taskLengthLimit}");
                //}*/

                if (splitInput == null || splitInput.Length < 1)
                {
                    await botClient.SendMessage(update.Message.Chat,
                        "Такой команды нет. Пожалуйста, введите команду из предложенных:"
                        + Environment.NewLine
                        + @"/start /help /info /addtask /showtasks /removetask /exit",
                        ct);
                    return;
                }

                switch (splitInput[0])
                {
                    case "/start":
                        if (user == null)
                        {
                            await _userService.RegisterUser(update.Message.From.Id, update.Message.From.Username, ct);
                            await botClient.SendMessage(update.Message.Chat, "Выберите нужную команду!", ct);
                            await botClient.SendMessage(update.Message.Chat, @"/start /help /info /addtask /completetask /showtasks /showalltasks /removetask /exit", ct);
                        }
                        //var userName = GetNameMethod(botClient, update.Message.Chat);
                        //user = new ToDoUser(userName, 0);  //  1 пункт
                        await botClient.SendMessage(update.Message.Chat, $"Здравствуйте, {update.Message.From.Username}! Чем я могу помочь?", ct);
                        break;

                    case "/help":
                        //  Отображает краткую справочную информацию о том, как пользоваться программой.
                        HelpMethod(botClient, update.Message.Chat, update.Message.From, ct);
                        break;

                    case "/info":
                        //  Предоставляет информацию о версии программы и дате её создания.
                        InfoMethod(botClient, update.Message.Chat, update.Message.From, ct);
                        break;

                    case "/addtask":
                        if (user != null)
                        {
                            await AddTask(botClient, update, splitInput, user, ct);
                        }
                        break;

                    case "/showtasks":
                        if (user != null)
                        {
                            await ShowTasks(botClient, update, user, ct);
                        }
                        break;

                    case "/removetask":
                        if (user != null)
                        {
                            await RemoveTask(botClient, update, splitInput, user, ct);
                        }
                        break;

                    case "/completetask":
                        //Найти задачу по Id
                        //Обновить State на ToDoItemState.Completed
                        //Обновить StateChangedAt
                        //Пример: / completetask 73c7940a - ca8c - 4327 - 8a15 - 9119bffd1d5e
                        if (user != null)
                        {
                            await CompleteTask(botClient, update, splitInput, user, ct);
                        }
                        break;

                    case "/showalltasks":
                        //Добавить обработку новой команды / showalltasks.По ней выводить команды с любым State и добавить State в вывод
                        //Пример: (Active)Имя задачи - 01.01.2025 00:00:00 - ffbfe448 - 4b39 - 4778 - 98aa - 1aed98f7eed8
                        //ShowAllTasks(tasks, true, botClient, update.Message.Chat);
                        if (user != null)
                        {
                            await ShowAllTasks(botClient, update, user, ct);
                        }
                        break;

                    case "/find":
                        //Добавить обработку новой команды / find.
                        //Пример команды: / find Имя
                        //Вывод в консоль должен быть как в / showtask
                        if (user != null)
                        {
                            await Find(botClient, update, splitInput, user, ct);

                        }
                        break;

                    case "/report":
                        if (user != null)
                        {
                            await ShowReport(botClient, update, user, ct);
                        }
                        break;

                    case "/exit":
                        break;

                    default:

                        break;
                }

            }
            catch (TaskCountLimitException e)
            {
                await botClient.SendMessage(update.Message.Chat, $"Исключение: {e.Message}", ct);
            }
            catch (TaskLengthLimitException e)
            {
                await botClient.SendMessage(update.Message.Chat, $"Исключение: {e.Message}", ct);
            }
            catch (DuplicateTaskException e)
            {
                await botClient.SendMessage(update.Message.Chat, $"Исключение: {e.Message}", ct);
            }
            catch (ArgumentException e)
            {
                await botClient.SendMessage(update.Message.Chat, $"Ошибка: {e.Message}", ct);
            }

            async Task Find(ITelegramBotClient botClient, Update update, string[] splitInput, ToDoUser? user, CancellationToken ct)
            {
                var taskToFind = string.Join(' ', splitInput[1..]);
                var tasks = await _toDoService.Find(user, taskToFind, ct);
                string message = "Задач нет.";
                if (tasks.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach (var task in tasks)
                    {
                        sb.AppendFormat("{0} - {1} - {2}\n", task.Name, task.CreatedAt, task.Id);
                    }
                    message = sb.ToString();
                }
                await botClient.SendMessage(update.Message.Chat, message, ct);
            }
        }

        private async Task AddTask(ITelegramBotClient botClient, Update update, string[] splitInput, ToDoUser? user, CancellationToken ct)
        {
            var taskToAdd = string.Join(' ', splitInput[1..]);
            //var taskToAdd = update.Message.Text.Substring();
            await _toDoService.Add(user, taskToAdd, ct);
            await botClient.SendMessage(update.Message.Chat, $"Задача '{taskToAdd}' добавлена.", ct);
        }

        private async Task ShowTasks(ITelegramBotClient botClient, Update update, ToDoUser? user, CancellationToken ct)
        {
            var tasks = await _toDoService.GetActiveByUserId(user.UserId, ct);
            string message = "Задач нет.";
            if (tasks.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var task in tasks)
                {
                    sb.AppendFormat("{0} - {1} - {2}\n", task.Name, task.CreatedAt, task.Id);
                }
                message = sb.ToString();
            }
            await botClient.SendMessage(update.Message.Chat, message, ct);
        }

        private async Task RemoveTask(ITelegramBotClient botClient, Update update, string[] splitInput, ToDoUser? user, CancellationToken ct)
        {
            var taskNumberToRemove = string.Join(' ', splitInput[1..]);
            var taskNumber = Validator.ParseAndValidateInt(taskNumberToRemove, ToDoService.TaskCountLimitMin, ToDoService.TaskCountLimitMax);
            var tasks = await _toDoService.GetAllByUserId(user.UserId, ct);

            if (tasks.Count > 0)
            {
                int indexToRemove = taskNumber - 1;

                if (indexToRemove >= 0 && indexToRemove < tasks.Count)
                {
                    var taskToRemove = tasks[taskNumber - 1];
                    await _toDoService.Delete(taskToRemove.Id, ct);
                    await botClient.SendMessage(update.Message.Chat, $"Задача '{taskToRemove.Name}' удалена.", ct);
                }
                else
                {
                    await botClient.SendMessage(update.Message.Chat, "Элемент с таким номером не существует. Пожалуйста, введите корректный номер.", ct);
                }
            }
            else
            {
                await botClient.SendMessage(update.Message.Chat, "Список пуст.", ct);
            }
        }

        private async Task CompleteTask(ITelegramBotClient botClient, Update update, string[] splitInput, ToDoUser? user, CancellationToken ct)
        {
            if (user != null)
            {
                var idToFind = string.Join(' ', splitInput[1..]);
                Validator.ValidateString(idToFind);
                if (Guid.TryParse(idToFind, out Guid idSearch))
                {
                    await _toDoService.MarkCompleted(idSearch, ct);
                    await botClient.SendMessage(update.Message.Chat, "Задача завершена.", ct);
                }
            }
        }

        private void InfoMethod(ITelegramBotClient botClient, Chat chat, User user, CancellationToken ct)
        {
            botClient.SendMessage(chat,
                $"{((user == null)
                ? "В"
                : $"{user.Username}, в")}ерсия и дата создания: {Program.ProgrammVersionInfo}",
                ct
                );
        }

        private void HelpMethod(ITelegramBotClient botClient, Chat chat, User user, CancellationToken ct)
        {
            botClient.SendMessage(chat,
                $"{((user != null)
                ? $"{user.Username},\nк"
                : "К")}оманда /help позволяет получить краткую справочную информацию о том, как пользоваться программой," +
                $"\nкоманда /info позволяет получить информацию о версии программы и дате её создания, " +
                $"\nкоманда /addtask позволяет добавлять задачи," +
                $"\nкоманда /completetask меняет статус задачи на completed," +
                $"\nкоманда /showtasks выводит список введенных задач со статусом Active, " +
                $"\nкоманда /showalltasks выводит список всех введенных задач, " +
                $"\nкоманда /removetask позволяет удалить определенную задачу," +
                $"\nкоманда /report позволяет узнать статистику по задачам," +
                $"\nкоманда /find позволяет найти все задачи, начинающиеся с введенного слова," +
                $"\nкоманда /exit позволяет выйти из меню.",
                ct
                );
        }
        private async Task ShowReport(ITelegramBotClient botClient, Update update, ToDoUser user, CancellationToken ct)
        {
            var (total, completed, active, generatedAt) = await _toDoReportService.GetUserStats(user.UserId, ct);
            await botClient.SendMessage(
                update.Message.Chat,
                $"Статистика по задачам на {generatedAt}. " +
                $"Всего: {total}; Завершенных: {completed}; " +
                $"Активных: {active};",
                ct
            );
        }
        private async Task ShowAllTasks(ITelegramBotClient botClient, Update update, ToDoUser user, CancellationToken ct)
        {
            var tasks = await _toDoService.GetAllByUserId(user.UserId, ct);
            string message = "Задач нет.";
            if (tasks.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var task in tasks)
                {
                    sb.AppendFormat("({0}) {1} - {2} - {3}\n", task.State, task.Name, task.CreatedAt, task.Id);
                }
                message = sb.ToString();
            }
            await botClient.SendMessage(update.Message.Chat, message, ct);
        }
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
        {
            Console.WriteLine($"HandleError: {exception})");
            return Task.CompletedTask;
        }
    }
}

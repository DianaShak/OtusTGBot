using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Xsl;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace Homework2
{

    public class UpdateHandler : IUpdateHandler
    {
        private const string StartCommand = "/start";
        private const string HelpCommand = "/help";
        private const string InfoCommand = "/info";
        private const string AddTaskCommand = "/addtask";
        private const string ShowTasksCommand = "/showtasks";
        private const string RemoveTaskCommand = "/removetask";
        private const string CompleteTaskCommand = "/completetask";
        private const string ShowAllTaskCommand = "/showalltasks";
        private const string FindCommand = "/find";
        private const string ReportCommand = "/report";
        private const string ExitCommand = "/exit";
        private static readonly HashSet<string> _unregisteredCommands = new HashSet<string>
        { StartCommand, HelpCommand, InfoCommand };
        private static readonly HashSet<string> _registeredCommands = new HashSet<string>
        { AddTaskCommand, ShowTasksCommand, ShowAllTaskCommand, FindCommand, ReportCommand, ExitCommand };

        private readonly List<BotCommand> commands = new()
                {
                    new BotCommand { Command = "/start", Description = "Регистрация"},
                    new BotCommand { Command = "/addtask", Description = "Добавить задачу"},
                    new BotCommand { Command = "/removetask", Description = "Удалить определенную задачу"},
                    new BotCommand { Command = "/showalltasks", Description = "Cписок всех задач"},
                    new BotCommand { Command = "/showtasks", Description = "Cписок задач со статусом Active"},
                    new BotCommand { Command = "/find", Description = "Найти задачи по введенному слову"},
                    new BotCommand { Command = "/report", Description = "Статистика по задачам"},
                    new BotCommand { Command = "/info", Description = "Информация о версии и дате создания"},
                    new BotCommand { Command = "/help", Description = "Справочная информация о коммандах"},
                };
        private readonly List<BotCommand> unregisteredCommands = new()
                {
                    new BotCommand { Command = "/start", Description = "Регистрация"},
                    new BotCommand { Command = "/info", Description = "Информация о версии и дате создания"},
                    new BotCommand { Command = "/help", Description = "Справочная информация о коммандах"},
                };
        private readonly ReplyKeyboardMarkup MainKeyboard = new(
                new KeyboardButton[] { "/showalltasks", "/showtasks", "/addtask", "/report" })
        {
            ResizeKeyboard = true
        };

        private readonly ReplyKeyboardMarkup StartKeyboard = new(
                new KeyboardButton[] { "/start" })
        {
            ResizeKeyboard = true
        };

        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly IToDoReportService _toDoReportService;
        private readonly IEnumerable<IScenario> _scenarios;
        private readonly IScenarioContextRepository _contextRepository;

        public UpdateHandler(IUserService userService, IToDoService toDoService, IToDoReportService toDoReportService, IEnumerable<IScenario> scenarios, IScenarioContextRepository contextRepository)
        {
            _userService = userService;
            _toDoService = toDoService;
            _toDoReportService = toDoReportService;
            _scenarios = scenarios;
            _contextRepository = contextRepository;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            try
            {
                if (update.Message?.Text == null)
                {
                    return;
                }
                var splitInput = update.Message.Text?.Split(' ');
                ToDoUser user;

                try
                {
                    user = await _userService.GetUser(update.Message.From.Id, ct);
                    await botClient.SetMyCommands(commands);
                }
                catch (Exception e)
                {
                    user = null;
                    if (_registeredCommands.Contains(splitInput[0]))
                    {
                        await botClient.SendMessage(update.Message.Chat,
                            "Вы не зарегистрированы, Вам доступна команда /start для регистрации.",
                            replyMarkup: StartKeyboard);
                        await botClient.SetMyCommands(unregisteredCommands);
                        return;
                    }
                    else if (!_unregisteredCommands.Contains(splitInput[0]))
                    {
                        await botClient.SendMessage(update.Message.Chat, "Команда не существует.");
                        await botClient.SetMyCommands(unregisteredCommands);
                        return;
                    }
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

                if (splitInput[0] == "/cancel")
                {
                    botClient.SendMessage(update.Message.Chat.Id, "Выберите команду:", replyMarkup: MainKeyboard);
                    _contextRepository.ResetContext(update.Message.From.Id, ct);
                }

                if (user != null)
                {
                    ScenarioContext? context = await _contextRepository.GetContext(user.TelegramUserId, ct);

                    if (context != null && context.CurrentScenario != ScenarioType.None)
                    {
                        await ProcessScenario(botClient, context, update, ct);
                        return;
                    }
                }

                if (splitInput == null || splitInput.Length < 1)
                {
                    await botClient.SendMessage(update.Message.Chat,
                        "Такой команды нет. Пожалуйста, введите команду из предложенных:",
                         replyMarkup: MainKeyboard);
                    return;
                }

                //if (user == null && splitInput[0] != StartCommand && splitInput[0] != HelpCommand && splitInput[0] != InfoCommand)
                //{
                //    await botClient.SendMessage(update.Message.Chat,
                //                    "Вы не зарегистрированы, " +
                //                    "Вам доступна команда /start для регистрации.", replyMarkup: StartKeyboard);
                //    return;
                //}

                switch (splitInput[0])
                {
                    case StartCommand:
                        if (user == null)
                        {
                            await _userService.RegisterUser(update.Message.From.Id, update.Message.From.Username, ct);
                            await botClient.SendMessage(update.Message.Chat, "Вы зарегистрированы! Выберите нужную команду!");
                            await botClient.SendMessage(update.Message.Chat, @"/start /help /info /addtask /completetask /showtasks /showalltasks /removetask /exit", replyMarkup: MainKeyboard);
                        }
                        await botClient.SendMessage(update.Message.Chat, $"Здравствуйте, {update.Message.From.Username}! Чем я могу помочь?", replyMarkup: MainKeyboard);
                        break;

                    case HelpCommand:
                        //  Отображает краткую справочную информацию о том, как пользоваться программой.
                        HelpMethod(botClient, update.Message.Chat, user, ct);
                        break;

                    case InfoCommand:
                        //  Предоставляет информацию о версии программы и дате её создания.
                        InfoMethod(botClient, update.Message.Chat, user, ct);
                        break;

                    case AddTaskCommand:
                        await AddTask(botClient, update, splitInput, user, ct);
                        break;

                    case ShowTasksCommand:
                        await ShowTasks(botClient, update, user, ct);
                        break;

                    case RemoveTaskCommand:
                        await RemoveTask(botClient, update, splitInput, user, ct);
                        break;

                    case CompleteTaskCommand:
                        //Найти задачу по Id
                        //Обновить State на ToDoItemState.Completed
                        //Обновить StateChangedAt
                        //Пример: / completetask 73c7940a - ca8c - 4327 - 8a15 - 9119bffd1d5e
                        await CompleteTask(botClient, update, splitInput, user, ct);
                        break;

                    case ShowAllTaskCommand:
                        //Добавить обработку новой команды / showalltasks.По ней выводить команды с любым State и добавить State в вывод
                        //Пример: (Active)Имя задачи - 01.01.2025 00:00:00 - ffbfe448 - 4b39 - 4778 - 98aa - 1aed98f7eed8
                        //ShowAllTasks(tasks, true, botClient, update.Message.Chat);
                        await ShowAllTasks(botClient, update, user, ct);
                        break;

                    case FindCommand:
                        //Добавить обработку новой команды / find.
                        //Пример команды: / find Имя
                        //Вывод в консоль должен быть как в / showtask
                        await Find(botClient, update, splitInput, user, ct);
                        break;

                    case ReportCommand:
                        await ShowReport(botClient, update, user, ct);
                        break;

                    case ExitCommand:
                        break;

                    default:
                        break;
                }
            }
            catch (TaskCountLimitException e)
            {
                await botClient.SendMessage(update.Message.Chat, $"Исключение: {e.Message}");
            }
            catch (TaskLengthLimitException e)
            {
                await botClient.SendMessage(update.Message.Chat, $"Исключение: {e.Message}");
            }
            catch (DuplicateTaskException e)
            {
                await botClient.SendMessage(update.Message.Chat, $"Исключение: {e.Message}");
            }
            catch (ArgumentException e)
            {
                await botClient.SendMessage(update.Message.Chat, $"Ошибка: {e.Message}");
            }
        }

        private async Task Find(ITelegramBotClient botClient, Update update, string[] splitInput, ToDoUser? user, CancellationToken ct)
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
            await botClient.SendMessage(update.Message.Chat, message, replyMarkup: MainKeyboard);
        }

        private async Task AddTask(ITelegramBotClient botClient, Update update, string[] splitInput, ToDoUser? user, CancellationToken ct)
        {
            var newContext = new ScenarioContext(ScenarioType.Addtask);

            await ProcessScenario(botClient, newContext, update, ct);

            //var taskToAdd = string.Join(' ', splitInput[1..]);
            //await _toDoService.Add(user, taskToAdd, ct);
            //await botClient.SendMessage(update.Message.Chat, $"Задача '{taskToAdd}' добавлена.", replyMarkup: MainKeyboard);
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
                    sb.AppendFormat("{0} - {1} - `{2}`\n", task.Name, task.CreatedAt, task.Id);
                }
                message = sb.ToString();
            }
            await botClient.SendMessage(update.Message.Chat, message, replyMarkup: MainKeyboard);
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
                    await botClient.SendMessage(update.Message.Chat, $"Задача '{taskToRemove.Name}' удалена.", replyMarkup: MainKeyboard);
                }
                else
                {
                    await botClient.SendMessage(update.Message.Chat, "Элемент с таким номером не существует. Пожалуйста, введите корректный номер.", replyMarkup: MainKeyboard);
                }
            }
            else
            {
                await botClient.SendMessage(update.Message.Chat, "Список пуст.", replyMarkup: MainKeyboard);
            }
        }

        private async Task CompleteTask(ITelegramBotClient botClient, Update update, string[] splitInput, ToDoUser? user, CancellationToken ct)
        {
            var idToFind = string.Join(' ', splitInput[1..]);
            Validator.ValidateString(idToFind);
            if (Guid.TryParse(idToFind, out Guid idSearch))
            {
                await _toDoService.MarkCompleted(idSearch, ct);
                await botClient.SendMessage(update.Message.Chat, "Задача завершена.", replyMarkup: MainKeyboard);
            }
        }

        private void InfoMethod(ITelegramBotClient botClient, Chat chat, ToDoUser user, CancellationToken ct)
        {
            var keyboard = user == null
                ? StartKeyboard
                : MainKeyboard;

            botClient.SendMessage(chat,
                $"{((user == null)
                ? "В"
                : $"{user.TelegramUserName}, в")}ерсия и дата создания: {Program.ProgrammVersionInfo}",
                replyMarkup: keyboard
                );
        }

        private void HelpMethod(ITelegramBotClient botClient, Chat chat, ToDoUser user, CancellationToken ct)
        {
            var keyboard = user == null
                ? StartKeyboard
                : MainKeyboard;

            botClient.SendMessage(chat,
                $"{((user != null)
                ? $"{user.TelegramUserName},\nк"
                : "К")}оманда /help позволяет получить краткую справочную информацию о том, как пользоваться программой," +
                $"\nкоманда /info позволяет получить информацию о версии программы и дате её создания, " +
                $"\nкоманда /addtask позволяет добавлять задачи," +
                $"\nкоманда /completetask меняет статус задачи на completed," +
                $"\nкоманда /showtasks выводит список введенных задач со статусом Active, " +
                $"\nкоманда /showalltasks выводит список всех введенных задач, " +
                $"\nкоманда /removetask позволяет удалить определенную задачу," +
                $"\nкоманда /report позволяет узнать статистику по задачам," +
                $"\nкоманда /cancel позволяет отменить текущий суенарий добавления задачи," +
                $"\nкоманда /find позволяет найти все задачи, начинающиеся с введенного слова.",
                replyMarkup: keyboard);
        }

        private async Task ShowReport(ITelegramBotClient botClient, Update update, ToDoUser user, CancellationToken ct)
        {
            var (total, completed, active, generatedAt) = await _toDoReportService.GetUserStats(user.UserId, ct);
            await botClient.SendMessage(
                update.Message.Chat,
                $"Статистика по задачам на {generatedAt}. " +
                $"Всего: {total}; Завершенных: {completed}; " +
                $"Активных: {active};",
                replyMarkup: MainKeyboard
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
                    sb.AppendFormat("({0}) {1} - {2} - `{3}`\n", task.State, task.Name, task.CreatedAt, task.Id);
                }
                message = sb.ToString();
            }
            await botClient.SendMessage(update.Message.Chat, message, replyMarkup: MainKeyboard);
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"HandleError: {exception})");
            return Task.CompletedTask;
        }

        private IScenario GetScenario(ScenarioType scenario)
        {
            return _scenarios.First(x => x.CanHandle(scenario));
        }

        private async Task ProcessScenario(ITelegramBotClient botClient, ScenarioContext context, Update update, CancellationToken ct)
        {
            IScenario getScenario;
            try
            {
                getScenario = GetScenario(context.CurrentScenario);
            }
            catch (Exception ex)
            {
                botClient.SendMessage(update.Message.Chat.Id, $"Исключение: {ex.Message}");
                return;
            }

            var scenarioResult = await getScenario.HandleMessageAsync(botClient, context, update, ct);
            if (scenarioResult == ScenarioResult.Completed)
            {
                await _contextRepository.ResetContext(update.Message.From.Id, ct);
            }
            else
            {
                await _contextRepository.SetContext(update.Message.From.Id, context, ct);
            }
        }
    }
}

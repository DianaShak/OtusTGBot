using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Homework2
{
    internal class AddTaskScenario : IScenario
    {
        private const string DateFormat = "dd'.'MM'.'yyyy";
        private const string User = "user";
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly ReplyKeyboardMarkup CancelKeyboard = new(
                new KeyboardButton[] { "/cancel" })
        {
            ResizeKeyboard = true
        };

        public AddTaskScenario(IUserService userService, IToDoService toDoService)
        {
            _toDoService = toDoService;
            _userService = userService;
        }

        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.Addtask;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            //        Добавить обработку шагов сценария(ScenarioContext.CurrentStep) через switch case
            //case null
            //Получить ToDoUser и сохранить его в ScenarioContext.Data.
            //Отправить пользователю сообщение "Введите название задачи:"
            //Обновить ScenarioContext.CurrentStep на "Name"
            //Вернуть ScenarioResult.Transition
            //case "Name"
            //Вызвать IToDoService.Add.Передать ToDoUser из ScenarioContext.Data и name из сообщения
            //Вернуть ScenarioResult.Completed
            //Добавить заполнение Deadline в AddTaskScenario через отдельный шаг. Формат текста dd.MM.yyyy.
            //Если пользователь введет дату в неверном формате, сценарий не должен прерваться и нужно еще раз запросить дату.
            switch (context.CurrentStep)
            {
                case null:
                    var userId = update.Message.From.Id;
                    var user = await _userService.GetUser(userId, ct);

                    context.Data[User] = user;

                    await bot.SendMessage(update.Message.Chat.Id, "Введите название задачи:", replyMarkup: CancelKeyboard);

                    context.CurrentStep = "Name";

                    return ScenarioResult.Transition;
                case "Name":
                    context.TemporaryTaskName = update.Message.Text;

                    await bot.SendMessage(update.Message.Chat.Id, "Введите дедлайн задачи в формате дд.мм.гггг (например, 25.05.2027): ", replyMarkup: CancelKeyboard);

                    context.CurrentStep = "Deadline";

                    return ScenarioResult.Transition;
                case "Deadline":
                    var deadlineFromMessage = update.Message.Text;

                    if (DateTime.TryParseExact(deadlineFromMessage, DateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime deadline))
                    {
                        if (deadline.Date < DateTime.Today)
                        {
                            await bot.SendMessage(update.Message.Chat.Id, "Дата неактуальна, попробуйте снова.", replyMarkup: CancelKeyboard);
                            context.CurrentStep = "Deadline";
                        }
                        else
                        {
                            //context.Data[context.CurrentStep] = deadline;
                            context.Data.TryGetValue(context.CurrentStep, out var someValue);
                            if (someValue is ToDoUser toDoUser)
                            {
                                await _toDoService.Add(toDoUser, context.TemporaryTaskName, deadline, ct);
                                await bot.SendMessage(update.Message.Chat.Id, $"Задача '{context.TemporaryTaskName}' добавлена.");
                            }
                        }
                    }
                    else
                    {
                        await bot.SendMessage(update.Message.Chat.Id, "Неверный формат даты. Введите дэдлайн задачи в формате дд.мм.гггг (например, 25.05.2025): ", replyMarkup: CancelKeyboard);
                        context.CurrentStep = "Deadline";
                    }
                    context.TemporaryTaskName = null;
                    return ScenarioResult.Completed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(context.CurrentStep), "Неизвестный шаг сценария.");
            }
        }
    }
}

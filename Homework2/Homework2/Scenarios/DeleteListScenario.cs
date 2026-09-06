using Homework2.Core.Services;
using Homework2.TelegramBot.DTO;
using Homework2.TelegramBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Homework2.Core.Entities;

namespace Homework2.Scenarios
{
    internal class DeleteListScenario : IScenario
    {
        private const string User = "user";
        private const string ToDoListItem = "toDoListItem";
        private readonly IUserService _userService;
        private readonly IToDoListService _toDoListService;
        private readonly IToDoService _toDoService;
        public DeleteListScenario(IUserService userService, IToDoListService toDoListService, IToDoService toDoService) 
        { 
            _toDoListService = toDoListService;
            _userService = userService;
            _toDoService = toDoService;
        }

        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.DeleteList;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            var messageText = update.Message?.Text;
            var userId = update.Message!.From!.Id;
            var user = await _userService.GetUser(userId, ct);
            var guidUserId = user.UserId;

            switch (context.CurrentStep)
            {
                case null:
                    context.Data[User] = user;
                    var allUserLists = await _toDoListService.GetUserLists(guidUserId, ct);
                    var listsKeyboard = DeleteListKeyboardHelper.GetListsKeyboard(allUserLists);

                    await bot.SendMessage(
                        update.Message.Chat.Id,
                        "Выберете список для удаления:",
                        replyMarkup: listsKeyboard,
                        cancellationToken: ct);

                    context.CurrentStep = "Approve";
                    return ScenarioResult.Transition;

                case "Approve":
                    var callback = update.CallbackQuery;
                    var toDoListDto = ToDoListCallbackDto.FromString(callback.Data);
                    var listId = toDoListDto.ToDoListId;

                    if (listId == null)
                    {
                        throw new ArgumentNullException(nameof(listId));
                    }
                    var toDoList = await _toDoListService.Get(listId.Value, ct);
                    context.Data[ToDoListItem] = toDoList;

                    var userLists = await _toDoListService.GetUserLists(guidUserId, ct);

                    var approveKeyboard = ApproveKeyboardHelper.GetApproveKeyboard(userLists);

                    await bot.SendMessage(
                        update.Message.Chat.Id,
                        $"Подтверждаете удаление списка {toDoList.Name} и всех его задач?",
                        replyMarkup: approveKeyboard,
                        cancellationToken: ct);

                    context.CurrentStep = "Delete";
                    return ScenarioResult.Transition;
                case "Delete":
                    var callbackData = update!.CallbackQuery!.Data;
                    
                    if (context.Data.TryGetValue(ToDoListItem, out object value))
                    {
                        toDoList = (ToDoList)value;
                        var toDoListId = toDoList.Id;

                        if (callbackData == "yes")
                        {
                            await _toDoService.GetByUserIdAndList(guidUserId, toDoListId, ct);
                            await _toDoListService.Delete(toDoListId, ct);
                        }
                        else if (callbackData == "no")
                        {
                            await bot.SendMessage(update.CallbackQuery.Id, "Удаление отменено", cancellationToken: ct);
                        }
                        return ScenarioResult.Completed;
                    }
                    else
                    {
                        await bot.SendMessage(update.CallbackQuery.Id, "Лист не найден.", cancellationToken: ct);
                        throw new Exception("Лист не найден.");
                    }
                default:
                    throw new Exception("Неизвестный шаг сценария");
            }
        }
    }
}

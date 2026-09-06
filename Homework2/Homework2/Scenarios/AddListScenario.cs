using Homework2.Core.Entities;
using Homework2.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Homework2.Scenarios
{
    internal class AddListScenario : IScenario
    {
        private const string User = "user";

        private IUserService _userService;
        private IToDoListService _toDoListService;

        public AddListScenario(IUserService userService, IToDoListService toDoListService)
        {
            _userService = userService;
            _toDoListService = toDoListService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddList;
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
                    await bot.SendMessage(
                        update.Message.Chat.Id, 
                        "Введите название списка:", 
                        cancellationToken: ct);
                    context.CurrentStep = "Name";

                    return ScenarioResult.Transition;

                case "Name":
                    var name = update.Message.Text;
                    await _toDoListService.Add(user, name, ct);

                    return ScenarioResult.Completed;
                default:
                    throw new Exception("Неизвестный шаг сценария.");
            }
        }
    }
}

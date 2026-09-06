using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Homework2
{
    public interface IScenario
    {
        /// <summary>
        /// Проверяет, может ли текущий сценарий обрабатывать указанный тип сценария.
        /// Используется для определения подходящего обработчика в системе сценариев.
        /// </summary>
        /// <param name="scenario"></param>
        /// <returns></returns>
        bool CanHandle(ScenarioType scenario);

        //Обрабатывает входящее сообщение от пользователя в рамках текущего сценария.
        //Включает основную бизнес-логику
        Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct);
    }
}

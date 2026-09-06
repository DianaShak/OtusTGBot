using Homework2.Core.Entities;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace Homework2.TelegramBot
{
    internal class ApproveKeyboardHelper
    {
        public static InlineKeyboardMarkup GetApproveKeyboard(IReadOnlyList<ToDoList> userLists)
        {
            var keyboardButtons = new List<List<InlineKeyboardButton>>();

            keyboardButtons.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("✅Да", "yes"),
                InlineKeyboardButton.WithCallbackData("❌Нет", "no")
            });

            return new InlineKeyboardMarkup(keyboardButtons);
        }
    }
}

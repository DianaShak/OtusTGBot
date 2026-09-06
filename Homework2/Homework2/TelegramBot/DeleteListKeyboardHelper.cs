using Homework2.Core.Entities;
using Homework2.TelegramBot.DTO;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace Homework2.TelegramBot
{
    public class DeleteListKeyboardHelper
    {
        public static InlineKeyboardMarkup GetListsKeyboard(IReadOnlyList<ToDoList> userLists)
        {
            var keyboardButtons = new List<List<InlineKeyboardButton>>();
            //  Кнопки для существующих списков пользователя
            foreach (var list in userLists)
            {
                var listCallback = new ToDoListCallbackDto("deletelist", list.Id).ToString();
                keyboardButtons.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(list.Name, listCallback)
                });
            }
            return new InlineKeyboardMarkup(keyboardButtons);
        }
    }
}

using Homework2.Core.Entities;
using Homework2.TelegramBot.DTO;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace Homework2.TelegramBot
{
    public class ListKeyboardHelper
    {
        public static InlineKeyboardMarkup GetShowKeyboard(IReadOnlyList<ToDoList> userLists)
        {
            var keyboardButtons = new List<List<InlineKeyboardButton>>();

            //  Кнопка "Без списка"
            var noListCallback = new ToDoListCallbackDto ("show", null).ToString();
            keyboardButtons.Add(new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData("📌Без списка", noListCallback)
        });

            //  Кнопки для существующих списков пользователя
            foreach (var list in userLists)
            {
                var listCallback = new ToDoListCallbackDto ("show", list.Id).ToString();
                keyboardButtons.Add(new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData(list.Name, listCallback)
                });
            }

            //  Нижний ряд
            keyboardButtons.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("🆕Добавить", "addlist"),
                InlineKeyboardButton.WithCallbackData("❌Удалить", "deletelist")
            });

            return new InlineKeyboardMarkup(keyboardButtons);
        }
    }
}

using System;

namespace Homework2
{
    public class ToDoUser
    {
        public Guid UserId { get; set; }

        public string TelegramUserName { get; set; }

        public DateTime RegisteredAt { get; set; }

        public long TelegramUserId { get; set; }


        public ToDoUser(string telegramUserName, long telegramUserId)
        {
            telegramUserName = TelegramUserName;
            RegisteredAt = DateTime.UtcNow;
            UserId = Guid.NewGuid();
            telegramUserId = TelegramUserId;
        }
    }
}

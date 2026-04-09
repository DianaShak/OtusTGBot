using System;
using System.Security.Cryptography.X509Certificates;

namespace Homework2
{
    public class ToDoUser
    {
        
        public Guid UserId { get; set; }
        public string TelegramUserName { get; set; }
        public DateTime RegisteredAt { get; set; }
        public ToDoUser(string telegramUserName)
        {
            telegramUserName = TelegramUserName;
            RegisteredAt = DateTime.UtcNow;
            UserId = Guid.NewGuid();
        }
            
        
    }

}

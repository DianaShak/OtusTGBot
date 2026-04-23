using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Homework2
{
    public class UserService : IUserService
    {
        private readonly Dictionary<long, ToDoUser> _users = new();

        public ToDoUser? GetUser(long telegramUserId)
        {
            if (_users.TryGetValue(telegramUserId, out var user))
            {  
                return user;
            }
            else
            {
                return null;
            }
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            ToDoUser user = new(telegramUserName, telegramUserId);
            _users.Add(telegramUserId, user);
            return user;
        }
    }
}

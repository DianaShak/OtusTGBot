using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Homework2
{
    internal class InMemoryUserRepository : IUserRepository
    {
        private List<ToDoUser> _userRepo = new();

        public void Add(ToDoUser user)
        {
            _userRepo.Add(user);
        }

        public ToDoUser? GetUser(Guid userId)
        {
            var foundUser = _userRepo.FirstOrDefault(item => item.UserId == userId);
            return foundUser;
        }

        public ToDoUser? GetUserByTelegramUserId(long telegramUserId)
        {
            var foundUser = _userRepo.FirstOrDefault(item => item.TelegramUserId == telegramUserId);
            return foundUser;
        }
    }
}

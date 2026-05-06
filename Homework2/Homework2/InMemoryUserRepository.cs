using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Homework2
{
    internal class InMemoryUserRepository : IUserRepository
    {
        private List<ToDoUser> _userRepo = new();

        public async Task Add(ToDoUser user, CancellationToken ct)
        {
            _userRepo.Add(user);
            return;
        }

        public async Task<ToDoUser?> GetUser(Guid userId, CancellationToken ct)
        {
            var foundUser = _userRepo.FirstOrDefault(item => item.UserId == userId);
            return foundUser;
        }

        public async Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            var foundUser = _userRepo.FirstOrDefault(item => item.TelegramUserId == telegramUserId);
            return foundUser;

        }
    }
}


using Homework2.Core.Entities;
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

        public async Task<ToDoUser> GetUser(Guid userId, CancellationToken ct)
        {
            return _userRepo.First(item => item.UserId == userId);
        }

        public async Task<ToDoUser> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            return _userRepo.First(item => item.TelegramUserId == telegramUserId);
        }
    }
}

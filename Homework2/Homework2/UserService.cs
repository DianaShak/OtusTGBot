using System;
using System.Threading;
using System.Threading.Tasks;

namespace Homework2
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ToDoUser> GetUser(long telegramUserId, CancellationToken ct)
        {
            return await _userRepository.GetUserByTelegramUserId(telegramUserId, ct);
        }

        public async Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken ct)
        {
            ToDoUser user = new(telegramUserName, telegramUserId);
            await _userRepository.Add(user, ct);
            return user;
        }
    }
}

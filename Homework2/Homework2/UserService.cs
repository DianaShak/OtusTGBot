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

        public async Task<ToDoUser?> GetUser(long telegramUserId, CancellationToken ct)
        {
            var foundedUser = _userRepository.GetUserByTelegramUserId(telegramUserId);
            return foundedUser;
        }

        public async Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken ct)
        {
            ToDoUser user = new(telegramUserName, telegramUserId);
            _userRepository.Add(user);
            return user;
        }
    }
}

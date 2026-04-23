using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework2
{
    public interface IUserService
    {
        ToDoUser RegisterUser(long telegramUserId, string telegramUserName);
        ToDoUser? GetUser(long telegramUserId);
    }
}

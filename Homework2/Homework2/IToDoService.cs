using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework2
{
    public interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
        //Возвращает ToDoItem для UserId со статусом Active
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);

        IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix);

        ToDoItem Add(ToDoUser user, string name);
        void MarkCompleted(Guid taskId);
        void Delete(Guid taskId);
    }
}

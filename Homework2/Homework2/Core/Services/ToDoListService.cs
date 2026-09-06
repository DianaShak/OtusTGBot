using Homework2.Core.DataAcsess;
using Homework2.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Homework2.Core.Services
{
    public class ToDoListService : IToDoListService
    {

        private readonly IToDoListRepository _toDoListRepository;
        public async Task<ToDoList> Add(ToDoUser user, string name, CancellationToken ct)
        {
            if (name.Length > 10)
            {
                throw new ArgumentException("Название списка превысило лимит в 10 символов.");
            }

            if (await _toDoListRepository.ExistsByName(user.UserId, name, ct)) 
            {
                throw new ArgumentException("Список с таким названием уже существует");
            }
            ToDoList newList = new ToDoList(name, user);
            await _toDoListRepository.Add(newList, ct);
            return newList;
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            await _toDoListRepository.Delete(id, ct);
            return;
        }

        public async Task<ToDoList?> Get(Guid id, CancellationToken ct)
        {
            var list = await _toDoListRepository.Get(id, ct);
            if (list == null)
            {
                throw new ArgumentException("Список задач не существует.");
            }
            return list;

        }

        public async Task<IReadOnlyList<ToDoList>> GetUserLists(Guid userId, CancellationToken ct)
        {
            var lists = await _toDoListRepository.GetByUserId(userId, ct);
            if (!lists.Any())
            {
                throw new ArgumentException("У Вас еще нет списков задач.");
            }

            return lists;
        }
    }
}

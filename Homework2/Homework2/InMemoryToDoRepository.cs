using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework2
{
    internal class InMemoryToDoRepository : IToDoRepository
    {
        private readonly List<ToDoItem> _itemRepo = new();

        public void Add(ToDoItem item)
        {
            _itemRepo.Add(item);
        }

        public int CountActive(Guid userId)
        {
            return _itemRepo.Count(i => i.State == ToDoItemState.Active && i.User.UserId == userId);
        }

        public void Delete(Guid id)
        {
            var foundTask = _itemRepo.FirstOrDefault(t => t.Id == id);
            if (foundTask != null)
            {
                _itemRepo.Remove(foundTask);
            }
            else
            {
                throw new Exception($"Задача {id} не найдена.");
            }
        }

        public bool ExistsByName(Guid userId, string name)
        {
            return _itemRepo.Any(i => i.Name == name && i.User.UserId == userId);
        }

        public IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate)
        {
            return _itemRepo.Where(i => i.User.UserId == userId).Where(predicate).ToList();

        }

        public ToDoItem? Get(Guid id)
        {
            return _itemRepo.FirstOrDefault(x => x.Id == id);
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return _itemRepo.Where(i => i.State == ToDoItemState.Active && i.User.UserId == userId).ToList();
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return _itemRepo.Where(i => i.User.UserId == userId).ToList();
        }

        public void Update(ToDoItem item)
        {
            var foundItem = _itemRepo.First(i => i.Id == item.Id);
            foundItem.State = item.State;
            foundItem.User = item.User;
            foundItem.StateChangedAt = item.StateChangedAt;
            foundItem.CreatedAt = item.CreatedAt;
            foundItem.Name = item.Name;
        }
    }
}

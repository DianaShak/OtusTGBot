
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Homework2
{
    internal class InMemoryToDoRepository : IToDoRepository
    {
        private readonly List<ToDoItem> _itemRepo = new();

        public async Task Add(ToDoItem item, CancellationToken ct)
        {
            _itemRepo.Add(item);
            return;
        }

        public async Task<int> CountActive(Guid userId, CancellationToken ct)
        {
            var countActive = _itemRepo.Count(i => i.State == ToDoItemState.Active && i.User.UserId == userId);
            return countActive;
        }

        public async Task Delete(Guid id, CancellationToken ct)
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
            return;
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            var existTask = _itemRepo.Any(i => i.Name == name && i.User.UserId == userId);
            return existTask;
        }

        public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            var foundTask = _itemRepo.Where(i => i.User.UserId == userId).Where(predicate).ToList();
            return foundTask;

        }

        public async Task<ToDoItem?> Get(Guid id, CancellationToken ct)
        {
            return _itemRepo.FirstOrDefault(x => x.Id == id);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            return _itemRepo.Where(i => i.State == ToDoItemState.Active && i.User.UserId == userId).ToList();
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
            return _itemRepo.Where(i => i.User.UserId == userId).ToList();
        }

        public async Task Update(ToDoItem item, CancellationToken ct)
        {
            var foundItem = _itemRepo.First(i => i.Id == item.Id);
            foundItem.State = item.State;
            foundItem.User = item.User;
            foundItem.StateChangedAt = item.StateChangedAt;
            foundItem.CreatedAt = item.CreatedAt;
            foundItem.Name = item.Name;
            return;
        }
        
    }
}

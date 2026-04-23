using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework2
{
    internal class ToDoService : IToDoService
    {
        public const int TaskCountLimitMin = 1;
        public const int TaskCountLimitMax = 2;
        public const int TaskLengthLimitMin = 1;
        public const int TaskLengthLimitMax = 2;

        private readonly Dictionary<Guid, List<ToDoItem>> _items = new();

        private int _taskCountLimitMin;
        private int _taskCountLimitMax;
        private int _taskLengthLimitMin;
        private int _taskLengthLimitMax;


        public ToDoService(int taskCountLimitMin, int taskCountLimitMax, int taskLengthLimitMin, int taskLengthLimitMax)
        {
            _taskCountLimitMin = taskCountLimitMin;
            _taskCountLimitMax = taskCountLimitMax;
            _taskLengthLimitMin = taskLengthLimitMin;
            _taskLengthLimitMax = taskLengthLimitMax;
        }

        public ToDoItem Add(ToDoUser user, string name)
        {
            if (!_items.TryGetValue(user.UserId, out var list))
            {
                list = new List<ToDoItem>();
                //_items.Add(user.UserId, newList);
                _items[user.UserId] = list;
            }

            if (list.Count >= _taskCountLimitMax)
            {
                throw new TaskCountLimitException(_taskCountLimitMax);
            }

            Validator.ValidateString(name);
            bool alreadyExist = list.Any(t => t.Name == name);

            if (name.Length < _taskLengthLimitMin || name.Length > _taskLengthLimitMax)
            {
                throw new TaskLengthLimitException(name.Length, _taskLengthLimitMin);
            }

            if (alreadyExist)
            {
                throw new DuplicateTaskException(name);
            }
            ToDoItem newItem = new(user, name);
            list.Add(newItem);
            return newItem;

        }

        public void Delete(Guid userId, Guid taskId)
        {
            //Ищем по id пользователя, из его листа тасков удаляем нужную.
            if (!_items.TryGetValue(userId, out var list) || list.Count <= 0)
            {
                throw new ArgumentException(nameof(userId), "Задач пока нет.");
            }

            var foundTask = list.First(t => t.Id == taskId);
            list.Remove(foundTask);
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            if (_items.TryGetValue(userId, out var list))
            {
                return list.Where(item => item.State == ToDoItemState.Active).ToList();
            }
            else
            {
                return new List<ToDoItem>();
            }
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            if (_items.TryGetValue(userId, out var list))
            {
                return list;
            }
            else
            {
                return new List<ToDoItem>();
            }
        }

        public void MarkCompleted(Guid userId, Guid taskId)
        {
            if (!_items.TryGetValue(userId, out var list))
            {
                return;
            }

            var foundItem = list.FirstOrDefault(item => item.Id == taskId);
            if (foundItem != null)
            {
                foundItem.State = ToDoItemState.Completed;
                foundItem.StateChangedAt = DateTime.UtcNow;
            }
        }
    }
}

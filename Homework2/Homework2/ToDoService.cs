using System;
using System.Collections.Generic;

namespace Homework2
{
    internal class ToDoService : IToDoService
    {
        public const int TaskCountLimitMin = 1;
        public const int TaskCountLimitMax = 100;
        public const int TaskLengthLimitMin = 1;
        public const int TaskLengthLimitMax = 100;

        private readonly IToDoRepository _toDoRepository;
        private int _taskCountLimitMin;
        private int _taskCountLimitMax;
        private int _taskLengthLimitMin;
        private int _taskLengthLimitMax;


        public ToDoService(IToDoRepository toDoRepository, int taskCountLimitMin, int taskCountLimitMax, int taskLengthLimitMin, int taskLengthLimitMax)
        {
            _toDoRepository = toDoRepository;
            _taskCountLimitMin = taskCountLimitMin;
            _taskCountLimitMax = taskCountLimitMax;
            _taskLengthLimitMin = taskLengthLimitMin;
            _taskLengthLimitMax = taskLengthLimitMax;
        }

        public ToDoItem Add(ToDoUser user, string name)
        {
            if (_toDoRepository.ExistsByName(user.UserId, name))
            {
                throw new DuplicateTaskException(name);
            }

            if (_toDoRepository.CountActive(user.UserId) >= _taskCountLimitMax)
            {
                throw new TaskCountLimitException(_taskCountLimitMax);
            }

            Validator.ValidateString(name);

            if (name.Length < _taskLengthLimitMin || name.Length > _taskLengthLimitMax)
            {
                throw new TaskLengthLimitException(name.Length, _taskLengthLimitMin);
            }

            ToDoItem newAddItem = new(user, name);
            _toDoRepository.Add(newAddItem);
            return newAddItem;
        }

        public void Delete(Guid taskId)
        {
            _toDoRepository.Delete(taskId);
        }

        public IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix)
        {
            return _toDoRepository.Find(user.UserId, i => i.Name.StartsWith(namePrefix));
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return _toDoRepository.GetActiveByUserId(userId);
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return _toDoRepository.GetAllByUserId(userId);
        }

        public void MarkCompleted(Guid taskId)
        {
            var foundTask = _toDoRepository.Get(taskId);
            if (foundTask == null)
            {
                throw new Exception($"Задача {taskId} не найдена.");
            }
            foundTask.State = ToDoItemState.Completed;
            foundTask.StateChangedAt = DateTime.UtcNow;
            _toDoRepository.Update(foundTask);
        }
    }
}

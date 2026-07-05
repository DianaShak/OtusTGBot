using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<ToDoItem> Add(ToDoUser user, string name, DateTime deadline, CancellationToken ct)
        {
            if (await _toDoRepository.ExistsByName(user.UserId, name, ct))
            {
                throw new DuplicateTaskException(name);
            }

            if (await _toDoRepository.CountActive(user.UserId, ct) >= _taskCountLimitMax)
            {
                throw new TaskCountLimitException(_taskCountLimitMax);
            }

            Validator.ValidateString(name);

            if (name.Length < _taskLengthLimitMin || name.Length > _taskLengthLimitMax)
            {
                throw new TaskLengthLimitException(name.Length, _taskLengthLimitMin);
            }

            ToDoItem newAddItem = new(user, name, deadline);
            await _toDoRepository.Add(newAddItem, ct);
            return newAddItem;
        }

        public async Task Delete(Guid taskId, CancellationToken ct)
        {
            await _toDoRepository.Delete(taskId, ct);
            return;
        }

        public async Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix, CancellationToken ct)
        {
            var foundedTask = await _toDoRepository.Find(user.UserId, i => i.Name.StartsWith(namePrefix), ct);
            return foundedTask;
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            var list = await _toDoRepository.GetActiveByUserId(userId, ct);
            return list;
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
            var list = await _toDoRepository.GetAllByUserId(userId, ct);
            return list;
        }

        public async Task MarkCompleted(Guid taskId, CancellationToken ct)
        {
            var foundTask = await _toDoRepository.Get(taskId, ct);
            if (foundTask == null)
            {
                throw new Exception($"Задача {taskId} не найдена.");
            }
            foundTask.State = ToDoItemState.Completed;
            foundTask.StateChangedAt = DateTime.UtcNow;
            await _toDoRepository.Update(foundTask, ct);
            return;
        }
    }
}

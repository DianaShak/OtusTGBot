using Homework2.Core.DataAcsess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Homework2.Core.Services
{
    internal class ToDoReportService : IToDoReportService
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IUserRepository _userRepository;
        public ToDoReportService(IToDoRepository toDoRepository, IUserRepository userRepository, CancellationTokenSource ct)
        {
            _toDoRepository = toDoRepository;
            _userRepository = userRepository;
        }
        public async Task<(int total, int completed, int active, DateTime generatedAt)> GetUserStats(Guid userId, CancellationToken ct)
        {
            //Статистика по задачам на 01.01.2025 00:00:00. Всего: 10; Завершенных: 7; Активных: 3;
            //var user = _userRepository.GetUser(userId);
            var list = await _toDoRepository.GetAllByUserId(userId, ct);
            var total = list.Count;
            var active = await _toDoRepository.CountActive(userId, ct);
            var completed = total - active;
            var timeAndDate = DateTime.UtcNow;
            return (total, completed, active, timeAndDate);
        }
    }
}

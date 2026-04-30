using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework2
{
    internal class ToDoReportService : IToDoReportService
    {
        private readonly IToDoRepository _toDoRepository;
        private readonly IUserRepository _userRepository;
        public ToDoReportService(IToDoRepository toDoRepository, IUserRepository userRepository)
        {
            _toDoRepository = toDoRepository;
            _userRepository = userRepository;
        }
        public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
        {
            //Статистика по задачам на 01.01.2025 00:00:00. Всего: 10; Завершенных: 7; Активных: 3;
            //var user = _userRepository.GetUser(userId);
            var list = _toDoRepository.GetAllByUserId(userId);
            var total = list.Count;
            var active = _toDoRepository.CountActive(userId);
            var completed = total - active;
            var timeAndDate = DateTime.UtcNow;
            return (total, completed, active, timeAndDate);
        }
    }
}

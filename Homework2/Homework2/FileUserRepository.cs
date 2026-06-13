using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Homework2
{
    internal class FileUserRepository : IUserRepository
    {
        private readonly string _baseDir;

        public FileUserRepository(string baseDirPath)
        {
            if (!Directory.Exists(baseDirPath))
            {
                Directory.CreateDirectory(baseDirPath);
            }
            _baseDir = baseDirPath;
        }

        public async Task Add(ToDoUser user, CancellationToken ct)
        {
            var jsonItem = JsonSerializer.Serialize(user);
            var fileName = $"{user.UserId}.json";
            await File.WriteAllTextAsync(Path.Combine(_baseDir, fileName), jsonItem);
            //await File.WriteAllTextAsync(Path.Combine(_baseDir, item.User.UserId.ToString(), fileName), jsonItem);
        }

        public async Task<ToDoUser> GetUser(Guid userId, CancellationToken ct)
        {
            var fileName = $"{userId}.json";
            var fullFilePath = Path.Combine(_baseDir, fileName);

            if (File.Exists(fullFilePath))
            {
                using var jsonStream = File.OpenRead(fullFilePath);
                var user = await JsonSerializer.DeserializeAsync<ToDoUser>(jsonStream);
                return user;
            }

            throw new Exception($"Пользователь {userId} не найден.");
        }

        public async Task<ToDoUser> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            var fileNames = Directory.GetFiles(_baseDir, "*.json");
            var foundTasks = new List<ToDoItem>();

            foreach (var file in fileNames)
            {
                using var jsonStream = File.OpenRead(file);
                var user = await JsonSerializer.DeserializeAsync<ToDoUser>(jsonStream)!;
                if (user.TelegramUserId == telegramUserId)
                {
                    return user;
                }
            }

            throw new Exception($"Пользователь {telegramUserId} не найден.");
            //return _userRepo.First(item => item.TelegramUserId == telegramUserId);
        }
    }
}

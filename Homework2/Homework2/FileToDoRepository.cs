using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Homework2
{
    internal class FileToDoRepository : IToDoRepository
    {
        private const string IndexFileName = "index.json";

        private readonly string _baseDir;
        private readonly string _indexFile;

        public FileToDoRepository(string baseDirPath) 
        {
            _baseDir = baseDirPath;
            _indexFile = Path.Combine(_baseDir, IndexFileName);
            if (!Directory.Exists(baseDirPath))
            {
                Directory.CreateDirectory(baseDirPath);
                SaveIndex(new Dictionary<Guid, Guid>());
            }
            else
            {
                if (!File.Exists(_indexFile))
                {
                    CreateIndex();
                }
            }
        }

        public async Task Add(ToDoItem item, CancellationToken ct)
        {
            var fileName = $"{item.Id}.json";
            //await File.WriteAllTextAsync(Path.Combine(_baseDir, fileName), jsonItem);
            var userDir = Path.Combine(_baseDir, item.User.UserId.ToString());  //  Формируем путь к персональной папке
            Directory.CreateDirectory(userDir);  //  Создаем папку пользователя
            var filePath = Path.Combine(userDir, fileName);  //  Путь к файлу задачи
            var indexFile = await LoadIndex(_indexFile);
            indexFile[item.Id] = item.User.UserId;  //  Записываем в индекс-файл задачу
            var jsonIndex = JsonSerializer.Serialize(indexFile);
            var jsonItem = JsonSerializer.Serialize(item);
            await File.WriteAllTextAsync(filePath, jsonItem);
            await File.WriteAllTextAsync(_indexFile, jsonIndex);
        }

        public async Task<int> CountActive(Guid userId, CancellationToken ct)
        {
            int countActive = 0;
            var fileNames = Directory.GetFiles(_baseDir, userId.ToString());

            foreach (var file in fileNames)
            {
                //var json = await File.ReadAllTextAsync(file, ct);
                using var jsonStream = File.OpenRead(file);
                var item = await JsonSerializer.DeserializeAsync<ToDoItem>(jsonStream)!;
                if (item.State == ToDoItemState.Active)
                {
                    countActive++;
                }
            }
            return countActive;
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            var dictionary = await LoadIndex(_indexFile);  //  Получаем десериализованный словарь
            dictionary.TryGetValue(id, out Guid userId);  //  Находим user id по id задачи
            var filePath = Path.Combine(_baseDir, userId.ToString(), id.ToString());
            File.Delete(filePath);  //  удаляем файл задачи
            dictionary.Remove(id);  //  удаляем задачу из индекс-файла
            SaveIndex(dictionary);

            //throw new Exception($"Задача {id} не найдена.");
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            var fileNames = Directory.GetFiles(_baseDir, userId.ToString());

            foreach (var file in fileNames)
            {
                //var json = await File.ReadAllTextAsync(file, ct);
                using var jsonStream = File.OpenRead(file);
                var item = await JsonSerializer.DeserializeAsync<ToDoItem>(jsonStream)!;
                if (item.Name == name && item.User.UserId == userId)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            var userFilesPath = Path.Combine(_baseDir, userId.ToString());
            var userFiles = Directory.GetFiles(userFilesPath, "*.json");
            var foundTasks = new List<ToDoItem>();

            foreach (var userFile in userFiles)
            {
                using var jsonStream = File.OpenRead(userFile);
                var item = await JsonSerializer.DeserializeAsync<ToDoItem>(jsonStream);

                if (predicate(item))
                {
                    foundTasks.Add(item);
                }
            }
            return foundTasks;
        }

        public async Task<ToDoItem> Get(Guid id, CancellationToken ct)
        {
            var dictionary = await LoadIndex(_indexFile);  //  Получаем десериализованный словарь
            dictionary.TryGetValue(id, out Guid userId);  //  Находим user id по id задачи
            var fullFilePath = Path.Combine(_baseDir, userId.ToString());

            if (File.Exists(fullFilePath))
            {
                //using var reader = new StreamReader(fullFilePath);
                //var json2 = await reader.ReadToEndAsync();
                //var json = await File.ReadAllTextAsync(fullFilePath, ct);
                using var jsonStream = File.OpenRead(fullFilePath);
                var item = await JsonSerializer.DeserializeAsync<ToDoItem>(jsonStream);
                return item;
            }

            throw new Exception($"Задача {id} не найдена.");
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            return await Find(userId, i => i.State == ToDoItemState.Active, ct);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
            return await Find(userId, i => true, ct);
        }

        public async Task Update(ToDoItem item, CancellationToken ct)
        {
            //var foundItem = await Get(item.Id, ct);
            //foundItem.State = item.State;
            //foundItem.User = item.User;
            //foundItem.StateChangedAt = item.StateChangedAt;
            //foundItem.CreatedAt = item.CreatedAt;
            //foundItem.Name = item.Name;
            await Add(item, ct);
        }
        
        private void SaveIndex(Dictionary<Guid, Guid> indexDictionary)
        {
            var jsonDictionary = JsonSerializer.Serialize(indexDictionary);
            File.WriteAllText(_indexFile, jsonDictionary);
        }

        private void CreateIndex()
        {
            var index = new Dictionary<Guid, Guid>();
            var userDirs = Directory.GetDirectories(_baseDir);
            foreach (var userDir in userDirs)
            {
                var userDirName = Path.GetDirectoryName(userDir);
                var userId = Guid.Parse(userDirName);
                var taskFiles = Directory.GetFiles(userDir);
                
                foreach (var taskFile in taskFiles)
                {
                    var taskFileName = Path.GetFileName(taskFile);
                    var taskId = Guid.Parse(taskFileName);
                    index[taskId] = userId;
                }
            }
            SaveIndex(index);
        }

        private async Task<Dictionary<Guid, Guid>> LoadIndex(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new Dictionary<Guid, Guid>();
            }
            using var jsonStream = File.OpenRead(filePath);
            var dictionary = await JsonSerializer.DeserializeAsync<Dictionary<Guid, Guid>>(jsonStream)!;
            return dictionary;
        }
    }
}

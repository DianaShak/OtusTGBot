using Homework2.Core.DataAcsess;
using Homework2.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Homework2.Infrastructure.DataAcsess.Repositories
{
    public class FileToDoListRepository : IToDoListRepository
    {
        private const string IndexFileName = "index.json";

        private readonly string _baseDir;
        private readonly string _indexFile;

        public FileToDoListRepository(string baseDirPath)
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
        public async Task Add(ToDoList list, CancellationToken ct)
        {
            var fileName = $"{list.Id}.json";
            var userDir = Path.Combine(_baseDir, list.User.UserId.ToString());  //  Формируем путь к персональной папке пользователя
            Directory.CreateDirectory(userDir);  //  Создаем папку пользователя
            
            var listDir = Path.Combine(userDir, list.Id.ToString());  //  Формируем путь к персональной папке листа
            Directory.CreateDirectory(listDir);  //  Создаем папку лист пользователя
            var jsonItemList = JsonSerializer.Serialize(list);
            await File.WriteAllTextAsync(Path.Combine(_baseDir, fileName), jsonItemList);

            var indexFile = await LoadIndex(_indexFile, ct);
            indexFile[list.Id] = list.User.UserId;  //  Записываем в индекс-файл список
            var jsonIndex = JsonSerializer.Serialize(indexFile);
            await File.WriteAllTextAsync(_indexFile, jsonIndex);
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            var dictionary = await LoadIndex(_indexFile, ct);  //  Получаем десериализованный словарь
            
            dictionary.TryGetValue(id, out Guid userId);  //  Находим user id по id задачи
            var filePath = Path.Combine(_baseDir, userId.ToString(), id.ToString());  //  Путь до списка
            
            File.Delete(filePath);  //  удаляем файл списка
            dictionary.Remove(id);  //  удаляем список из индекс-файла
            await SaveIndex(dictionary);
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            var path = Path.Combine(_baseDir, userId.ToString());
            var fileNames = Directory.GetFiles(path, "*.json");
            
            foreach (var file in fileNames)
            {
                using (var jsonStream = File.OpenRead(file))
                {
                    var item = await JsonSerializer.DeserializeAsync<ToDoList>(jsonStream)!;
                    if (item.Name == name && item.User.UserId == userId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<ToDoList?> Get(Guid id, CancellationToken ct)
        {
            var fileName = $"{id}.json";
            var dictionary = await LoadIndex(_indexFile, ct);  //  Получаем десериализованный словарь
            dictionary.TryGetValue(id, out Guid userId);  //  Находим user id по id списка
            var fullFilePath = Path.Combine(_baseDir, userId.ToString(), fileName);

            if (File.Exists(fullFilePath))
            {
                using var jsonStream = File.OpenRead(fullFilePath);
                var list = await JsonSerializer.DeserializeAsync<ToDoList>(jsonStream);
                return list;
            }
            return null;
        }

        public async Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken ct)
        {
            var userFilesPath = Path.Combine(_baseDir, userId.ToString());
            var userFiles = Directory.GetFiles(userFilesPath, "*.json");
            var foundLists = new List<ToDoList>();

            foreach (var userFile in userFiles)
            {
                using (var jsonStream = File.OpenRead(userFile))
                {
                    var list = await JsonSerializer.DeserializeAsync<ToDoList>(jsonStream);
                    foundLists.Add(list);
                }
            }
            return foundLists;
        }

        private async Task SaveIndex(Dictionary<Guid, Guid> indexDictionary)
        {
            var jsonDictionary = JsonSerializer.Serialize(indexDictionary);
            File.WriteAllText(_indexFile, jsonDictionary);
        }

        private async Task CreateIndex()
        {
            var index = new Dictionary<Guid, Guid>();
            var userDirs = Directory.GetDirectories(_baseDir);
            foreach (var userDir in userDirs)
            {
                var userDirName = Path.GetDirectoryName(userDir);
                var userId = Guid.Parse(userDirName);
                var listFiles = Directory.GetFiles(userDir);

                foreach (var listFile in listFiles)
                {
                    var listFileName = Path.GetFileName(listFile);
                    var listId = Guid.Parse(listFileName);
                    index[listId] = userId;
                }
            }
            await SaveIndex(index);
        }

        private async Task<Dictionary<Guid, Guid>> LoadIndex(string filePath, CancellationToken ct)
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

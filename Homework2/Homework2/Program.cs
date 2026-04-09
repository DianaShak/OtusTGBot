using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Homework2
{
    internal class Program
    {
        static ToDoUser user;
        
        const string ProgrammVersionInfo = "версия 1.0, 17.02.2026";
        
        public static void ShowAllTasks(List<ToDoItem> tasks, bool showAll)
        {
            int taskIndex = 1;
            if (tasks?.Count > 0)
            {
                foreach (var task in tasks)
                {
                    if (showAll)
                    {
                        Console.WriteLine($"{taskIndex++} {task.State} {task.Name} {task.CreatedAt} {task.Id}");
                    }
                    else if (task.State == ToDoItemState.Active)
                    {
                        Console.WriteLine($"{taskIndex++} {task.Name} {task.CreatedAt} {task.Id}");
                    }
                } 
            }
            else 
            {
                Console.WriteLine("Список пуст.");
            }
        }

        static void Main(string[] args)
        {
            
            Console.WriteLine("Выберите нужную команду!");
            Console.WriteLine(@"/start /help /info /echo /addtask /completetask /showtasks /showalltasks /removetask /exit");
            var input = string.Empty;
            var name = string.Empty;
            var tasks = new List<ToDoItem>();
            int taskCountLimit = 0;
            int taskLengthLimit = 0;
            string taskLength = string.Empty;
            int min = 1;
            int max = 100;

            //if (splitInput[0] != "/start" || splitInput[0] != "/help" || splitInput[0] != "/info" || splitInput[0] != "/echo" || splitInput[0] != "/addtask" || splitInput[0] != "/showtasks" || splitInput[0] != "/removetask" || splitInput[0] != "/exit")
            //{
            //    Console.WriteLine("Такой команды нет. Пожалуйста, введите команду из предложенных:");
            //    Console.WriteLine(@"/start /help /info /echo /addtask /showtasks /removetask /exit");
            //    continue;
            //}

            while (input != "/exit")
            {
                input = Console.ReadLine();
                var splitInput = input?.Split(' ');
                if (splitInput == null || splitInput.Length < 1)
                {
                    Console.WriteLine("Такой команды нет. Пожалуйста, введите команду из предложенных:");
                    Console.WriteLine(@"/start /help /info /echo /addtask /showtasks /removetask /exit");
                    continue;
                }

                try
                {
                    
                    if (taskCountLimit == 0)
                    {
                        Console.WriteLine("Введите максимально допустимое количество задач (от 1 до 100):");
                        var maxNumber = Console.ReadLine();
                        taskCountLimit = ParseAndValidateInt(maxNumber);
                        Console.WriteLine($"Максимальное количество задач: {taskCountLimit}");
                    }

                    if (taskLengthLimit == 0)
                    {
                        Console.WriteLine("Введите максимально допустимую длину задачи (от 1 до 100):");
                        taskLength = Console.ReadLine();
                        taskLengthLimit = ParseAndValidateInt(taskLength);
                        Console.WriteLine($"Максимальная длина задачи: {taskLengthLimit}");
                    }

                    switch (splitInput[0])
                    {
                        case "/start":
                            //  Запрашиваем имя пользователя.
                            var userName = GetNameMethod();
                            ToDoUser user = new ToDoUser(name);  //  1 пункт
                            Console.WriteLine($"Здравствуйте, {userName}! Чем я могу помочь?");
                            break;

                        case "/help":
                            //  Отображает краткую справочную информацию о том, как пользоваться программой.
                            HelpMethod();
                            break;

                        case "/info":
                            //  Предоставляет информацию о версии программы и дате её создания.
                            InfoMethod();
                            break;

                        case "/echo":
                            //  После ввода имени становится доступной команда /echo.
                            //  При вводе этой команды с аргументом (например, /echo Hello), программа возвращает введенный текст (в данном примере "Hello").
                            EchoMethod(splitInput);
                            break;

                        case "/addtask":
                            AddTaskMethod(tasks, taskCountLimit, taskLengthLimit);
                            break;

                        case "/showtasks":
                            ShowTasksMethod(tasks);
                            break;

                        case "/removetask":
                            RemoveTaskMethod(tasks);
                            break;
                        case "/completetask":
                            //Найти задачу по Id
                            //Обновить State на ToDoItemState.Completed
                            //Обновить StateChangedAt
                            //Пример: / completetask 73c7940a - ca8c - 4327 - 8a15 - 9119bffd1d5e
                            var idToFind = string.Join(' ', splitInput[1..]);
                            ValidateString(idToFind);
                            if (Guid.TryParse(idToFind, out Guid idSearch));
                            {
                                ToDoItem foundTask = tasks.Find(t => t.Id == idSearch);
                                foundTask.State = ToDoItemState.Completed;
                                foundTask.StateChangedAt = DateTime.UtcNow;
                                Console.WriteLine("Задача завершена.");
                            }
                            break;
                        case "/showalltasks":
                            //Добавить обработку новой команды / showalltasks.По ней выводить команды с любым State и добавить State в вывод
                            //Пример: (Active)Имя задачи - 01.01.2025 00:00:00 - ffbfe448 - 4b39 - 4778 - 98aa - 1aed98f7eed8
                            ShowAllTasks(tasks, true);
                            break;

                        case "/exit":
                            break;

                        default:
                           
                            break;
                    }
                }
                catch (TaskCountLimitException e)
                {
                    Console.WriteLine($"Исключение: {e.Message}");
                }
                catch (TaskLengthLimitException e)
                {
                    Console.WriteLine($"Исключение: {e.Message}");
                }
                catch (DuplicateTaskException e)
                {
                    Console.WriteLine($"Исключение: {e.Message}");
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine($"Ошибка: {e.Message}");
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Произошла непредвиденная ошибка:");
                    Console.WriteLine($"Тип: {exp.GetType().FullName}");
                    Console.WriteLine($"Исключение: {exp.Message}");
                    Console.WriteLine($"Трассировка стека: {exp.StackTrace}");
                    Console.WriteLine($"Информация об исключении: {exp.InnerException}");
                }
            }
        }

        private static void EchoMethod(string[] splitInput)
        {
            Console.WriteLine($"{((user == null) ? "Пожалуйста, начните с команды /start." : $"{string.Join(' ', splitInput[1..])}")}");
        }

        private static void InfoMethod()
        {
            Console.WriteLine($"{((user == null) ? "Пожалуйста, начните с команды /start." : $"{user?.TelegramUserName}, версия и дата создания: {ProgrammVersionInfo}")}");
        }

        private static void HelpMethod()
        {
            Console.WriteLine($"{((user == null) ? "Пожалуйста, начните с команды /start." : $"{user?.TelegramUserName},\nкоманда /start запрашивает имя и открывает доступ к команде /echo,\nкоманда /help позволяет получить краткую справочную информацию о том, как пользоваться программой,\nкоманда /info позволяет получить информацию о версии программы и дате её создания,\nкоманда /echo позволяет вывести введенный Вами текст после команды,\nкоманда /addtask позволяет добавлять задачи, \nкоманда /completetask меняет статус задачи на completed, \nкоманда /showtasks выводит список введенных задач со статусом Active, \nкоманда /showalltasks выводит список всех введенных задач, \nкоманда /removetask позволяет удалить определенную задачу,\nкоманда /exit позволяет выйти из меню.")}");
        }

        private static void RemoveTaskMethod(List<ToDoItem> tasks)
        {
            bool valid = false;
            do
            {
                if (tasks != null && tasks.Count > 0)
                {
                    Console.WriteLine("Введите номер задачи, которую хотите удалить:");
                    ShowAllTasks(tasks, true);
                    var taskNumberToRemove = Console.ReadLine();
                    int taskNumber;

                    if (!int.TryParse(taskNumberToRemove, out taskNumber))
                    {
                        Console.WriteLine("Число не распознано.");
                        continue;
                    }
                    int indexToRemove = taskNumber - 1;

                    if (indexToRemove >= 0 && indexToRemove < tasks.Count)
                    {
                        valid = true;
                        var removedTaskName = tasks[taskNumber - 1];
                        tasks.RemoveAt(indexToRemove);
                        Console.WriteLine($"Задача '{removedTaskName}' удалена.");
                    }
                    else
                    {
                        Console.WriteLine("Элемент с таким номером не существует. Пожалуйста, введите корректный номер.");
                    }
                }
                else
                {
                    Console.WriteLine("Список пуст.");
                }
            } while (!valid);
        }

        private static void ShowTasksMethod(List<ToDoItem> tasks)
        {
            if (tasks != null && tasks.Count > 0)
            {
                Console.WriteLine("Вот Ваш список задач:");
                ShowAllTasks(tasks, false);
            }
            else
            {
                Console.WriteLine("Список пуст.");
            }
        }

        private static void AddTaskMethod(List<ToDoItem> tasks, int taskCountLimit, int taskLengthLimit)
        {
            bool isValid = false;
            if (tasks.Count >= taskCountLimit)
            {
                throw new TaskCountLimitException(taskCountLimit);
            }

            do
            {
                Console.WriteLine("Пожалуйста, введите описание задачи:");
                var task = Console.ReadLine();
                int length = task.Length;
                ValidateString(task);
                bool alreadyExist = tasks.Any(t => t.Name == task);

                if (task.Length > taskLengthLimit)
                {
                    throw new TaskLengthLimitException(task.Length, taskLengthLimit);
                }
                else if (alreadyExist)
                {
                    throw new DuplicateTaskException(task);
                }
                else
                {
                    isValid = true;
                    tasks.Add(new ToDoItem(user, task));  //  2 пункт
                    Console.WriteLine($"Задача '{task}' добавлена.");  //  2 пункт
                }
            } while (!isValid);
        }

        public static string? GetNameMethod()
        {
            Console.WriteLine("Пожалуйста, введите свое имя:");
            var name = Console.ReadLine();
            ValidateString(name);
            
            return name;
        }

        private static int ParseAndValidateInt(string? str, int min = 1, int max = 100)
        {
            int strInt = 0;
            if (!int.TryParse(str, out strInt))
            {
                throw new ArgumentException("Введено не число.");
            }
            if (strInt > max || strInt < min)
            {
                throw new ArgumentException("Длинна введенных данных не соответсвует диапазону (от 1 до 100).");
            }
            return strInt;
        }

        private static string ValidateString(string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Вы ничего не ввели.");
            }
            else
            {
                return str;
            }
        }
    }
}
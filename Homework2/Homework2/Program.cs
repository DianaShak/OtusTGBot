using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Homework2
{
    internal class Program
    {
        const string ProgrammVersionInfo = "версия 1.0, 17.02.2026";
        
        public static void ShowAllTasks(List<string> tasks)
        {
            int taskIndex = 1;
            if (tasks != null || tasks.Count > 0)
            {
                foreach (var task in tasks)
                {
                    Console.WriteLine($"{taskIndex++} {task}");
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
            Console.WriteLine(@"/start /help /info /echo /addtask /showtasks /removetask /exit");
            var input = string.Empty;
            var name = string.Empty;
            List<string> tasks = new List<string>();
            int taskCountLimit = 0;
            int taskLengthLimit = 0;
            string taskLength = string.Empty;
            int min = 1;
            int max = 100;
            


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

                        if (!int.TryParse(maxNumber, out taskCountLimit))
                        {
                            throw new ArgumentException("Введено не число.");
                            
                        }
                        else if (taskCountLimit < 1 || taskCountLimit > 100)
                        {
                            throw new ArgumentException($"Число должно быть в диапазоне от 1 до 100. Число {taskCountLimit} не подходит.");

                        }
                        
                        else
                        {
                            Console.WriteLine($"Максимальное количество задач: {taskCountLimit}");
                        }
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
                            name = NameMethod();
                            break;

                        case "/help":
                            //  Отображает краткую справочную информацию о том, как пользоваться программой.
                            HelpMethod(name);
                            break;

                        case "/info":
                            //  Предоставляет информацию о версии программы и дате её создания.
                            InfoMethod(name);
                            break;

                        case "/echo":
                            //  После ввода имени становится доступной команда /echo.
                            //  При вводе этой команды с аргументом (например, /echo Hello), программа возвращает введенный текст (в данном примере "Hello").
                            EchoMethod(name, splitInput);
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

        private static void EchoMethod(string? name, string[] splitInput)
        {
            Console.WriteLine($"{(string.IsNullOrWhiteSpace(name) ? "Пожалуйста, начните с команды /start." : $"{string.Join(' ', splitInput[1..])}")}");
        }

        private static void InfoMethod(string? name)
        {
            Console.WriteLine($"{(string.IsNullOrWhiteSpace(name) ? "Пожалуйста, начните с команды /start." : $"{name}, версия и дата создания: {ProgrammVersionInfo}")}");
        }

        private static void HelpMethod(string? name)
        {
            Console.WriteLine($"{(string.IsNullOrWhiteSpace(name) ? "Пожалуйста, начните с команды /start." : $"{name},\nкоманда /start запрашивает имя и открывает доступ к команде /echo,\nкоманда /help позволяет получить краткую справочную информацию о том, как пользоваться программой,\nкоманда /info позволяет получить информацию о версии программы и дате её создания,\nкоманда /echo позволяет вывести введенный Вами текст после команды,\nкоманда /addtask позволяет добавлять задачи,\nкоманда /showtasks выводит список введенных задач,\nкоманда /removetask позволяет удалить определенную задачу,\nкоманда /exit позволяет выйти из меню.")}");
        }

        private static void RemoveTaskMethod(List<string> tasks)
        {
            bool valid = false;
            do
            {
                if (tasks != null && tasks.Count > 0)
                {
                    Console.WriteLine("Введите номер задачи, которую хотите удалить:");
                    ShowAllTasks(tasks);
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
                        string removedTaskName = tasks[taskNumber - 1];
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

        private static void ShowTasksMethod(List<string> tasks)
        {
            if (tasks != null && tasks.Count > 0)
            {
                Console.WriteLine("Вот Ваш список задач:");
                ShowAllTasks(tasks);
            }
            else
            {
                Console.WriteLine("Список пуст.");
            }
        }

        private static void AddTaskMethod(List<string> tasks, int taskCountLimit, int taskLengthLimit)
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
                string containsTask;
                ValidateString(task);

                if (task.Length >= taskLengthLimit)
                {
                    throw new TaskLengthLimitException(task.Length, taskLengthLimit);
                }
                else if (tasks.Contains(task))
                {
                    throw new DuplicateTaskException(task);
                }
                else
                {
                    isValid = true;
                    tasks.Add(task);

                    Console.WriteLine($"Задача '{task}' добавлена.");
                }
            } while (!isValid);
        }

        private static string? NameMethod()
        {
            string? name;
            Console.WriteLine("Пожалуйста, введите свое имя:");
            name = Console.ReadLine();
            Console.WriteLine($"Здравствуйте, {name}! Чем я могу помочь?");
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

        private static void ValidateString(string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Вы ничего не ввели.");
            }
        }
    }
}
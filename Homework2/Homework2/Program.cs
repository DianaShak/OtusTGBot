using System;
using System.Collections.Generic;

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
            Console.WriteLine("Привет! Выберите нужную команду!");
            Console.WriteLine(@"/start /help /info /echo /addtask /showtasks /removetask /exit");
            var input = string.Empty;
            var name = string.Empty;
            List<string> tasks = new List<string>();

            while (input != "/exit")
            {
                input = Console.ReadLine();
                var splitInput = input?.Split(' ');
                if (splitInput == null || splitInput.Length < 1)
                {
                    continue;
                }

                switch (splitInput[0])
                {
                    case "/start":
                        //  Запрашиваем имя пользователя.
                        Console.WriteLine("Пожалуйста, введите свое имя:");
                        name = Console.ReadLine();
                        Console.WriteLine($"Здравствуйте, {name}! Чем я могу помочь?");
                        break;

                    case "/help":
                        //  Отображает краткую справочную информацию о том, как пользоваться программой.
                        Console.WriteLine($"{(string.IsNullOrWhiteSpace(name) ? "Пожалуйста, начните с команды /start." : $"{name},\nкоманда /start запрашивает имя и открывает доступ к команде /echo,\nкоманда /help позволяет получить краткую справочную информацию о том, как пользоваться программой,\nкоманда /info позволяет получить информацию о версии программы и дате её создания,\nкоманда /echo позволяет вывести введенный Вами текст после команды,\nкоманда /addtask позволяет добавлять задачи,\nкоманда /showtasks выводит список введенных задач,\nкоманда /removetask позволяет удалить определенную задачу,\nкоманда /exit позволяет выйти из меню.")}");
                        break;   

                    case "/info":
                        //  Предоставляет информацию о версии программы и дате её создания.
                        Console.WriteLine($"{(string.IsNullOrWhiteSpace(name) ? "Пожалуйста, начните с команды /start." : $"{name}, версия и дата создания: {ProgrammVersionInfo}")}");
                        break;

                    case "/echo":
                        //  После ввода имени становится доступной команда /echo.
                        //  При вводе этой команды с аргументом (например, /echo Hello), программа возвращает введенный текст (в данном примере "Hello").
                        Console.WriteLine($"{(string.IsNullOrWhiteSpace(name) ? "Пожалуйста, начните с команды /start." : $"{string.Join(' ', splitInput[1..])}")}");
                        break;

                    case "/addtask":
                        Console.WriteLine("Пожалуйста, введите описание задачи:");
                        var task = Console.ReadLine();
                        tasks.Add(task);
                        Console.WriteLine($"Задача {task} добавлена.");
                        break;

                    case "/showtasks":
                        if (tasks != null && tasks.Count > 0)
                        {
                            Console.WriteLine("Вот Ваш список задач:");
                            ShowAllTasks(tasks);
                        }
                        else
                        {
                            Console.WriteLine("Список пуст.");
                        }
                        break;

                    case "/removetask":
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
                                    Console.WriteLine($"Задача {removedTaskName} удалена.");
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
                        break;
                    case "/exit":
                        break;

                    default:
                        break;


                }
            }

        }
    }
}

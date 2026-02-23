using System;

namespace Homework2
{
    internal class Program
    {
        const string ProgrammVersionInfo = "Версия 1.0, 17.02.2026";

        static void Main(string[] args)
        {
            Console.WriteLine("Привет! Выберите нужную команду!");
            Console.WriteLine(@"/start /help /info /exit");
            var input = string.Empty;
            var name = string.Empty;
            var input2 = string.Empty;

            while (input != "/exit")
            {
                input = Console.ReadLine();
                switch (input)
                {
                    case "/start":
                        //  Запрашиваем имя пользователя.
                        Console.WriteLine("Пожалуйста, введите свое имя:");
                        name = Console.ReadLine();
                        break;

                    case "/help":
                        //  Отображает краткую справочную информацию о том, как пользоваться программой.
                        Console.WriteLine($"{(string.IsNullOrWhiteSpace(name) ? "Б" : $"{name}, б")}от позволяет вносить данные о времени обучения за сегодня, записывать пройденную тему, смотреть статистику данных о времени занятий.");
                        break;   

                    case "/info":
                        //  Предоставляет информацию о версии программы и дате её создания.
                        Console.WriteLine(ProgrammVersionInfo);
                        break;

                    case "/echo":
                        //  После ввода имени становится доступной команда /echo.
                        //  При вводе этой команды с аргументом (например, /echo Hello), программа возвращает введенный текст (в данном примере "Hello").
                        input2 = Console.ReadLine();
                        Console.WriteLine(input2);
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

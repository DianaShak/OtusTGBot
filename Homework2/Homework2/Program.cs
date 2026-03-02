using System;

namespace Homework2
{
    internal class Program
    {
        const string ProgrammVersionInfo = "версия 1.0, 17.02.2026";

        static void Main(string[] args)
        {
            Console.WriteLine("Привет! Выберите нужную команду!");
            Console.WriteLine(@"/start /help /info /exit");
            var input = string.Empty;
            var name = string.Empty;

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
                        break;

                    case "/help":
                        //  Отображает краткую справочную информацию о том, как пользоваться программой.
                        Console.WriteLine($"{(string.IsNullOrWhiteSpace(name) ? "Пожалуйста, начните с команды /start." : $"{name}, команда /start запрашивает имя и открывает доступ к команде /echo, команда /help позволяет получить краткую справочную информацию о том, как пользоваться программой, команда /info позволяет получить информацию о версии программы и дате её создания, команда /echo позволяет вывести введенный Вами текст после команды, команда /exit позволяет выйти из меню.")}");
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

                    case "/exit":
                        break;

                    default:
                        break;


                }
            }
        }
    }
}

using System;

namespace Homework2
{
    public class DuplicateTaskException : Exception
    {

        public DuplicateTaskException(string task)
            : base($"Задача '{task}' уже существует.")
        {
        }
    }
}

using System;

namespace Homework2
{
    public class TaskCountLimitException : Exception
    {
        public TaskCountLimitException(int taskCountLimit)
            : base($"Превышено максимальное количество задач равное {taskCountLimit}")
        {
        }

        //public TaskCountLimitException()
        //{
        //}

        //public TaskCountLimitException(string message, Exception inner)
        //    : base(message, inner)
        //{
        //}
    }
}


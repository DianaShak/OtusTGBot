using System;

namespace Homework2
{
    public class TaskLengthLimitException : Exception
    {

        public TaskLengthLimitException(int length, int taskLengthtLimit)
            : base($"Длина задачи '{length}' превышает максимально допустимое значение '{taskLengthtLimit}'")
        {
        }
    }
}

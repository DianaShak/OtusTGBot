using System;

namespace Homework2
{
    public static class Validator
    {
        public static string ValidateString(string? str)
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

        public static int ParseAndValidateInt(string? str, int min, int max)
        {
            int strInt = 0;
            if (!int.TryParse(str, out strInt))
            {
                throw new ArgumentException("Введено не число.");
            }
            if (strInt > max || strInt < min)
            {
                throw new ArgumentException($"Длина введенных данных не соответствует диапазону от {min} до {max}.");
            }
            return strInt;
        }
    }
}

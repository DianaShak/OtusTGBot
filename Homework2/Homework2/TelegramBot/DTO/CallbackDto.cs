using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework2.TelegramBot.DTO
{
    public class CallbackDto
    {
        public string Action { get; set; } = string.Empty; //с помощью него будет определять за какое действие отвечает кнопка

        public CallbackDto(string action) 
        {
            Action = action;
        }
        public static CallbackDto FromString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            var parts = input.Split('|');

            return new CallbackDto(parts[0]);
        }
        public override string ToString()
        {
            return Action;
        }
    }
}

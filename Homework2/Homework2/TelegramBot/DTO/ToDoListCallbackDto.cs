using System;

namespace Homework2.TelegramBot.DTO
{
    public class ToDoListCallbackDto : CallbackDto
    {
        public Guid? ToDoListId { get; set; }

        public ToDoListCallbackDto(string action, Guid? toDoListId) : base(action)
        {
            ToDoListId = toDoListId;
        }

        public static new ToDoListCallbackDto FromString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            var parts = input.Split('|');
            var action = parts[0];

            Guid? toDoListId = null;
            if (parts.Length > 1 && Guid.TryParse(parts[1], out Guid parsedGuid))
            {
                toDoListId = parsedGuid;
            }

            return new ToDoListCallbackDto(action, toDoListId);
        }

        public override string ToString()
        {
            return ToDoListId.HasValue
                ? $"{base.ToString()}|{ToDoListId}"
                : base.ToString();
        }
    }
}

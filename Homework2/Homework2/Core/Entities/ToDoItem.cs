using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework2.Core.Entities
{
    public enum ToDoItemState { Active, Completed }
    public class ToDoItem
    {
        public Guid Id { get; set; }

        public ToDoUser User { get; set; }

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public ToDoItemState State { get; set; }

        public DateTime? StateChangedAt { get; set; }

        public DateTime Deadline {  get; set; }

        public ToDoList? List { get; set; }

        public ToDoItem(ToDoUser user, string name, DateTime deadline, ToDoList? list)
        {
            User = user;
            Name = name;
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            State = ToDoItemState.Active;
            Deadline = deadline;
            List = list;
        }
    }
}

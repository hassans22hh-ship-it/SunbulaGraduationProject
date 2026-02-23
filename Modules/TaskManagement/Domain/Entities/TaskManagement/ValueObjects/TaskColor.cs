using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDomain.Entities.TaskManagement.ValueObjects
{
    public class TaskColor : SharedKernel.ValueObject
    {
        public string Value { get; private set; }

        private TaskColor() { } // EF Core

        private TaskColor(string value)
        {
            Value = value;
        }

        public static TaskColor Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Color cannot be empty", nameof(value));

            return new TaskColor(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}

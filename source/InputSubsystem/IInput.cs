using System;

namespace InputSubsystem
{
    public interface IInput<out T>
    {
        public bool MoveNext();

        public T Current { get; }

        public bool IsOver { get; }
    }
}
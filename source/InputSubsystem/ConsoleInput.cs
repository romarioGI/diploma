using System;

namespace InputSubsystem
{
    public class ConsoleInput: TextReaderInput
    {
        public ConsoleInput() : base(Console.In)
        {
        }
    }
}
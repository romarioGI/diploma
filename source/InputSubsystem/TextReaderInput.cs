using System;
using System.IO;

namespace InputSubsystem
{
    public class TextReaderInput : IInput<Symbol>
    {
        private readonly TextReader _textReader;
        private int _symbolNumber;

        public TextReaderInput(TextReader textReader)
        {
            _textReader = textReader ?? throw new ArgumentNullException(nameof(textReader));
            Current = null;
            _symbolNumber = 0;
            IsOver = false;
        }

        public bool MoveNext()
        {
            var symbol = _textReader.Read();
            if (symbol < 0 || symbol == '\n' || symbol == '\r')
            {
                IsOver = true;
                return false;
            }

            Current = new Symbol(_symbolNumber++, Convert.ToChar(symbol));

            return true;
        }

        public Symbol Current { get; private set; }

        public bool IsOver { get; private set; }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace InputSubsystem
{
    //TODO [MTH] medium (integration) tests: input+parse+output=input
    public class TextReaderInput : IInput<Symbol>, IDisposable
    {
        private readonly TextReader _textReader;

        protected TextReaderInput(TextReader textReader)
        {
            _textReader = textReader ?? throw new ArgumentNullException(nameof(textReader));
        }

        public IEnumerator<Symbol> GetEnumerator()
        {
            var symbolNumber = 0;
            int symbol;
            while ((symbol = _textReader.Read()) >= 0)
                yield return new Symbol(symbolNumber++, Convert.ToChar(symbol));
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            _textReader?.Dispose();
        }
    }
}
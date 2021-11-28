using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace InputSubsystem
{
    public class TextReaderInput : IInput<Symbol>, IDisposable
    {
        private readonly TextReader _textReader;

        public TextReaderInput([NotNull] TextReader textReader)
        {
            _textReader = textReader ?? throw new ArgumentNullException(nameof(textReader));
        }

        public IEnumerator<Symbol> GetEnumerator()
        {
            var symbolNumber = 0;
            var symbol = 0;
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
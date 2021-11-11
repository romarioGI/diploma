using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace InputSubsystem
{
    public class TextReaderInput : IInput<Symbol>
    {
        private readonly TextReader _textReader;

        public TextReaderInput([NotNull] TextReader textReader)
        {
            _textReader = textReader ?? throw new NullReferenceException();
        }

        public IEnumerator<Symbol> GetEnumerator()
        {
            var symbolNumber = 0;
            foreach (var c in _textReader.ReadToEnd())
                yield return new Symbol(symbolNumber++, c);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
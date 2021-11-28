using System.Collections.Generic;
using System.Text;
using Common;
using InputSubsystem;

namespace ParserSubsystem
{
    public class LiteralLexeme : Lexeme
    {
        private readonly string _string;

        public LiteralLexeme(IEnumerable<Symbol> symbols)
        {
            var str = new StringBuilder();

            symbols = symbols.CutFirst(out var head);
            FirstSymbolIndex = head.Index;

            var last = symbols.DoActionAndReturnLast(s => str.Append(s.Character));
            LastSymbolIndex = last.Index;

            _string = str.ToString();
        }

        public override int FirstSymbolIndex { get; }
        public override int LastSymbolIndex { get; }

        public override string ToString() => _string;
    }
}
using InputSubsystem;

namespace ParserSubsystem
{
    public class SpecialSymbolLexeme : Lexeme
    {
        private readonly Symbol _symbol;

        public SpecialSymbolLexeme(Symbol symbol)
        {
            _symbol = symbol;
        }

        public override int FirstSymbolIndex => _symbol.Index;
        public override int LastSymbolIndex => _symbol.Index;

        public override string ToString() => new(_symbol.Character, 1);
    }
}
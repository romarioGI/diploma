namespace ParserSubsystem
{
    public class IdentifierToken : Token
    {
        public readonly IdentifierType Type;
        private readonly string _string;

        public override int FirstSymbolIndex { get; }
        public override int LastSymbolIndex { get; }

        public override string ToString() => _string;

        public IdentifierToken(IdentifierType type, int firstSymbolIndex, int lastSymbolIndex, string representation)
        {
            Type = type;
            FirstSymbolIndex = firstSymbolIndex;
            LastSymbolIndex = lastSymbolIndex;
            _string = representation;
        }
    }
}
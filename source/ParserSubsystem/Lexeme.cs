namespace ParserSubsystem
{
    public abstract class Lexeme : IWord
    {
        public abstract int FirstSymbolIndex { get; }
        public abstract int LastSymbolIndex { get; }

        public abstract override string ToString();
    }
}
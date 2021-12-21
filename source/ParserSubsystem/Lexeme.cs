namespace ParserSubsystem
{
    public abstract class Lexeme : Word
    {
        public abstract int FirstSymbolIndex { get; }
        public abstract int LastSymbolIndex { get; }

        public abstract override string ToString();
    }
}
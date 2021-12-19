namespace ParserSubsystem
{
    public abstract class Lexeme : Word
    {
        public abstract override int FirstSymbolIndex { get; }
        public abstract override int LastSymbolIndex { get; }

        public abstract override string ToString();
    }
}
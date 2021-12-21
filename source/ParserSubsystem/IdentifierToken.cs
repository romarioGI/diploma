namespace ParserSubsystem
{
    public class IdentifierToken : Token
    {
        public readonly IdentifierType Type;
        private readonly string _string;

        public override string ToString() => _string;

        public IdentifierToken(IdentifierType type, string representation)
        {
            Type = type;
            _string = representation;
        }
    }
}
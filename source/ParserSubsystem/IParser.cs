using InputSubsystem;

namespace ParserSubsystem
{
    public interface IParser<in TS, out TE> where TS: ISymbol where TE: IExpression
    {
        public TE Parse(IInput<TS> input);
    }
}
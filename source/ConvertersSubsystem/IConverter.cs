using ParserSubsystem;

namespace ConvertersSubsystem
{
    public interface IConverter<TE> where TE : IExpression
    {
        public TE Convert(TE expression);
    }
}
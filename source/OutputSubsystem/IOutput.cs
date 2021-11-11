using ParserSubsystem;

namespace OutputSubsystem
{
    public interface IOutput<in TE> where TE : IExpression
    {
        public void Print(TE expression);
    }
}
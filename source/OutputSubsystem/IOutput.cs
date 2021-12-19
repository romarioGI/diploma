using System.Collections.Generic;
using ParserSubsystem;

namespace OutputSubsystem
{
    public interface IOutput<in TE> where TE : IExpression
    {
        public string Print(TE expression);
    }
}
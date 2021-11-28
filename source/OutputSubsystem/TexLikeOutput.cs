using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ParserSubsystem;

namespace OutputSubsystem
{
    public class TexLikeOutput : IOutput<SyntaxTree>
    {
        public IEnumerable<char> Print([NotNull] SyntaxTree expression)
        {
            throw new NotImplementedException();
        }
    }
}
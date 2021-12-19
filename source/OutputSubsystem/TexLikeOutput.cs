using System;
using System.Collections.Generic;
using ParserSubsystem;

namespace OutputSubsystem
{
    //TODO [MTH] все спецсимволы как в техе
    //TODO [NTH] ввести приоритет операций и подсчитывать его, чтобы не плодить лишних скобок
    public class TexLikeOutput : IOutput<SyntaxTree>
    {
        public IEnumerable<char> Print(SyntaxTree expression)
        {
            throw new NotImplementedException();
        }
    }
}
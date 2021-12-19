using System;
using System.Linq;

namespace ParserSubsystem
{
    public class SyntaxTree : IExpression
    {
        private readonly SyntaxTree[] _operands;

        public readonly Token Token;

        public int OperandsCount => _operands.Length;

        public SyntaxTree GetOperand(int index)
        {
            if (index < 0 || index > OperandsCount) throw new IndexOutOfRangeException();
            return _operands[index];
        }

        public SyntaxTree(ExpressionType type, Token token, params SyntaxTree[] operands)
        {
            Type = type;
            Token = token ?? throw new ArgumentNullException(nameof(token));
            if (operands is null || operands.Any(x => x is null))
                throw new ArgumentNullException(nameof(operands));
            _operands = operands;
        }

        public ExpressionType Type { get; }
    }
}
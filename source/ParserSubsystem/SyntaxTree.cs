using System;
using System.Collections.Immutable;
using System.Linq;

namespace ParserSubsystem
{
    public class SyntaxTree : IExpression
    {
        public readonly Token Token;

        public int OperandsCount => Operands.Length;

        public ImmutableArray<SyntaxTree> Operands { get; }

        public SyntaxTree GetOperand(int index)
        {
            if (index < 0 || index > OperandsCount) throw new IndexOutOfRangeException();
            return Operands[index];
        }

        public SyntaxTree(ExpressionType type, Token token, params SyntaxTree[] operands)
        {
            Type = type;
            Token = token ?? throw new ArgumentNullException(nameof(token));
            if (operands is null || operands.Any(x => x is null))
                throw new ArgumentNullException(nameof(operands));
            Operands = operands.ToImmutableArray();
        }

        public ExpressionType Type { get; }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        protected SyntaxTree([NotNull] Token token, [NotNull] SyntaxTree[] operands)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
            if (operands is null || operands.Any(x => x is null))
                throw new ArgumentNullException(nameof(operands));
            _operands = operands;
        }

        protected SyntaxTree([NotNull] Token token, [NotNull] IEnumerable<SyntaxTree> operands) : this(token,
            operands.ToArray())
        {
        }
    }
}
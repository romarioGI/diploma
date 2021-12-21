using System;

namespace ParserSubsystem
{
    public class OperatorToken : Token
    {
        public readonly OperatorName Name;

        public override string ToString() => Name switch
        {
            OperatorName.ExistentialQuantifier => "∃",
            OperatorName.UniversalQuantifier => "∀",
            OperatorName.Conjunction => "&",
            OperatorName.Disjunction => "∨",
            OperatorName.Implication => "→",
            OperatorName.Negation => "¬",
            OperatorName.Less => "<",
            OperatorName.More => ">",
            OperatorName.Equal => "=",
            OperatorName.Plus => "+",
            OperatorName.Minus => "-",
            OperatorName.Multi => "*",
            OperatorName.Divide => "/",
            OperatorName.Exponentiation => "^",
            OperatorName.True => @"\true",
            OperatorName.False => @"\false",
            _ => throw new NotSupportedException()
        };

        public OperatorToken(OperatorName name)
        {
            Name = name;
        }
    }
}
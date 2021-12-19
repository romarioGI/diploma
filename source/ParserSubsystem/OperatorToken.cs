using System;

namespace ParserSubsystem
{
    public class OperatorToken : Token
    {
        public readonly OperatorName Name;

        public override int FirstSymbolIndex { get; }
        public override int LastSymbolIndex { get; }

        //TODO ещё не все
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
            _ => throw new NotSupportedException()
        };

        public OperatorToken(OperatorName name, int firstSymbolIndex, int lastSymbolIndex)
        {
            Name = name;
            FirstSymbolIndex = firstSymbolIndex;
            LastSymbolIndex = lastSymbolIndex;
        }
    }
}
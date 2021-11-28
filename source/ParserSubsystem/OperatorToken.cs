using System;

namespace ParserSubsystem
{
    public class OperatorToken : Token // опеаторы
    {
        public readonly OperatorName Name;

        public override int FirstSymbolIndex { get; }
        public override int LastSymbolIndex { get; }

        public override string ToString() => Name switch
        {
            OperatorName.Name => "name",
            _ => throw new NotImplementedException()
        };

        public OperatorToken(OperatorName name, int firstSymbolIndex, int lastSymbolIndex)
        {
            Name = name;
            FirstSymbolIndex = firstSymbolIndex;
            LastSymbolIndex = lastSymbolIndex;
        }
    }
}
namespace InputSubsystem
{
    public class Symbol : ISymbol
    {
        public readonly int Index;
        public readonly char Character;

        public Symbol(int index, char character)
        {
            Index = index;
            Character = character;
        }
    }
}
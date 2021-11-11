namespace InputSubsystem
{
    public readonly struct Symbol : ISymbol
    {
        public readonly int Number;
        public readonly char? Character;

        public Symbol(int number, char? character)
        {
            Number = number;
            Character = character;
        }
    }
}
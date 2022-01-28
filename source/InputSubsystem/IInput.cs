namespace InputSubsystem
{
    public interface IInput<out T> where T: ISymbol
    {
        public bool MoveNext();

        public T Current { get; }

        public bool IsOver { get; }
    }
}
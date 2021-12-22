namespace ProcessorsSubsystem.New
{
    public interface IRingElement
    {
        public Sign Sign { get; }

        public IRingElement Add(IRingElement right);

        public IRingElement Subtract(IRingElement right);

        public IRingElement Multiply(IRingElement right);

        public bool Equal(IRingElement right);
    }
}
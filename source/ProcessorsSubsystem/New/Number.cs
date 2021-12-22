namespace ProcessorsSubsystem.New
{
    public class Number: IRingElement
    {
        public Sign Sign => throw new System.NotImplementedException();

        //TODO должен хранить числа вида 2ab + b + c
        public Number(string variableName)
        {
            throw new System.NotImplementedException();
        }
        
        public IRingElement Add(IRingElement right)
        {
            throw new System.NotImplementedException();
        }

        public IRingElement Subtract(IRingElement right)
        {
            throw new System.NotImplementedException();
        }

        public IRingElement Multiply(IRingElement right)
        {
            throw new System.NotImplementedException();
        }

        public bool Equal(IRingElement right)
        {
            throw new System.NotImplementedException();
        }
    }
}
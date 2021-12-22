using ParserSubsystem;

namespace ProcessorsSubsystem
{
    public interface ISyntaxTreeProcessor: IProcessor<SyntaxTree>
    {
        public SyntaxTree DoOnlyRoot(SyntaxTree syntaxTree);
    }
}
using ParserSubsystem;

namespace ProcessorsSubsystem
{
    public abstract class SyntaxTreeProcessor : IProcessor<SyntaxTree>
    {
        protected abstract SyntaxTree DoInner(SyntaxTree syntaxTree, bool recursively);

        public SyntaxTree Do(SyntaxTree syntaxTree)
        {
            return DoInner(syntaxTree, true);
        }

        public SyntaxTree DoOnlyRoot(SyntaxTree syntaxTree)
        {
            return DoInner(syntaxTree, false);
        }
    }
}

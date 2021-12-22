using System;
using System.Linq;
using ParserSubsystem;

namespace ProcessorsSubsystem
{
    public class TarskiQuantifierEliminator: ISyntaxTreeProcessor
    {
        public SyntaxTree Do(SyntaxTree expression)
        {
            return DoInner(expression, true);
        }

        public SyntaxTree DoOnlyRoot(SyntaxTree syntaxTree)
        {
            return DoInner(syntaxTree, false);
        }

        private SyntaxTree DoInner(SyntaxTree syntaxTree, bool recursively)
        {
            switch (syntaxTree.Type)
            {
                case ExpressionType.Formula:
                {
                    if (syntaxTree.Token is OperatorToken operatorToken)
                    {
                        if (operatorToken.Name.IsQuantifier() && syntaxTree.OperandsCount == 2)
                        {
                            var variableTree = syntaxTree.GetOperand(0);
                            if (variableTree.Type == ExpressionType.Identifier
                                && variableTree.Token is IdentifierToken {Type: IdentifierType.Variable} variable)
                            {
                                var subFormula = syntaxTree.GetOperand(1);

                                if (recursively)
                                    subFormula = Do(subFormula);

                                return TarskiElimination(operatorToken, variable, subFormula);
                            }
                        }
                        else if (operatorToken.Name.IsConnective())
                        {
                            if (recursively)
                            {
                                var operands = syntaxTree.Operands.AsParallel().Select(Do).ToArray();
                                syntaxTree = new SyntaxTree(
                                    syntaxTree.Type,
                                    syntaxTree.Token,
                                    operands
                                );
                            }

                            return Processors.ZeroArityPredicateEliminator.DoOnlyRoot(syntaxTree);
                        }
                    }

                    break;
                }
            }

            return syntaxTree;
        }

        private static SyntaxTree TarskiElimination(OperatorToken quantifier, IdentifierToken variable, SyntaxTree subFormula)
        {
            throw new NotImplementedException();
        }
    }
}
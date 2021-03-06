using System.Linq;
using ParserSubsystem;

namespace ProcessorsSubsystem
{
    public class ZeroArityPredicateEliminator : SyntaxTreeProcessor
    {
        private static bool IsTruePredicate(SyntaxTree expression)
        {
            return expression.Type == ExpressionType.Term
                   && expression.Token is OperatorToken {Name: OperatorName.True};
        }

        private static bool IsFalsePredicate(SyntaxTree expression)
        {
            return expression.Type == ExpressionType.Term
                   && expression.Token is OperatorToken {Name: OperatorName.False};
        }

        private static bool IsBooleanPredicate(SyntaxTree expression)
        {
            return IsTruePredicate(expression) || IsFalsePredicate(expression);
        }

        private static SyntaxTree GetTrueExpression()
        {
            return new(ExpressionType.Term, new OperatorToken(OperatorName.True));
        }

        private static SyntaxTree GetFalseExpression()
        {
            return new(ExpressionType.Term, new OperatorToken(OperatorName.False));
        }

        private SyntaxTree GetNegationExpression(SyntaxTree syntaxTree)
        {
            var result = new SyntaxTree(ExpressionType.Formula, new OperatorToken(OperatorName.Negation), syntaxTree);
            return Do(result);
        }

        protected override SyntaxTree DoInner(SyntaxTree syntaxTree, bool recursively)
        {
            var expOperands = syntaxTree.Operands.AsParallel();
            if (recursively)
                expOperands = expOperands.Select(Do);
            var operands = expOperands.ToArray();
            if (syntaxTree.Type != ExpressionType.Formula
                || !operands.Any(IsBooleanPredicate)
                || syntaxTree.Token is not OperatorToken operatorToken)
                return syntaxTree;
            switch (operatorToken.Name)
            {
                case OperatorName.Conjunction when operands.Length == 2:
                {
                    if (IsFalsePredicate(operands[0]))
                        return operands[0];
                    if (IsFalsePredicate(operands[1]))
                        return operands[1];
                    if (IsTruePredicate(operands[0]))
                        return operands[1];
                    if (IsTruePredicate(operands[1]))
                        return operands[0];
                    return syntaxTree;
                }
                case OperatorName.Disjunction when operands.Length == 2:
                {
                    if (IsTruePredicate(operands[0]))
                        return operands[0];
                    if (IsTruePredicate(operands[1]))
                        return operands[1];
                    if (IsFalsePredicate(operands[0]))
                        return operands[1];
                    if (IsFalsePredicate(operands[1]))
                        return operands[0];
                    return syntaxTree;
                }
                case OperatorName.Implication when operands.Length == 2:
                {
                    if (IsTruePredicate(operands[0]))
                        return operands[1];
                    if (IsFalsePredicate(operands[0]))
                        return GetTrueExpression();
                    if (IsTruePredicate(operands[1]))
                        return GetTrueExpression();
                    if (IsFalsePredicate(operands[1]))
                        return GetNegationExpression(operands[0]);
                    return syntaxTree;
                }
                case OperatorName.Negation when operands.Length == 1:
                {
                    if (IsTruePredicate(operands[0]))
                        return GetFalseExpression();
                    if (IsFalsePredicate(operands[0]))
                        return GetTrueExpression();
                    return syntaxTree;
                }
                case OperatorName.ExistentialQuantifier when operands.Length == 2:
                {
                    if (IsBooleanPredicate(operands[1]))
                        return operands[1];
                    return syntaxTree;
                }
                case OperatorName.UniversalQuantifier when operands.Length == 2:
                {
                    if (IsBooleanPredicate(operands[1]))
                        return operands[1];
                    return syntaxTree;
                }
                default:
                    return syntaxTree;
            }
        }
    }
}
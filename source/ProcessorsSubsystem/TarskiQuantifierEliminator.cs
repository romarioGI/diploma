using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ParserSubsystem;

namespace ProcessorsSubsystem
{
    public class TarskiQuantifierEliminator : SyntaxTreeProcessor
    {
        protected override SyntaxTree DoInner(SyntaxTree syntaxTree, bool recursively)
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

                                if (WithoutQuantifier(subFormula) && ExponentiationIsArithmetic(subFormula))
                                {
                                    var result = TarskiElimination(operatorToken, variable, subFormula);
                                    if (result is not null)
                                    {
                                        result = Processors.ZeroArityPredicateEliminator.Do(result);

                                        return result;
                                    }
                                }
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

        private static bool WithoutQuantifier(SyntaxTree formula)
        {
            return !formula.Any(t =>
                t is OperatorToken op && op.Name.IsQuantifier());
        }

        private static bool ExponentiationIsArithmetic(SyntaxTree formula)
        {
            if (!formula.Operands.All(ExponentiationIsArithmetic))
                return false;
            if (formula.Type == ExpressionType.Term && formula.Token is OperatorToken
                {Name: OperatorName.Exponentiation})
            {
                if (formula.OperandsCount != 2)
                    return false;
                var rightOperands = formula.GetOperand(1);

                return rightOperands.Type == ExpressionType.Identifier
                       && rightOperands.OperandsCount == 0
                       && rightOperands.Token is IdentifierToken {Type: IdentifierType.Constant};
            }

            return true;
        }

        private static SyntaxTree TarskiElimination(OperatorToken quantifier, IdentifierToken variable,
            SyntaxTree subFormula)
        {
            var predicateTerms = subFormula.GetAll(t =>
                t.Token is OperatorToken operatorToken && operatorToken.Name.IsPredicate());

            var predicatesToPolynomialAndSign = predicateTerms
                .AsParallel()
                .ToDictionary(pr => pr, pr => PredicatesToPolynomialsWithExpectedSigns(pr, variable));

            var polynomials = predicatesToPolynomialAndSign
                .Values
                .Select(x => x.Item1);

            var saturatedSystem = Saturate(polynomials);
            var tarskiTable = BuildTarskiTable(saturatedSystem);
            var newFormula = BuildEquivalentFormula(
                tarskiTable,
                quantifier,
                subFormula,
                predicatesToPolynomialAndSign
            );

            return newFormula;
        }

        private static (Polynomial, Sign) PredicatesToPolynomialsWithExpectedSigns(
            SyntaxTree predicateTerm,
            IdentifierToken variable
        )
        {
            var predicate = predicateTerm.Token as OperatorToken;
            var operands = predicateTerm.Operands;

            var sign = PredicateToSign(predicate);
            var polynomial = ToPolynomial(operands, variable);

            return (polynomial, sign);
        }

        private static Sign PredicateToSign(OperatorToken operatorToken)
        {
            return operatorToken.Name switch
            {
                OperatorName.Less => Sign.LessZero,
                OperatorName.More => Sign.MoreZero,
                OperatorName.Equal => Sign.EqualZero,
                _ => Sign.None
            };
        }

        private static Polynomial ToPolynomial(ImmutableArray<SyntaxTree> operands, IdentifierToken variable)
        {
            if (operands.Length > 2 && operands.Length <= 0)
                throw new NotSupportedException();
            var term = operands[0];
            if (operands.Length == 2)
                term = new SyntaxTree(ExpressionType.Term, new OperatorToken(OperatorName.Minus), operands[0],
                    operands[1]);

            return ToPolynomial(term, new VariableName(variable.ToString()));
        }

        private static Polynomial ToPolynomial(SyntaxTree term, VariableName variable)
        {
            var token = term.Token;
            switch (term.Type)
            {
                case ExpressionType.Term when token is OperatorToken operatorToken:
                    switch (term.OperandsCount)
                    {
                        case 1:
                        {
                            var p1 = ToPolynomial(term.Operands[0], variable);
                            return operatorToken.Name switch
                            {
                                OperatorName.Minus => -p1,
                                _ => throw new NotSupportedException()
                            };
                        }
                        case 2:
                        {
                            var p1 = ToPolynomial(term.Operands[0], variable);
                            var p2 = ToPolynomial(term.Operands[1], variable);
                            return operatorToken.Name switch
                            {
                                OperatorName.Plus => p1 + p2,
                                OperatorName.Minus => p1 - p2,
                                OperatorName.Multi => p1 * p2,
                                OperatorName.Divide when p2.Degree > 1 => throw new Exception(
                                    "not support polynomial divide"),
                                OperatorName.Divide => p1 / p2,
                                OperatorName.Exponentiation when p2.Degree > 1 => throw new Exception(
                                    "not support polynomial pow"),
                                OperatorName.Exponentiation when !p2.Leading.IsNatural => throw new Exception(
                                    "not support not natural pow"),
                                OperatorName.Exponentiation => p1.Pow(p2.Leading),
                                _ => throw new NotSupportedException()
                            };
                        }
                        default:
                            throw new NotSupportedException();
                    }
                case ExpressionType.Identifier when token is IdentifierToken identifierToken:
                    switch (identifierToken.Type)
                    {
                        case IdentifierType.Constant:
                            return new Polynomial(
                                new RationalNumber[] {Convert.ToInt32(identifierToken.ToString())},
                                variable
                            );
                        case IdentifierType.Variable:
                        {
                            var curVariable = new VariableName(identifierToken.ToString());
                            return curVariable.Equals(variable) switch
                            {
                                false => throw new Exception("VariableDomainException"),
                                _ => new Polynomial(new RationalNumber[] {0, 1}, variable)
                            };
                        }
                        default:
                            throw new NotSupportedException();
                    }
                default:
                    throw new NotSupportedException();
            }
        }

        private static IEnumerable<Polynomial> Saturate(IEnumerable<Polynomial> polynomials)
        {
            return Saturator.Saturate(polynomials);
        }

        private static TarskiTable BuildTarskiTable(IEnumerable<Polynomial> saturatedSystem)
        {
            return new(saturatedSystem);
        }

        private static SyntaxTree GetTrueExpression()
        {
            return new(ExpressionType.Term, new OperatorToken(OperatorName.True));
        }

        private static SyntaxTree GetFalseExpression()
        {
            return new(ExpressionType.Term, new OperatorToken(OperatorName.False));
        }

        private static SyntaxTree BuildEquivalentFormula(TarskiTable tarskiTable, OperatorToken quantifier,
            SyntaxTree formula, Dictionary<SyntaxTree, (Polynomial, Sign)> predicates)
        {
            var tarskiTableWidth = tarskiTable.Width;
            var formulas = Enumerable.Empty<SyntaxTree>();
            var tarskiTableDictionary = tarskiTable.GetTableDictionary();
            for (var i = 0; i < tarskiTableWidth; i++)
            {
                var substitutions = new Dictionary<SyntaxTree, SyntaxTree>();
                foreach (var (predicateFormula, (polynomial, sign)) in predicates)
                {
                    var actualSign = tarskiTableDictionary[polynomial][i];
                    if (actualSign == sign)
                        substitutions.Add(predicateFormula, GetTrueExpression());
                    else
                        substitutions.Add(predicateFormula, GetFalseExpression());
                }

                formulas = formulas.Append(SubstituteInFormula(formula, substitutions));
            }

            return JoinFormulas(formulas.ToArray(), quantifier);
        }

        private static SyntaxTree GetDisjunctionExpression(SyntaxTree operand1, SyntaxTree operand2)
        {
            return new(ExpressionType.Formula, new OperatorToken(OperatorName.Disjunction), operand1, operand2);
        }

        private static SyntaxTree GetConjunctionExpression(SyntaxTree operand1, SyntaxTree operand2)
        {
            return new(ExpressionType.Formula, new OperatorToken(OperatorName.Conjunction), operand1, operand2);
        }
        
        private static SyntaxTree SubstituteInFormula(SyntaxTree formula,
            Dictionary<SyntaxTree, SyntaxTree> substitutions)
        {
            if (formula.Type == ExpressionType.Formula)
            {
                var operatorToken = formula.Token as OperatorToken;
                if (operatorToken.Name.IsPredicate())
                    return substitutions[formula];
                if (operatorToken.Name.IsConnective())
                {
                    var operands = formula
                        .Operands
                        .AsParallel()
                        .Select(f => SubstituteInFormula(f, substitutions))
                        .ToArray();
                    return new SyntaxTree(ExpressionType.Formula, operatorToken, operands);
                }

                return formula;
            }

            return formula;
        }

        private static SyntaxTree JoinFormulas(SyntaxTree[] formulas, OperatorToken quantifier)
        {
            var formula = formulas[0];
            for (var i = 1; i < formulas.Length; ++i)
            {
                formula = quantifier.Name == OperatorName.UniversalQuantifier
                    ? GetConjunctionExpression(formula, formulas[i])
                    : GetDisjunctionExpression(formula, formulas[i]);
            }

            return formula;
        }
    }
}
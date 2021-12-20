using System;
using System.Text;
using ParserSubsystem;

namespace OutputSubsystem
{
    //TODO [NTH] все спецсимволы как в техе
    public class NativeOutput : IOutput<SyntaxTree>
    {
        public string Print(SyntaxTree expression)
        {
            return Print(expression, out _).ToString();
        }

        private StringBuilder Print(SyntaxTree expression, out int minBracketsFreeOperatorPriority)
        {
            minBracketsFreeOperatorPriority = MaxPriority;
            var result = new StringBuilder();
            switch (expression.Type)
            {
                case ExpressionType.Formula:
                case ExpressionType.Term:
                {
                    if (expression.Token is OperatorToken operatorToken)
                    {
                        var notation = GetNotation(operatorToken, expression.OperandsCount);
                        var associativity = GetAssociativity(operatorToken);
                        var priority = GetPriority(operatorToken, notation);
                        switch (notation)
                        {
                            case NotationType.QuantifierLike:
                            {
                                result.Append('(');
                                result.Append(operatorToken);
                                result.Append(Print(expression.GetOperand(0)));
                                result.Append(')');
                                result.Append(Print(expression.GetOperand(1), out var subFormulaPriority));
                                minBracketsFreeOperatorPriority =
                                    Math.Min(minBracketsFreeOperatorPriority, subFormulaPriority);
                                break;
                            }
                            case NotationType.Prefix:
                            {
                                minBracketsFreeOperatorPriority =
                                    Math.Min(minBracketsFreeOperatorPriority, priority);
                                result.Append(operatorToken);
                                for (var i = 0; i < expression.OperandsCount; ++i)
                                {
                                    var subFormulaString = Print(expression.GetOperand(i), out var subFormulaPriority);
                                    if (subFormulaPriority < priority)
                                    {
                                        result.Append('(');
                                        result.Append(subFormulaString);
                                        result.Append(')');
                                    }
                                    else
                                    {
                                        result.Append(subFormulaString);
                                        minBracketsFreeOperatorPriority =
                                            Math.Min(minBracketsFreeOperatorPriority, subFormulaPriority);
                                    }
                                }

                                break;
                            }
                            case NotationType.Infix:
                                minBracketsFreeOperatorPriority =
                                    Math.Min(minBracketsFreeOperatorPriority, priority);
                                var leftSubFormula = Print(expression.GetOperand(0), out var leftSubFormulaPriority);
                                var rightSubFormula = Print(expression.GetOperand(1), out var rightSubFormulaPriority);

                                switch (associativity)
                                {
                                    case AssociativityType.Left:
                                    {
                                        if (leftSubFormulaPriority < priority)
                                        {
                                            result.Append('(');
                                            result.Append(leftSubFormula);
                                            result.Append(')');
                                        }
                                        else
                                        {
                                            result.Append(leftSubFormula);
                                            minBracketsFreeOperatorPriority =
                                                Math.Min(minBracketsFreeOperatorPriority, leftSubFormulaPriority);
                                        }

                                        result.Append(operatorToken);

                                        if (rightSubFormulaPriority <= priority)
                                        {
                                            result.Append('(');
                                            result.Append(rightSubFormula);
                                            result.Append(')');
                                        }
                                        else
                                        {
                                            result.Append(rightSubFormula);
                                            minBracketsFreeOperatorPriority =
                                                Math.Min(minBracketsFreeOperatorPriority, rightSubFormulaPriority);
                                        }

                                        break;
                                    }
                                    case AssociativityType.Right:
                                    {
                                        if (leftSubFormulaPriority <= priority)
                                        {
                                            result.Append('(');
                                            result.Append(leftSubFormula);
                                            result.Append(')');
                                        }
                                        else
                                        {
                                            result.Append(leftSubFormula);
                                            minBracketsFreeOperatorPriority =
                                                Math.Min(minBracketsFreeOperatorPriority, leftSubFormulaPriority);
                                        }

                                        result.Append(operatorToken);

                                        if (rightSubFormulaPriority < priority)
                                        {
                                            result.Append('(');
                                            result.Append(rightSubFormula);
                                            result.Append(')');
                                        }
                                        else
                                        {
                                            result.Append(rightSubFormula);
                                            minBracketsFreeOperatorPriority =
                                                Math.Min(minBracketsFreeOperatorPriority, rightSubFormulaPriority);
                                        }

                                        break;
                                    }
                                    default:
                                        throw new NotSupportedException();
                                }

                                break;
                            default:
                                throw new NotSupportedException();
                        }
                    }
                    else
                        throw new NotSupportedException();

                    break;
                }
                case ExpressionType.Identifier:
                    switch (expression.Token)
                    {
                        case IdentifierToken identifierToken:
                            result.Append(identifierToken);
                            break;
                        default:
                            throw new NotSupportedException();
                    }

                    break;
                default:
                    throw new NotSupportedException();
            }

            return result;
        }

        private static NotationType GetNotation(OperatorToken operatorToken, int operandsCount)
        {
            return operatorToken.Name switch
            {
                OperatorName.ExistentialQuantifier when operandsCount == 2 => NotationType.QuantifierLike,
                OperatorName.UniversalQuantifier when operandsCount == 2 => NotationType.QuantifierLike,
                OperatorName.Conjunction when operandsCount == 2 => NotationType.Infix,
                OperatorName.Disjunction when operandsCount == 2 => NotationType.Infix,
                OperatorName.Implication when operandsCount == 2 => NotationType.Infix,
                OperatorName.Negation when operandsCount == 1 => NotationType.Prefix,
                OperatorName.Less when operandsCount == 2 => NotationType.Infix,
                OperatorName.More when operandsCount == 2 => NotationType.Infix,
                OperatorName.Equal when operandsCount == 2 => NotationType.Infix,
                OperatorName.Plus when operandsCount == 2 => NotationType.Infix,
                OperatorName.Minus when operandsCount == 1 => NotationType.Prefix,
                OperatorName.Minus when operandsCount == 2 => NotationType.Infix,
                OperatorName.Multi when operandsCount == 2 => NotationType.Infix,
                OperatorName.Divide when operandsCount == 2 => NotationType.Infix,
                OperatorName.Exponentiation when operandsCount == 2 => NotationType.Infix,
                _ => throw new NotSupportedException()
            };
        }

        private static AssociativityType GetAssociativity(OperatorToken operatorToken)
        {
            return operatorToken.Name == OperatorName.Exponentiation
                ? AssociativityType.Right
                : AssociativityType.Left;
        }

        private const int MaxPriority = int.MaxValue;

        private static int GetPriority(OperatorToken operatorToken, NotationType type)
        {
            return operatorToken.Name switch
            {
                OperatorName.ExistentialQuantifier => 10,
                OperatorName.UniversalQuantifier => 10,
                OperatorName.Conjunction => 20,
                OperatorName.Disjunction => 20,
                OperatorName.Implication => 20,
                OperatorName.Negation => 50,
                OperatorName.Less => 60,
                OperatorName.More => 60,
                OperatorName.Equal => 60,
                OperatorName.Plus => 70,
                OperatorName.Minus when type == NotationType.Infix => 70,
                OperatorName.Multi => 80,
                OperatorName.Divide => 80,
                OperatorName.Exponentiation => 90,
                OperatorName.Minus when type == NotationType.Prefix => 100,
                _ => throw new NotSupportedException()
            };
        }
    }
}
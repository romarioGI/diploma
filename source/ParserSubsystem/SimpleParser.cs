using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using InputSubsystem;

namespace ParserSubsystem
{
    //TODO [MTH] add medium (integration) tests: input+parser+output
    public class SimpleParser : IParser<Symbol, SyntaxTree>
    {
        private const int NotFound = ParsingContext.NotFound;

        public SyntaxTree Parse(IInput<Symbol> input)
        {
            if (input is null)
                //TODO  это системная ошибка, она должна перехватываться в ошибку "что-то пошло не так", в отличии от ошибок парсинга, которые можно отдавать as is
                throw new Exception("Input is null");

            var lexemes = ToLexemes(input).ToImmutableArray();
            //TODO [NTH] залоггировать, что с лексемами всё ок, можно выписать сами лексемы
            var syntaxTree = ToSyntaxTree(lexemes);

            return syntaxTree;
        }

        private static IEnumerable<Lexeme> ToLexemes(IEnumerable<Symbol> input)
        {
            using var enumerator = input.GetEnumerator();

            if (enumerator.Current is null)
                yield break;

            do
            {
                if (enumerator.Current.IsWhiteSpace())
                    enumerator.MoveNext();
                else if (enumerator.Current.IsLiteralSymbol() || enumerator.Current.IsBackslash())
                    yield return GetLiteralLexeme(enumerator);
                else
                    yield return GetSpecialSymbolLexeme(enumerator);

            } while (enumerator.Current is not null);
        }

        private static Lexeme GetSpecialSymbolLexeme(IEnumerator<Symbol> enumerator)
        {
            var lexeme = new SpecialSymbolLexeme(enumerator.Current);
            enumerator.MoveNext();

            return lexeme;
        }

        private static Lexeme GetLiteralLexeme(IEnumerator<Symbol> enumerator)
        {
            return new LiteralLexeme(GetLiteralSymbols(enumerator));
        }

        private static IEnumerable<Symbol> GetLiteralSymbols(IEnumerator<Symbol> enumerator)
        {
            if (enumerator.Current.IsBackslash())
            {
                yield return enumerator.Current;
                enumerator.MoveNext();
            }

            while (enumerator.Current is not null)
            {
                if (enumerator.Current.IsLiteralSymbol())
                    yield return enumerator.Current;
                else
                    break;
                enumerator.MoveNext();
            }
        }

        private static SyntaxTree ToSyntaxTree(ImmutableArray<Lexeme> lexemes)
        {
            var ctx = new ParsingContext(lexemes);
            return ParseFormula(ctx);
        }

        private static SyntaxTree ParseFormula(ParsingContext ctx)
        {
            return ctx.IsEmpty ? ExpectedLexemeButEmpty(ctx) : ParseQuantifierFormula(ctx);
        }

        private static SyntaxTree ParseQuantifierFormula(ParsingContext ctx)
        {
            var end = FindQuantifierExpressionEnd(ctx, out var quantifier, out var variable);
            if (end == NotFound)
                return ParseInfixConnectiveFormula(ctx);

            var subFormulaCtx = ctx.Skip(end);
            var subFormula = ParseQuantifierFormula(subFormulaCtx);

            var variableSyntaxTree = new SyntaxTree(ExpressionType.Identifier, variable);

            return new SyntaxTree(ExpressionType.Formula, quantifier, variableSyntaxTree, subFormula);
        }

        private static int FindQuantifierExpressionEnd(ParsingContext ctx, out OperatorToken quantifier,
            out IdentifierToken variable)
        {
            var curCtx = ctx;

            quantifier = null;
            variable = null;

            if (curCtx.IsEmpty || !curCtx.First.IsLeftBracket())
                return NotFound;
            curCtx = curCtx.SkipFirst();

            if (curCtx.IsEmpty || !curCtx.First.IsQuantifier())
                return NotFound;
            quantifier = LexemeToOperatorToken(curCtx.First);
            curCtx = curCtx.SkipFirst();

            var variableEnd = FindVariableExpressionEnd(curCtx, out variable);
            if (variableEnd == NotFound)
                return NotFound;
            curCtx = curCtx.Skip(variableEnd);

            if (curCtx.IsEmpty || !curCtx.First.IsRightBracket())
                return NotFound;
            curCtx = curCtx.SkipFirst();

            return ctx.LexemeCount - curCtx.LexemeCount;
        }

        private static int FindVariableExpressionEnd(ParsingContext ctx, out IdentifierToken variable)
        {
            variable = null;

            var curCtx = ctx;

            if (curCtx.IsEmpty
                || !(curCtx.First is LiteralLexeme letterLexeme)
                || !letterLexeme.IsOnlyLetters()
            )
                return NotFound;
            curCtx = curCtx.SkipFirst();

            if (curCtx.IsEmpty
                || !curCtx.First.IsUnderlining())
            {
                variable = LexemeToIdentifierToken(letterLexeme);
            }
            else
            {
                var underlining = curCtx.First;
                curCtx = curCtx.SkipFirst();

                if (curCtx.IsEmpty
                    || !(curCtx.First is LiteralLexeme digitLexeme)
                    || !digitLexeme.IsOnlyDigits()
                )
                    return NotFound;
                curCtx = curCtx.SkipFirst();

                variable = LexemeToIdentifierToken(letterLexeme, underlining, digitLexeme);
            }

            return ctx.LexemeCount - curCtx.LexemeCount;
        }

        private static int FindConstantExpressionEnd(ParsingContext ctx, out IdentifierToken constant)
        {
            constant = null;

            var curCtx = ctx;
            if (curCtx.IsEmpty
                || !(curCtx.First is LiteralLexeme digitLexeme)
                || !digitLexeme.IsOnlyDigits()
            )
                return NotFound;
            curCtx = curCtx.SkipFirst();

            constant = new IdentifierToken(
                IdentifierType.Constant,
                digitLexeme.FirstSymbolIndex,
                digitLexeme.LastSymbolIndex,
                digitLexeme.ToString()
            );
            
            return ctx.LexemeCount - curCtx.LexemeCount;
        }

        private static OperatorToken LexemeToOperatorToken(Lexeme lexeme)
        {
            return new(lexeme.ToOperatorName(), lexeme.FirstSymbolIndex, lexeme.LastSymbolIndex);
        }

        private static IdentifierToken LexemeToIdentifierToken(params Lexeme[] lexemes)
        {
            var firstIndex = lexemes[0].FirstSymbolIndex;
            var lastIndex = lexemes[^1].LastSymbolIndex;
            var representation = string.Join<Lexeme>(string.Empty, lexemes);
            return new IdentifierToken(IdentifierType.Variable, firstIndex, lastIndex, representation);
        }

        private static SyntaxTree ParseInfixConnectiveFormula(ParsingContext ctx)
        {
            var connectiveIndex = ctx.FindFirstWithZeroBracketBalance(l => l.IsInfixPropositionalConnective());
            if (connectiveIndex == NotFound)
                return ParsePrefixConnectiveFormula(ctx);

            var (leftSubFormulaCtx, tailCtx) = ctx.SplitIntoTwoParts(connectiveIndex);
            var connectiveLexeme = tailCtx.First;
            var rightSubFormulaCtx = tailCtx.SkipFirst();

            var leftSubFormula = ParsePrefixConnectiveFormula(leftSubFormulaCtx);
            var connectiveToken = LexemeToOperatorToken(connectiveLexeme);
            var rightSubFormula = ParseInfixConnectiveFormula(rightSubFormulaCtx);

            return new SyntaxTree(ExpressionType.Formula, connectiveToken, leftSubFormula, rightSubFormula);
        }

        private static SyntaxTree ParsePrefixConnectiveFormula(ParsingContext ctx)
        {
            if (ctx.IsEmpty || !ctx.First.IsPrefixPropositionalConnective())
                return ParseInfixPredicateFormula(ctx);

            var connective = LexemeToOperatorToken(ctx.First);
            ctx = ctx.SkipFirst();

            var subFormula = ParsePrefixConnectiveFormula(ctx);

            return new SyntaxTree(ExpressionType.Formula, connective, subFormula);
        }

        private static SyntaxTree ParseInfixPredicateFormula(ParsingContext ctx)
        {
            var predicateIndex = ctx.FindFirstWithZeroBracketBalance(l => l.IsInfixPredicate());
            if (predicateIndex == NotFound)
                return ParseBracketsFormula(ctx);

            var (leftTermCtx, tailCtx) = ctx.SplitIntoTwoParts(predicateIndex);
            var predicateLexeme = tailCtx.First;
            var rightTermCtx = tailCtx.SkipFirst();

            var leftTerm = ParseTerm(leftTermCtx);
            var predicateToken = LexemeToOperatorToken(predicateLexeme);
            var rightTerm = ParseTerm(rightTermCtx);

            return new SyntaxTree(ExpressionType.Formula, predicateToken, leftTerm, rightTerm);
        }

        private static SyntaxTree ParseBracketsFormula(ParsingContext ctx)
        {
            if (ctx.IsEmpty)
                return ParseFormula(ctx);
            if (!ctx.First.IsLeftBracket())
                return UnexpectedLexeme(ctx);
            ctx = ctx.SkipFirst();

            if (ctx.IsEmpty)
                return ParseFormula(ctx);
            if (ctx.IsEmpty || !ctx.Last.IsRightBracket())
                return UnexpectedLexeme(ctx);
            ctx = ctx.SkipLast();

            return ParseFormula(ctx);
        }

        private static SyntaxTree ParseTerm(ParsingContext ctx)
        {
            return ctx.IsEmpty ? ExpectedLexemeButEmpty(ctx) : ParseInfixPlusOrMinusFunctionTerm(ctx);
        }

        private static SyntaxTree ParseInfixPlusOrMinusFunctionTerm(ParsingContext ctx)
        {
            var functionIndex = ctx.FindFirstWithZeroBracketBalance(l => l.IsPlusOrMinus());
            if (functionIndex == NotFound)
                return ParseInfixMultiOrDivideFunctionTerm(ctx);

            var (leftTermCtx, tailCtx) = ctx.SplitIntoTwoParts(functionIndex);
            var functionLexeme = tailCtx.First;
            var rightTermCtx = tailCtx.SkipFirst();

            var leftTerm = ParseInfixMultiOrDivideFunctionTerm(leftTermCtx);
            var functionToken = LexemeToOperatorToken(functionLexeme);
            var rightTerm = ParseInfixPlusOrMinusFunctionTerm(rightTermCtx);

            return new SyntaxTree(ExpressionType.Term, functionToken, leftTerm, rightTerm);
        }

        private static SyntaxTree ParseInfixMultiOrDivideFunctionTerm(ParsingContext ctx)
        {
            var functionIndex = ctx.FindFirstWithZeroBracketBalance(l => l.IsMultiOrDivide());
            if (functionIndex == NotFound)
                return ParseExponentiationFunctionTerm(ctx);

            var (leftTermCtx, tailCtx) = ctx.SplitIntoTwoParts(functionIndex);
            var functionLexeme = tailCtx.First;
            var rightTermCtx = tailCtx.SkipFirst();

            var leftTerm = ParseExponentiationFunctionTerm(leftTermCtx);
            var functionToken = LexemeToOperatorToken(functionLexeme);
            var rightTerm = ParseInfixMultiOrDivideFunctionTerm(rightTermCtx);

            return new SyntaxTree(ExpressionType.Term, functionToken, leftTerm, rightTerm);
        }

        private static SyntaxTree ParseExponentiationFunctionTerm(ParsingContext ctx)
        {
            var functionIndex = ctx.FindLastWithZeroBracketBalance(l => l.IsExponentiation());
            if (functionIndex == NotFound)
                return ParseUnaryFunctionTerm(ctx);

            var (leftTermCtx, tailCtx) = ctx.SplitIntoTwoParts(functionIndex);
            var functionLexeme = tailCtx.First;
            var rightTermCtx = tailCtx.SkipFirst();

            var leftTerm = ParseExponentiationFunctionTerm(leftTermCtx);
            var functionToken = LexemeToOperatorToken(functionLexeme);
            var rightTerm = ParseUnaryFunctionTerm(rightTermCtx);

            return new SyntaxTree(ExpressionType.Term, functionToken, leftTerm, rightTerm);
        }

        private static SyntaxTree ParseUnaryFunctionTerm(ParsingContext ctx)
        {
            if (ctx.IsEmpty || !ctx.First.IsMinus())
                return ParseIdentifierTerm(ctx);

            var function = LexemeToOperatorToken(ctx.First);
            ctx = ctx.SkipFirst();

            var subTerm = ParseUnaryFunctionTerm(ctx);

            return new SyntaxTree(ExpressionType.Term, function, subTerm);
        }

        private static SyntaxTree ParseIdentifierTerm(ParsingContext ctx)
        {
            var variableExpressionEnd = FindVariableExpressionEnd(ctx, out var variable);
            if (variableExpressionEnd != NotFound)
            {
                var ctxTail = ctx.Skip(variableExpressionEnd);
                ThrowIfIsNotEmpty(ctxTail);

                return new SyntaxTree(ExpressionType.Identifier, variable);
            }

            var constantExpressionEnd = FindConstantExpressionEnd(ctx, out var constant);
            if (constantExpressionEnd != NotFound)
            {
                var ctxTail = ctx.Skip(constantExpressionEnd);
                ThrowIfIsNotEmpty(ctxTail);

                return new SyntaxTree(ExpressionType.Identifier, constant);
            }

            return ParseBracketsTerm(ctx);
        }

        private static SyntaxTree ParseBracketsTerm(ParsingContext ctx)
        {
            if (ctx.IsEmpty)
                return ParseTerm(ctx);
            if (!ctx.First.IsLeftBracket())
                return UnexpectedLexeme(ctx);
            ctx = ctx.SkipFirst();

            if (ctx.IsEmpty)
                return ParseTerm(ctx);
            if (ctx.IsEmpty || !ctx.Last.IsRightBracket())
                return UnexpectedLexeme(ctx);
            ctx = ctx.SkipLast();

            return ParseTerm(ctx);
        }

        private static SyntaxTree ExpectedLexemeButEmpty(ParsingContext ctx)
        {
            //TODO надо прокинуть индекс, ибо не неожиданный конец может быть в середине, если парсим левую подформлу
            throw new Exception("Expected lexeme, but empty at ");
        }

        private static SyntaxTree UnexpectedLexeme(ParsingContext ctx)
        {
            //TODO
            throw new Exception($"Unexpected lexeme at {ctx.First.FirstSymbolIndex}-{ctx.First.LastSymbolIndex}");
        }

        private static void ThrowIfIsNotEmpty(ParsingContext ctx)
        {
            if (!ctx.IsEmpty)
                //TODO
                throw new Exception(
                    $"Expected end, but has lexeme at {ctx.First.FirstSymbolIndex}-{ctx.First.LastSymbolIndex}");
        }
    }
}
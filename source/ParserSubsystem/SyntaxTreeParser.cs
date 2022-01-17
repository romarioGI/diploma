using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using InputSubsystem;

namespace ParserSubsystem
{
    public class SyntaxTreeParser : IParser<Symbol, SyntaxTree>
    {
        private const int NotFound = ParsingContext.NotFound;

        public SyntaxTree Parse(IInput<Symbol> input)
        {
            if (input is null)
                throw new Exception("Input is null");

            var lexemes = ToLexemes(input).ToImmutableArray();
            var syntaxTree = ToSyntaxTree(lexemes);

            return syntaxTree;
        }

        private static IEnumerable<Lexeme> ToLexemes(IInput<Symbol> input)
        {
            input.MoveNext();
            while (!input.IsOver)
            {
                if (input.Current.IsWhiteSpace())
                    input.MoveNext();
                if (input.Current.IsLiteralSymbol() || input.Current.IsBackslash())
                    yield return GetLiteralLexeme(input);
                else
                    yield return GetSpecialSymbolLexeme(input);

            }
        }

        private static Lexeme GetSpecialSymbolLexeme(IInput<Symbol> input)
        {
            var lexeme = new SpecialSymbolLexeme(input.Current);
            input.MoveNext();

            return lexeme;
        }

        private static Lexeme GetLiteralLexeme(IInput<Symbol> input)
        {
            return new LiteralLexeme(GetLiteralSymbols(input));
        }

        private static IEnumerable<Symbol> GetLiteralSymbols(IInput<Symbol> input)
        {
            if (input.Current.IsBackslash())
            {
                yield return input.Current;
                if (!input.MoveNext())
                    yield break;
            }

            if (input.Current.IsLiteralSymbol())
                yield return input.Current;
            else
                yield break;

            while (input.MoveNext())
                if (input.Current.IsLiteralSymbol())
                    yield return input.Current;
                else
                    break;
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
                digitLexeme.ToString()
            );

            return ctx.LexemeCount - curCtx.LexemeCount;
        }

        private static OperatorToken LexemeToOperatorToken(Lexeme lexeme)
        {
            return new(lexeme.ToOperatorName());
        }

        private static IdentifierToken LexemeToIdentifierToken(params Lexeme[] lexemes)
        {
            var representation = string.Join<Lexeme>(string.Empty, lexemes);
            return new IdentifierToken(IdentifierType.Variable, representation);
        }

        private static SyntaxTree ParseInfixConnectiveFormula(ParsingContext ctx)
        {
            var connectiveIndex = ctx.FindLastWithZeroBracketBalance(l => l.IsInfixPropositionalConnective());
            if (connectiveIndex == NotFound)
                return ParsePrefixConnectiveFormula(ctx);

            var (leftSubFormulaCtx, tailCtx) = ctx.SplitIntoTwoParts(connectiveIndex);
            var connectiveLexeme = tailCtx.First;
            var rightSubFormulaCtx = tailCtx.SkipFirst();

            var leftSubFormula = ParseInfixConnectiveFormula(leftSubFormulaCtx);
            var connectiveToken = LexemeToOperatorToken(connectiveLexeme);
            var rightSubFormula = ParsePrefixConnectiveFormula(rightSubFormulaCtx);

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
            var functionIndex = ctx.FindLastWithZeroBracketBalance(l => l.IsPlusOrMinus());
            if (functionIndex == NotFound)
                return ParseInfixMultiOrDivideFunctionTerm(ctx);

            var (leftTermCtx, tailCtx) = ctx.SplitIntoTwoParts(functionIndex);
            var functionLexeme = tailCtx.First;
            var rightTermCtx = tailCtx.SkipFirst();

            var leftTerm = ParseInfixPlusOrMinusFunctionTerm(leftTermCtx);
            var functionToken = LexemeToOperatorToken(functionLexeme);
            var rightTerm = ParseInfixMultiOrDivideFunctionTerm(rightTermCtx);

            return new SyntaxTree(ExpressionType.Term, functionToken, leftTerm, rightTerm);
        }

        private static SyntaxTree ParseInfixMultiOrDivideFunctionTerm(ParsingContext ctx)
        {
            var functionIndex = ctx.FindLastWithZeroBracketBalance(l => l.IsMultiOrDivide());
            if (functionIndex == NotFound)
                return ParseExponentiationFunctionTerm(ctx);

            var (leftTermCtx, tailCtx) = ctx.SplitIntoTwoParts(functionIndex);
            var functionLexeme = tailCtx.First;
            var rightTermCtx = tailCtx.SkipFirst();

            var leftTerm = ParseInfixMultiOrDivideFunctionTerm(leftTermCtx);
            var functionToken = LexemeToOperatorToken(functionLexeme);
            var rightTerm = ParseExponentiationFunctionTerm(rightTermCtx);

            return new SyntaxTree(ExpressionType.Term, functionToken, leftTerm, rightTerm);
        }

        private static SyntaxTree ParseExponentiationFunctionTerm(ParsingContext ctx)
        {
            var functionIndex = ctx.FindFirstWithZeroBracketBalance(l => l.IsExponentiation());
            if (functionIndex == NotFound)
                return ParseUnaryFunctionTerm(ctx);

            var (leftTermCtx, tailCtx) = ctx.SplitIntoTwoParts(functionIndex);
            var functionLexeme = tailCtx.First;
            var rightTermCtx = tailCtx.SkipFirst();

            var leftTerm = ParseUnaryFunctionTerm(leftTermCtx);
            var functionToken = LexemeToOperatorToken(functionLexeme);
            var rightTerm = ParseExponentiationFunctionTerm(rightTermCtx);

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
            throw new Exception($"Expected lexeme, but empty");
        }

        private static SyntaxTree UnexpectedLexeme(ParsingContext ctx)
        {
            throw new Exception($"Unexpected lexeme at {ctx.First.FirstSymbolIndex}-{ctx.First.LastSymbolIndex}");
        }

        private static void ThrowIfIsNotEmpty(ParsingContext ctx)
        {
            if (!ctx.IsEmpty)
                throw new Exception(
                    $"Expected end, but has lexeme at {ctx.First.FirstSymbolIndex}-{ctx.First.LastSymbolIndex}");
        }
    }
}
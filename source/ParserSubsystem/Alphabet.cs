using System;
using InputSubsystem;

namespace ParserSubsystem
{
    public static class Alphabet
    {
        public static bool IsLiteralSymbol(this Symbol symbol)
        {
            return char.IsLetterOrDigit(symbol.Character);
        }

        public static bool IsBackslash(this Symbol symbol)
        {
            return symbol.Character == '\\';
        }

        public static bool IsWhiteSpace(this Symbol symbol)
        {
            return char.IsWhiteSpace(symbol.Character);
        }

        public static bool IsLeftBracket(this Lexeme lexeme)
        {
            return lexeme is SpecialSymbolLexeme && lexeme.ToString() == "(";
        }

        public static bool IsRightBracket(this Lexeme lexeme)
        {
            return lexeme is SpecialSymbolLexeme && lexeme.ToString() == ")";
        }

        public static bool IsUnderlining(this Lexeme lexeme)
        {
            return lexeme is SpecialSymbolLexeme && lexeme.ToString() == "_";
        }

        private static bool IsExistentialQuantifier(this Lexeme lexeme)
        {
            return lexeme is LiteralLexeme && lexeme.ToString() == "\\exists";
        }

        private static bool IsUniversalQuantifier(this Lexeme lexeme)
        {
            return lexeme is LiteralLexeme && lexeme.ToString() == "\\forall";
        }

        public static bool IsQuantifier(this Lexeme lexeme)
        {
            return lexeme.IsExistentialQuantifier() || lexeme.IsUniversalQuantifier();
        }

        private static bool IsConjunction(this Lexeme lexeme)
        {
            return lexeme is LiteralLexeme && lexeme.ToString() == "\\land";
        }

        private static bool IsDisjunction(this Lexeme lexeme)
        {
            return lexeme is LiteralLexeme && lexeme.ToString() == "\\lor";
        }

        private static bool IsImplication(this Lexeme lexeme)
        {
            return lexeme is LiteralLexeme && lexeme.ToString() == "\\to";
        }

        public static bool IsInfixPropositionalConnective(this Lexeme lexeme)
        {
            return lexeme.IsConjunction() || lexeme.IsDisjunction() || lexeme.IsImplication();
        }

        private static bool IsNegation(this Lexeme lexeme)
        {
            return lexeme is LiteralLexeme && lexeme.ToString() == "\\lnot";
        }

        public static bool IsPrefixPropositionalConnective(this Lexeme lexeme)
        {
            return lexeme.IsNegation();
        }

        private static bool IsLessPredicate(this Lexeme lexeme)
        {
            return lexeme is SpecialSymbolLexeme && lexeme.ToString() == "<";
        }

        private static bool IsMorePredicate(this Lexeme lexeme)
        {
            return lexeme is SpecialSymbolLexeme && lexeme.ToString() == ">";
        }

        private static bool IsEqualPredicate(this Lexeme lexeme)
        {
            return lexeme is SpecialSymbolLexeme && lexeme.ToString() == "=";
        }

        public static bool IsInfixPredicate(this Lexeme lexeme)
        {
            return lexeme.IsLessPredicate() || lexeme.IsMorePredicate() || lexeme.IsEqualPredicate();
        }

        private static bool IsPlus(this Lexeme lexeme)
        {
            return lexeme is SpecialSymbolLexeme && lexeme.ToString() == "+";
        }

        public static bool IsMinus(this Lexeme lexeme)
        {
            return lexeme is SpecialSymbolLexeme && lexeme.ToString() == "-";
        }

        public static bool IsPlusOrMinus(this Lexeme lexeme)
        {
            return lexeme.IsPlus() || lexeme.IsMinus();
        }

        private static bool IsMulti(this Lexeme lexeme)
        {
            return lexeme is SpecialSymbolLexeme && lexeme.ToString() == "*";
        }

        private static bool IsDivide(this Lexeme lexeme)
        {
            return lexeme is SpecialSymbolLexeme && lexeme.ToString() == "/"
                   || lexeme is LiteralLexeme && lexeme.ToString() == "\\over";
        }

        public static bool IsMultiOrDivide(this Lexeme lexeme)
        {
            return lexeme.IsMulti() || lexeme.IsDivide();
        }

        public static bool IsExponentiation(this Lexeme lexeme)
        {
            return lexeme is SpecialSymbolLexeme && lexeme.ToString() == "^";
        }

        public static OperatorName ToOperatorName(this Lexeme lexeme)
        {
            if (lexeme.IsExistentialQuantifier())
                return OperatorName.ExistentialQuantifier;
            if (lexeme.IsUniversalQuantifier())
                return OperatorName.UniversalQuantifier;
            if (lexeme.IsConjunction())
                return OperatorName.Conjunction;
            if (lexeme.IsDisjunction())
                return OperatorName.Disjunction;
            if (lexeme.IsImplication())
                return OperatorName.Implication;
            if (lexeme.IsNegation())
                return OperatorName.Negation;
            if (lexeme.IsLessPredicate())
                return OperatorName.Less;
            if (lexeme.IsMorePredicate())
                return OperatorName.More;
            if (lexeme.IsEqualPredicate())
                return OperatorName.Equal;
            if (lexeme.IsPlus())
                return OperatorName.Plus;
            if (lexeme.IsMinus())
                return OperatorName.Minus;
            if (lexeme.IsMulti())
                return OperatorName.Multi;
            if (lexeme.IsDivide())
                return OperatorName.Divide;
            if (lexeme.IsExponentiation())
                return OperatorName.Exponentiation;
            throw new NotSupportedException();
        }

        public static bool IsConnective(this OperatorName name)
        {
            return name == OperatorName.Conjunction
                   || name == OperatorName.Disjunction
                   || name == OperatorName.Implication
                   || name == OperatorName.Negation;
        }

        public static bool IsQuantifier(this OperatorName name)
        {
            return name == OperatorName.ExistentialQuantifier
                   || name == OperatorName.UniversalQuantifier;
        }

        public static bool IsPredicate(this OperatorName name)
        {
            return name == OperatorName.Less
                   || name == OperatorName.More
                   || name == OperatorName.Equal;
        }
    }
}
using System;
using System.Collections.Generic;
using InputSubsystem;

namespace ParserSubsystem
{
    public class SimpleParser : IParser<Symbol, SyntaxTree>
    {
        public SyntaxTree Parse(IInput<Symbol> input)
        {
            var lexemes = ToLexemes(input);
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
                if (IsEmptySymbol(enumerator.Current))
                    enumerator.MoveNext();
                else if (IsLiteralSymbol(enumerator.Current))
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
            do
            {
                if (IsLiteralSymbol(enumerator.Current))
                    yield return enumerator.Current;
                else
                    break;
            } while (enumerator.MoveNext());
        }

        private static bool IsLiteralSymbol(Symbol symbol)
        {
            return symbol is not null &&
                   (char.IsLetter(symbol.Character) || char.IsDigit(symbol.Character));
        }

        private static bool IsEmptySymbol(Symbol symbol)
        {
            return char.IsWhiteSpace(symbol.Character);
        }

        private static SyntaxTree ToSyntaxTree(IEnumerable<Lexeme> lexemes)
        {
            throw new NotImplementedException();
        }
    }
}
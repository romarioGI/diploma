using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ParserSubsystem
{
    internal class ParsingContext
    {
        public const int NotFound = -1;

        private readonly ImmutableArray<Lexeme> _lexemes;
        private readonly int _begin;
        private readonly int _end;

        public ParsingContext(ImmutableArray<Lexeme> lexemes)
            : this(lexemes, 0, lexemes.Length)
        {
        }

        private ParsingContext(ImmutableArray<Lexeme> lexemes, int begin, int end)
        {
            _lexemes = lexemes;
            _begin = begin;
            _end = end;
        }

        public int LexemeCount => _end - _begin;

        private Lexeme GetLexeme(int index)
        {
            return _lexemes[_begin + index];
        }

        public (ParsingContext, ParsingContext) SplitIntoTwoParts(int firstPartEnd)
        {
            if (firstPartEnd < 0 || firstPartEnd > LexemeCount)
                throw new IndexOutOfRangeException();

            var leftPart = new ParsingContext(_lexemes, _begin, _begin + firstPartEnd);
            var rightPart = new ParsingContext(_lexemes, _begin + firstPartEnd, _end);

            return (leftPart, rightPart);
        }

        public ParsingContext Skip(int skipPartLength)
        {
            return SplitIntoTwoParts(skipPartLength).Item2;
        }

        private ParsingContext Take(int takePartLength)
        {
            return SplitIntoTwoParts(takePartLength).Item1;
        }

        public ParsingContext SkipFirst()
        {
            return Skip(1);
        }

        public ParsingContext SkipLast()
        {
            return Take(LexemeCount - 1);
        }

        public bool IsEmpty => LexemeCount == 0;

        public Lexeme First => GetLexeme(0);

        public Lexeme Last => GetLexeme(LexemeCount - 1);

        private IEnumerable<int> FindAllWithZeroBracketBalance(Func<Lexeme, bool> predicate)
        {
            var bracketBalance = 0;
            var count = 0;
            for (var i = _begin; i < _end; ++i)
            {
                var lexeme = _lexemes[i];
                if (lexeme.IsLeftBracket())
                    ++bracketBalance;
                if (lexeme.IsRightBracket())
                    --bracketBalance;
                switch (bracketBalance)
                {
                    case < 0:
                        //TODO
                        throw new Exception();
                    case 0 when predicate(lexeme):
                        yield return i - _begin;
                        ++count;
                        break;
                }
            }

            if (count == 0)
                yield return NotFound;
        }

        public int FindFirstWithZeroBracketBalance(Func<Lexeme, bool> predicate)
        {
            return FindAllWithZeroBracketBalance(predicate).First();
        }

        public int FindLastWithZeroBracketBalance(Func<Lexeme, bool> predicate)
        {
            return FindAllWithZeroBracketBalance(predicate).Last();
        }
    }
}
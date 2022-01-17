using System;
using System.Collections.Generic;
using System.Linq;
using ParserSubsystem;

namespace ProcessorsSubsystem
{
    public static class ExtensionMethods
    {
        public static bool Any(this SyntaxTree syntaxTree, Func<Token, bool> predicate)
        {
            return predicate(syntaxTree.Token)
                   || syntaxTree.Operands.Any(s => s.Any(predicate));
        }

        public static IEnumerable<SyntaxTree> GetAll(this SyntaxTree syntaxTree, Func<SyntaxTree, bool> predicate)
        {
            if (predicate(syntaxTree))
                yield return syntaxTree;
            else
                foreach (var res in syntaxTree.Operands.SelectMany(operand => operand.GetAll(predicate)))
                    yield return res;
        }
        
        public static Sign Invert(this Sign sign)
        {
            return sign switch
            {
                Sign.None => Sign.None,
                Sign.LessZero => Sign.MoreZero,
                Sign.MoreZero => Sign.LessZero,
                Sign.EqualZero => Sign.EqualZero,
                Sign.NotLess => Sign.NotMore,
                Sign.NotMore => Sign.NotLess,
                Sign.NotEqual => Sign.NotEqual,
                Sign.Any => Sign.Any,
                _ => throw new NotSupportedException()
            };
        }
    }
}
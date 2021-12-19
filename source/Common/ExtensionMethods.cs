using System;
using System.Collections.Generic;

namespace Common
{
    //TODO [NTH] add small tests
    public static class ExtensionMethods
    {
        private static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> source)
        {
            if (source.Current is null)
                yield break;
            yield return source.Current;
            while (source.MoveNext())
                yield return source.Current;
        }

        public static IEnumerable<T> CutFirst<T>(this IEnumerable<T> source, out T first)
        {
            using var enumerator = source.GetEnumerator();
            first = enumerator.Current;
            enumerator.MoveNext();

            return enumerator.ToIEnumerable();
        }

        public static T DoActionAndReturnLast<T>(this IEnumerable<T> source, Action<T> action)
        {
            using var enumerator = source.GetEnumerator();
            var prev = enumerator.Current;
            while (enumerator.MoveNext())
            {
                action(prev);
                prev = enumerator.Current;
            }

            return prev;
        }
    }
}
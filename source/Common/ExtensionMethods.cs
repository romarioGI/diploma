using System;
using System.Collections.Generic;

namespace Common
{
    public static class ExtensionMethods
    {
        private static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> source)
        {
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

        public static void DoAction<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var s in source)
                action(s);
        }
    }
}
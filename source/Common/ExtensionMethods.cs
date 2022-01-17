using System;
using System.Collections.Generic;

namespace Common
{
    public static class ExtensionMethods
    {
        private static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> source)
        {
            yield return source.Current;
            while (source.MoveNext())
                yield return source.Current;
        }

        public static IEnumerable<T> GetFirstOrDefault<T>(this IEnumerable<T> source, out T first)
        {
            using var enumerator = source.GetEnumerator();
            first = enumerator.MoveNext() ? enumerator.Current : default;
            return ToIEnumerable(enumerator);
        }

        public static T DoActionAndReturnLast<T>(this IEnumerable<T> source, Action<T> action)
        {
            var prev = default(T);
            foreach (var cur in source)
            {
                action(cur);
                prev = cur;
            }

            return prev;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PaperIoStrategy.AISolver
{
    public static class EnumerableExtention
    {
        public static T MinSingle<T>(this IEnumerable<T> collection, Func<T, int> selector) => collection.Aggregate((r, x) => selector(r) < selector(x) ? r : x);

        public static IEnumerable<T> Min<T>(this IEnumerable<T> collection, Func<T, int> selector)
        {
            var enumerable = collection as T[] ?? collection.ToArray();
            var min = enumerable.Select(selector).Min();
            return enumerable.Where(t => selector(t) == min);
        }

        public static T MaxSingle<T>(this IEnumerable<T> collection, Func<T, int> selector) => collection.Aggregate((r, x) => selector(r) > selector(x) ? r : x);

        public static IEnumerable<T> Max<T>(this IEnumerable<T> collection, Func<T, int> selector)
        {
            var enumerable = collection as T[] ?? collection.ToArray();
            var max = enumerable.Select(selector).Max();
            return enumerable.Where(t => selector(t) == max);
        }

        public static IEnumerable<T> While<T>(this IEnumerable<T> collection, Func<T, bool> selector)
        {
            foreach (var item in collection)
                if (selector(item)) yield return item;
                else yield break;
        }

    }
}
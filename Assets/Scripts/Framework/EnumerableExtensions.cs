using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    public static class EnumerableExtensions
    {
        public static bool IsSubsetOf<T>(this IEnumerable<T> x, IEnumerable<T> y)
        {
            return !x.Except(y).Any();
        }

        /// <summary>
        /// Выполняет действие над коллекцией.
        /// </summary>
        public static void ForAll<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        /// <summary>
        /// Выполняет действие над коллекцией как списком.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection.ToList())
            {
                action(item);
            }
        }

        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            return new[] { item };
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, T item)
        {
            return collection.Except(item.ToEnumerable());
        }

        public static IEnumerable<T> Union<T>(this IEnumerable<T> collection, T item)
        {
            return collection.Union(item.ToEnumerable());
        }

        public static T[] Concat<T>(this T[] x, T[] y)
        {
            var z = new T[x.Length + y.Length];
            x.CopyTo(z, 0);
            y.CopyTo(z, x.Length);
            return z;
        }

        public static bool IsUnique<T>(this IEnumerable<T> collection)
        {
            return collection.Distinct().Count() == collection.Count();
        }

        /// <summary>
        /// True if two lists contain same items.
        /// </summary>
        public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }
    }
}
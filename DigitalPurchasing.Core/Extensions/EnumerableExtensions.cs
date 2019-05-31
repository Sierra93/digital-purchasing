using System.Collections.Generic;
using System.Linq;

namespace DigitalPurchasing.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> items, T first)
        {
            yield return first;
            foreach (var item in items)
                yield return item;
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> items)
        {
            if (!items.Any())
            {
                yield return items;
            }
            else
            {
                var head = items.First();
                var tail = items.Skip(1);
                foreach (var sequence in tail.Combinations())
                {
                    yield return sequence;
                    yield return sequence.Prepend(head);
                }
            }
        }

        public static string JoinNotEmpty(this IEnumerable<string> values, string separator = ",") =>
            string.Join(separator, values.Where(i => !string.IsNullOrWhiteSpace(i)));
    }
}

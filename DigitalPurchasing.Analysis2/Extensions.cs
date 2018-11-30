using System.Collections.Generic;
using System.Linq;

namespace DigitalPurchasing.Analysis2
{
    public static class Extensions
    {
        public static List<List<T>> CartesianProduct<T>(
            this List<List<T>> sequences)
        {
            var accum = new List<List<T>>();
            var list = sequences.ToList();
            if (list.Count > 0)
                CartesianRecurse(accum, new Stack<T>(), list, list.Count - 1);
            return accum;
        }

        static void CartesianRecurse<T>(List<List<T>> accum, Stack<T> stack,
            List<List<T>> list, int index)
        {
            foreach (var item in list[index])
            {
                stack.Push(item);
                if (index == 0)
                    accum.Add(stack.ToList());
                else
                    CartesianRecurse(accum, stack, list, index - 1);
                stack.Pop();
            }
        }
    }
}
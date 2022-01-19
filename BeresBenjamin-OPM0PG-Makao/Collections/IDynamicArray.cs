using System;
using System.Text;

namespace Makao.Collections
{
    public static class IDynamicArrayExtension
    {
        private static StringBuilder sb = new StringBuilder();

        public static bool All<T>(this IDynamicArray<T> collection, Func<T, bool> P)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (P is null)
            {
                throw new ArgumentNullException(nameof(P));
            }

            int i = 0;

            while (i < collection.Count && P(collection[i]))
            {
                i++;
            }
            return i > collection.Count;
        }

        public static bool Contains<T>(this IDynamicArray<T> collection, Func<T, bool> P, out int index)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (P is null)
            {
                throw new ArgumentNullException(nameof(P));
            }

            int i = 0;

            while (i < collection.Count && !P(collection[i]))
            {
                i++;
            }

            index = i;
            return i < collection.Count;
        }

        public static int Count<T>(this IDynamicArray<T> collection, Func<T, bool> P)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (P is null)
            {
                throw new ArgumentNullException(nameof(P));
            }
            int count = 0;

            for (int i = 0; i < collection.Count; i++)
            {
                if (P(collection[i]))
                {
                    count++;
                }
            }

            return count;
        }

        public static bool IsNullOrEmpty<T>(this IDynamicArray<T> collection)
        {
            return collection == null || collection.IsEmpty;
        }

        public static string Join<T>(this IDynamicArray<T> collection, Func<T, string> str, string separator)
        {
            if (!collection.IsNullOrEmpty())
            {
                sb.Append(str(collection[0]));
                for (int i = 1; i < collection.Count; i++)
                {
                    sb.Append(separator);
                    sb.Append(str(collection[i]));
                }
            }

            string join = sb.ToString();
            sb.Clear();
            return join;
        }

        public static string Join<T>(this IDynamicArray<T> collection, string separator)
        {
            return Join(collection, (i) => i.ToString(), separator);
        }

        public static string Join<T>(this T[] array, Func<T, string> str, string separator)
        {
            if (array != null || array.Length != 0)
            {
                sb.Append(str(array[0]));
                for (int i = 1; i < array.Length; i++)
                {
                    sb.Append(separator);
                    sb.Append(str(array[i]));
                }
            }
            string join = sb.ToString();
            sb.Clear();
            return join;
        }

        public static T Max<T>(this IDynamicArray<T> collection, Func<T, int> compare, out int index)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (compare is null)
            {
                throw new ArgumentNullException(nameof(compare));
            }

            int max = 0;

            for (int i = 1; i < collection.Count; i++)
            {
                if (compare(collection[max]) < compare(collection[i]))
                {
                    max = i;
                }
            }

            index = max;
            return collection[max];
        }

        public static T Min<T>(this IDynamicArray<T> collection, Func<T, int> compare, out int index)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (compare is null)
            {
                throw new ArgumentNullException(nameof(compare));
            }

            int min = 0;

            for (int i = 1; i < collection.Count; i++)
            {
                if (compare(collection[min]) > compare(collection[i]))
                {
                    min = i;
                }
            }

            index = min;
            return collection[min];
        }

        public static IDynamicArray<T> Select<T>(this IDynamicArray<T> collection, Func<T, bool> P)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (P is null)
            {
                throw new ArgumentNullException(nameof(P));
            }

            DynamicArray<T> items = new DynamicArray<T>();

            for (int i = 0; i < collection.Count; i++)
            {
                if (P(collection[i]))
                {
                    items.Add(collection[i]);
                }
            }

            return items;
        }

        public static int Sum<T>(this IDynamicArray<T> collection, Func<T, int> toInt)
        {
            int sum = 0;
            for (int i = 0; i < collection.Count; i++)
            {
                sum += toInt(collection[i]);
            }
            return sum;
        }
    }

    public interface IDynamicArray<T>
    {
        public int Count { get; }
        public bool IsEmpty { get; }
        public T this[int index] { get; }

        public bool Contains(T item, out int index);

        public DynamicArray<T> CopyAsDynamicArray();
    }
}
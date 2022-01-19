namespace Makao.Collections
{
    public class ReadOnlyDynamicArray<T> : IDynamicArray<T>
    {
        private readonly DynamicArray<T> items;
        public static readonly ReadOnlyDynamicArray<T> Empty = new ReadOnlyDynamicArray<T>(new DynamicArray<T>(capacity: 0));
        public int Count => items.Count;
        public bool IsEmpty => items.IsEmpty;
        public T this[int index] => items[index];

        public ReadOnlyDynamicArray(DynamicArray<T> items)
        {
            this.items = items;
        }

        public ReadOnlyDynamicArray(T[] items)
        {
            this.items = new DynamicArray<T>(items);
        }

        public bool Contains(T item, out int index)
        {
            return items.Contains(item, out index);
        }

        public T[] Copy()
        {
            return items.Copy();
        }

        public DynamicArray<T> CopyAsDynamicArray()
        {
            return items.CopyAsDynamicArray();
        }

        public override string ToString()
        {
            return items.ToString();
        }

        public bool TryGetFirstItem(out T firstItem)
        {
            return items.TryGetFirstItem(out firstItem);
        }

        public bool TryGetLastItem(out T lastItem)
        {
            return items.TryGetLastItem(out lastItem);
        }
    }
}
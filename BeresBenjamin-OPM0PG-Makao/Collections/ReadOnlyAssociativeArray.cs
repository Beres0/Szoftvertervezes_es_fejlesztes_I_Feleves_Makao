namespace Makao.Collections
{
    public class ReadOnlyAssociativeArray<TKey, TValue> : IDynamicArray<KeyValuePair<TKey, TValue>>
    {
        private AssociativeArray<TKey, TValue> Items { get; }
        public int Count => Items.Count;
        public bool IsEmpty => Items.IsEmpty;
        public KeyValuePair<TKey, TValue> this[int index] => Items[index];

        public ReadOnlyAssociativeArray(AssociativeArray<TKey, TValue> items)
        {
            Items = items;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item, out int index)
        {
            return Items.Contains(item, out index);
        }

        public bool ContainsKey(TKey key, out int index)
        {
            return Items.ContainsKey(key, out index);
        }

        public bool ContainsValue(TValue value, out int index)
        {
            return Items.ContainsValue(value, out index);
        }

        public KeyValuePair<TKey, TValue>[] Copy()
        {
            return Items.Copy();
        }

        public AssociativeArray<TKey, TValue> CopyAsAssociativeArray()
        {
            return Items.CopyAsAssociativeArray();
        }

        public DynamicArray<KeyValuePair<TKey, TValue>> CopyAsDynamicArray()
        {
            return CopyAsDynamicArray();
        }

        public TKey GetKey(int index)
        {
            return Items.GetKey(index);
        }

        public TValue GetValue(int index)
        {
            return Items.GetValue(index);
        }

        public override string ToString()
        {
            return Items.ToString();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Items.TryGetValue(key, out value);
        }
    }
}
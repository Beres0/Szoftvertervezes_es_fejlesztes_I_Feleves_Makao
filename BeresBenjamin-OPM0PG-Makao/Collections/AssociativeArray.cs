namespace Makao.Collections
{
    public class AssociativeArray<TKey, TValue> : IDynamicArray<KeyValuePair<TKey, TValue>>
    {
        private DynamicArray<KeyValuePair<TKey, TValue>> Items { get; }

        public int Count => Items.Count;
        public bool IsEmpty => Items.IsEmpty;
        public KeyValuePair<TKey, TValue> this[int index] => Items[index];

        public AssociativeArray()
        {
            Items = new DynamicArray<KeyValuePair<TKey, TValue>>();
        }

        public AssociativeArray(KeyValuePair<TKey, TValue>[] items) : this()
        {
            for (int i = 0; i < items.Length; i++)
            {
                Add(items[i].Key, items[i].Value);
            }
        }

        public bool Add(TKey key, TValue value)
        {
            return Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public bool Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            if (!ContainsKey(keyValuePair.Key, out int index))
            {
                Items.Add(keyValuePair);
                return true;
            }
            else return false;
        }

        public ReadOnlyAssociativeArray<TKey, TValue> AsReadOnly()
        {
            return new ReadOnlyAssociativeArray<TKey, TValue>(this);
        }

        public void Clear()
        {
            Items.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item, out int index)
        {
            return Items.Contains(item, out index);
        }

        public bool ContainsKey(TKey key, out int index)
        {
            int i = 0;
            while (i < Count && !key.Equals(Items[i].Key))
            {
                i++;
            }
            index = i;
            return i < Count;
        }

        public bool ContainsValue(TValue value, out int index)
        {
            int i = 0;
            while (i < Count && !value.Equals(Items[i].Value))
            {
                i++;
            }
            index = i;
            return i < Count;
        }

        public KeyValuePair<TKey, TValue>[] Copy()
        {
            return Items.Copy();
        }

        public AssociativeArray<TKey, TValue> CopyAsAssociativeArray()
        {
            return new AssociativeArray<TKey, TValue>(Items.Copy());
        }

        public DynamicArray<KeyValuePair<TKey, TValue>> CopyAsDynamicArray()
        {
            return Items.CopyAsDynamicArray();
        }

        public TKey GetKey(int index)
        {
            return Items[index].Key;
        }

        public TValue GetValue(int index)
        {
            return Items[index].Value;
        }

        public bool Insert(int index, TKey key, TValue value)
        {
            if (ContainsKey(key, out int keyIndex))
            {
                Items.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
                return true;
            }
            return false;
        }

        public bool Remove(TKey key)
        {
            if (ContainsKey(key, out int index))
            {
                Items.RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            Items.RemoveAt(index);
        }

        public void SetValue(int index, TValue value)
        {
            TKey key = GetKey(index);
            Items[index] = new KeyValuePair<TKey, TValue>(key, value);
        }

        public void SetValue(TKey key, TValue value)
        {
            if (ContainsKey(key, out int index))
            {
                Items[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else
            {
                Add(key, value);
            }
        }

        public override string ToString()
        {
            return Items.ToString();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default;
            if (ContainsKey(key, out int index))
            {
                value = GetValue(index);
                return true;
            }
            else return false;
        }

        public bool TryPopValue(TKey key, out TValue value)
        {
            value = default;
            if (ContainsKey(key, out int index))
            {
                value = Items[index].Value;
                RemoveAt(index);
                return true;
            }
            else return false;
        }
    }
}
using System;
using System.Text;

namespace Makao.Collections
{
    public class DynamicArray<T> : IDynamicArray<T>
    {
        private static int defaultCapacity = 4;

        private int count;

        private T[] items;

        public int Count
        {
            get => count;

            private set
            {
                int capacity = items.Length;
                if (capacity < value)
                {
                    while (capacity < value)
                    {
                        capacity <<= 1;
                    }

                    T[] resized = new T[capacity];
                    Copy(items, count, resized, 0);
                    items = resized;
                }
                count = value;
            }
        }

        public bool IsEmpty => Count < 1;

        public T this[int index]
        {
            get
            {
                CheckIndex(index, 0);
                return items[index];
            }
            set
            {
                CheckIndex(index, 0);
                items[index] = value;
            }
        }

        public DynamicArray(int capacity)
        {
            items = new T[capacity];
            Count = 0;
        }

        public DynamicArray() : this(defaultCapacity)
        { }

        public DynamicArray(T[] array)
        {
            if (array == null || array.Length == 0)
            {
                items = new T[defaultCapacity];
            }
            else
            {
                items = new T[array.Length];
                Copy(array, array.Length, items, 0);
                count = array.Length;
            }
        }

        private static void Copy(T[] source, int count, T[] destination, int startIndex)
        {
            for (int i = 0; i < count; i++)
            {
                destination[startIndex + i] = source[i];
            }
        }

        private void CheckIndex(int index, int countOffset)
        {
            int count = Count + countOffset;
            if (index >= count)
            {
                throw new IndexOutOfRangeException($"Index '{index}' was out of range '{count}'.");
            }
        }

        public void Add(T item)
        {
            int originalCount = Count;
            Count++;
            items[originalCount] = item;
        }

        public virtual void Append(T item)
        {
            Insert(0, item);
        }

        public ReadOnlyDynamicArray<T> AsReadOnly()
        {
            return new ReadOnlyDynamicArray<T>(this);
        }

        public void Clear()
        {
            Count = 0;
            items = new T[defaultCapacity];
        }

        public bool Contains(T item, out int index)
        {
            int i = 0;
            while (i < Count && !item.Equals(items[i]))
            {
                i++;
            }

            index = i;

            return i < Count;
        }

        public T[] Copy()
        {
            T[] items = new T[count];

            for (int i = 0; i < count; i++)
            {
                items[i] = this.items[i];
            }
            return items;
        }

        public DynamicArray<T> CopyAsDynamicArray()
        {
            return new DynamicArray<T>(Copy());
        }

        public void Insert(int index, T item)
        {
            CheckIndex(index, 1);
            Count++;

            for (int i = index; i < Count; i++)
            {
                T temp = items[i];
                items[i] = item;
                item = temp;
            }
        }

        public T PopAt(int index)
        {
            CheckIndex(index, 0);
            T item = items[index];
            RemoveAt(index);
            return item;
        }

        public bool Remove(T item)
        {
            if (Contains(item, out int index))
            {
                RemoveAt(index);
                return true;
            }
            else return false;
        }

        public void RemoveAt(int index)
        {
            CheckIndex(index, 0);
            Count--;

            for (int i = index; i < Count; i++)
            {
                items[i] = items[i + 1];
            }
        }

        public override string ToString()
        {
            if (!IsEmpty)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(items[0]);
                for (int i = 1; i < Count; i++)
                {
                    sb.Append(',' + items[i].ToString());
                }
                return sb.ToString();
            }
            else return base.ToString();
        }

        public bool TryGetFirstItem(out T firstItem)
        {
            firstItem = default;
            if (!IsEmpty)
            {
                firstItem = items[Count - 1];
                return true;
            }
            return false;
        }

        public bool TryGetFromTheBack(int n, out T item)
        {
            item = default;
            bool isValid = Count - 1 >= n && n >= 0;
            if (isValid)
            {
                item = items[Count - 1 - n];
            }
            return isValid;
        }

        public bool TryGetLastItem(out T lastItem)
        {
            lastItem = default;
            if (!IsEmpty)
            {
                lastItem = items[Count - 1];
                return true;
            }
            return false;
        }

        public bool TryPopFirst(out T item)
        {
            item = default;
            if (!IsEmpty)
            {
                item = PopAt(0);
                return true;
            }
            else return false;
        }

        public bool TryPopLast(out T item)
        {
            item = default;
            if (!IsEmpty)
            {
                Count--;
                item = items[Count];
                return true;
            }
            else return false;
        }
    }
}
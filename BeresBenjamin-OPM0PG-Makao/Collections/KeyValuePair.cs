using System;

namespace Makao.Collections
{
    public struct KeyValuePair<TKey, TValue>
    {
        public TKey Key { get; }
        public TValue Value { get; }

        public KeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public static bool operator !=(KeyValuePair<TKey, TValue> left, KeyValuePair<TKey, TValue> right)
        {
            return !(left == right);
        }

        public static bool operator ==(KeyValuePair<TKey, TValue> left, KeyValuePair<TKey, TValue> right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return obj is KeyValuePair<TKey, TValue> pair && pair.Key.Equals(Key) && pair.Value.Equals(Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key);
        }

        public override string ToString()
        {
            return $"[{Key},{Value}]";
        }
    }
}
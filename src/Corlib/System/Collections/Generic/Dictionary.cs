//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System.Collections.Generic
{
    internal class Dictionary<TKey, TValue>
    {
        private readonly List<TKey> Keys;
        private readonly List<TValue> Values;

        public int Count => Values.Count;
        public TValue this[TKey key]
        {
            get => Values[Keys.IndexOf(key)];
            set => Values[Keys.IndexOf(key)] = value;
        }

        public Dictionary()
        {
            Keys = new();
            Values = new();
        }

        public Dictionary(int capacity)
        {
            Keys = new(capacity);
            Values = new(capacity);
        }

        public void Add(TKey key, TValue value)
        {
            Keys.Add(key);
            Values.Add(value);
        }

        public void Clear()
        {
            Keys.Clear();
            Values.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return Keys.IndexOf(key) != -1;
        }

        public bool ContainsValue(TValue value)
        {
            return Values.IndexOf(value) != -1;
        }

        public void Remove(TKey key)
        {
            Values.Remove(Values[Keys.IndexOf(key)]);
            Keys.Remove(key);
        }

        public override void Dispose()
        {
            Keys.Clear();
            Values.Clear();
            Values.Dispose();
            Keys.Dispose();
            base.Dispose();
        }
    }
}

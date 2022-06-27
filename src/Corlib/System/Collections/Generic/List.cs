//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.
namespace System.Collections.Generic
{
    public class List<T>
    {
        private readonly T[] _value;

        public int Count = 0;

        public List(int initsize = 256)
        {
            _value = new T[initsize];
        }
        public List(Array array)
        {
            _value = (T[])array;
        }

        public T this[int index]
        {
            get => _value[index];
            set => _value[index] = value;
        }

        public void Add(T t)
        {
            _value[Count] = t;
            Count++;
        }

        public void Insert(int index, T item, bool internalMove = false)
        {
            if (index == IndexOf(item))
            {
                return;
            }

            if (!internalMove)
            {
                Count++;
            } else
            {
                int _index = IndexOf(item);
                for (int i = _index; i < Count - 1; i++)
                {
                    _value[i] = _value[i + 1];
                }
            }

            for (int i = Count - 1; i > index; i--)
            {
                _value[i] = _value[i - 1];
            }
            _value[index] = item;
        }

        public T[] ToArray()
        {
            T[] array = new T[Count];
            for (int i = 0; i < Count; i++)
            {
                array[i] = this[i];
            }
            return array;
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                object first = this[i];
                object second = item;

                if (first == second)
                {
                    return i;
                }
            }

            return -1;
        }
        public bool Remove(T item)
        {
            int at = IndexOf(item);

            if (at < 0)
            {
                return false;
            }

            RemoveAt(at);

            return true;
        }

        public void RemoveAt(int index)
        {
            Count--;

            for (int i = index; i < Count; i++)
            {
                _value[i] = _value[i + 1];
            }

            _value[Count] = default;
        }

        public override void Dispose()
        {
            _value.Dispose();
            base.Dispose();
        }

        public void Clear()
        {
            Count = 0;
        }

    }
}
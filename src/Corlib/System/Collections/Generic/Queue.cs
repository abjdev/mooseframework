//Copyright © 2022 Contributors of moose-org, This code is licensed under the BSD 3-Clause "New" or "Revised" License.

namespace System.Collections.Generic
{
    public class Queue<T>
    {
        private readonly List<T> list;

        public Queue(int initsize = 256)
        {
            list = new List<T>(initsize);
        }

        public T Tail => Length == 0 ? default : list[list.Count - 1];

        public T Head => Length == 0 ? default : list[0];
        public int Length => list.Count;


        public void Enqueue(T item)
        {
            list.Add(item);
        }

        public T Dequeue()
        {
            if (list.Count == 0)
            {
                return default;
            }

            list.Count--;
            return list[list.Count];
        }
    }
}
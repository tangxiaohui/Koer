using System.Collections.Generic;

namespace UpdateSystem.Data
{
    /// <summary>
    /// 线程安全的队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SyncQueue<T> : Queue<T>
    {
        private object _obj;
        private Queue<T> _queue;

        public new int Count
        {
            get
            {
                lock (_obj)
                {
                    return _queue.Count;
                }
            }
        }

        public SyncQueue()
        {
            _obj = new object();
            _queue = new Queue<T>();
        }

        public new void Enqueue(T arg)
        {
            lock (_obj)
            {
                _queue.Enqueue(arg);
            }
        }

        public new T Peek()
        {
            lock (_obj)
            {
                return _queue.Peek();
            }
        }

        public new T Dequeue()
        {
            lock (_obj)
            {
                return _queue.Dequeue();
            }
        }

        public new bool Contains(T arg)
        {
            lock (_obj)
            {
                return _queue.Contains(arg);
            }
        }

        public new Queue<T>.Enumerator GetEnumerator()
        {
            lock (_obj)
            {
                return _queue.GetEnumerator();
            }
        }

        public new T[] ToArray()
        {
            lock (_obj)
            {
                return _queue.ToArray();
            }
        }


        public new void CopyTo(T[] array, int idx)
        {
            lock (_obj)
            {
                _queue.CopyTo(array, idx);
            }
        }

        public new void Clear()
        {
            lock(_obj)
            {
                _queue.Clear();
            }
        }
    }
}

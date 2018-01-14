using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    public class SingelPool<T> where T : new()
    {
        private static ObjectPool<T> poos;
        public static T Get()
        {
            //return new T();
            if (poos == null)
            {
                poos = getInstance();
            }
            return poos.Get();
        }

        public static void Release(T element)
        {
            if (poos == null)
            {
                poos = getInstance();
            }
            poos.Release(element);
        }

        private static ObjectPool<T> getInstance()
        {
            var m = new ObjectPool<T>();
            PoolHelper.Add(m);
            return m;
        }

        public static void AllRelase()
        {
            poos.Clear();
        }
    }
}

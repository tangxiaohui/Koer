using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    internal static class ListPool<T>
    {
        // Object pool to avoid allocations.
        private static ObjectPool<List<T>> s_ListPool;

        private static ObjectPool<List<T>> getInstance()
        {
            var m = new ObjectPool<List<T>>(null, l =>
            {
                var isReference = PoolHelper.IsReferenceType(typeof(T));
                var a = l.Count;
                for (int i = 0; i < a; i++)
                {
                    if (isReference)
                        l[i] = default(T);
                }
                l.Clear();
            });
            PoolHelper.Add(m);
            return m;
        }

        public static List<T> Get()
        {
            if (s_ListPool == null) s_ListPool=getInstance();
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            if (s_ListPool == null) s_ListPool=getInstance();
            s_ListPool.Release(toRelease);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    /// <summary>
    /// 临时对象池帮助
    /// </summary>
    internal static class PoolHelper
    {
        static List<Pool> m_cache = new List<Pool>();

        internal static void Add(Pool pool)
        {
            m_cache.Add(pool);
        }
        /// <summary>
        /// 清空临时池
        /// </summary>
        internal static void ClearTempPools()
        {
            var count = m_cache.Count;
            for (int i = 0; i < count; i++)
            {
                m_cache[i].Clear();
            }
        }
        internal static bool IsReferenceType(Type type)
        {
            return type.IsAbstract || type.IsArray || type.IsClass || type.IsInterface;
        }
    }
}

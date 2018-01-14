using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Utility
{
    public interface Pool
    {
        void Clear();
    }
    public class ObjectPool<T> : Pool where T : new()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();
        private readonly System.Action<T> m_ActionOnGet;
        private readonly System.Action<T> m_ActionOnRelease;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }



        public ObjectPool(System.Action<T> actionOnGet, System.Action<T> actionOnRelease)
        {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }
        public ObjectPool()
        {

        }
        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
                countAll++;
            }
            else
            {
                element = m_Stack.Pop();
            }
            if (m_ActionOnGet != null)
                m_ActionOnGet(element);
            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
            {
                Common.UDebug.LogError("尝试销毁一个已经在队列里面存在的元素");
                return;
            }
            if (m_ActionOnRelease != null)
                m_ActionOnRelease(element);
            m_Stack.Push(element);
        }
        public void Clear()
        {
            while (m_Stack.Count > 0)
            {
                m_Stack.Pop();
            }
        }
    }
}

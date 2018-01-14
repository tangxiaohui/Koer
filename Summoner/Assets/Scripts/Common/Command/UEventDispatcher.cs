using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Event
{
    public static class EventDispatcher
    {
        public readonly static UEventController EventController;
        static EventDispatcher()
        {
            EventController = new UEventController();
        }
        public static void AddListener(string eventType, EventListenerDelegate callback)
        {
            EventController.AddEventListener(eventType, callback);
        }

        public static void RemoveListener(string eventType, EventListenerDelegate callback)
        {
            EventController.RemoveEventListener(eventType, callback);
        }

        public static void Dispatch(string eventType,object target,params object[] obj)
        {
            var u = new UEvent();
            u.eventType = eventType;
            u.eventParams = obj;
            u.target = target;
            EventController.DispatchEvent(u, target);
        }
    }


    public class UEventController
    {
        protected IList<UEventListener> eventListenerList;
        public UEventController()
        {
            this.eventListenerList = new List<UEventListener>();
        }
        /// <summary>
        /// 侦听事件
        /// </summary>
        /// <param name="eventType">事件类别</param>
        /// <param name="callback">回调函数</param>
        public void AddEventListener(string eventType, EventListenerDelegate callback)
        {
            UEventListener eventListener = this.getListener(eventType);
            if (eventListener == null)
            {
                eventListener = new UEventListener(eventType);
                eventListenerList.Add(eventListener);
            }
            eventListener.OnEvent += callback;
        }
        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="eventType">事件类别</param>
        /// <param name="callback">回调函数</param>
        public void RemoveEventListener(string eventType, EventListenerDelegate callback)
        {
            UEventListener eventListener = this.getListener(eventType);
            if (eventListener != null)
            {
                eventListener.OnEvent -= callback;
            }
        }
        public void Cleanup()
        {
            var count = eventListenerList.Count;
            for (int i = 0; i < count; i++)
            {
                eventListenerList[i].Cleanup();
                eventListenerList[i] = null;
            }
            //foreach (var item in eventListenerList)
            //    item.Cleanup();
            this.eventListenerList.Clear();
        }
        /// <summary>
        /// 是否存在事件
        /// </summary>
        /// <returns>/returns>
        /// <param name="eventType">Event type.</param>
        public bool HasListener(string eventType)
        {
            return this.getListenerList(eventType).Count > 0;
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="gameObject"></param>
        public void DispatchEvent(UEvent evt, object gameObject)
        {
            var count = eventListenerList.Count;
            for (int i = 0; i < count; i++)
            {
                var eventListener = eventListenerList[i];
                if (eventListener.eventType.Equals(evt.eventType))
                {
                    eventListener.Excute(evt);
                }
            }
        }
        /// <summary>
        /// 获取事件列表
        /// </summary>
        /// <returns></returns>
        /// <param name="eventType"></param>
        private IList<UEventListener> getListenerList(string eventType)
        {
            IList<UEventListener> resultList = new List<UEventListener>();
            var count = this.eventListenerList.Count;
            for (int i = 0; i < count; i++)
            {
                var eventListener = eventListenerList[i];
                if (eventListener.eventType.Equals(eventType)) resultList.Add(eventListener);
            }
            //foreach (UEventListener eventListener in this.eventListenerList)
            //{
            //    if (eventListener.eventType == eventType) resultList.Add(eventListener);
            //}
            return resultList;
        }
        /// <summary>
        /// 获取事件
        /// </summary>
        /// <returns></returns>
        /// <param name="eventType"></param>
        private UEventListener getListener(string eventType)
        {
            var count = this.eventListenerList.Count;
            for (int i = 0; i < count; i++)
            {
                var eventListener = eventListenerList[i];
                if (eventListener.eventType.Equals(eventType)) return eventListener;
            }
            //foreach (UEventListener eventListener in this.eventListenerList)
            //{
            //    if (eventListener.eventType == eventType) return eventListener;
            //}
            return null;
        }
    }
    public delegate void EventListenerDelegate(UEvent evt);
    public class UEventListener
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public string eventType;
        public UEventListener(string eventType)
        {
            this.eventType = eventType;
        }
        public event EventListenerDelegate OnEvent;
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="evt"></param>
        public void Excute(UEvent evt)
        {
            if (OnEvent != null)
            {
                this.OnEvent(evt);
            }
        }
        public void Cleanup()
        {
            this.OnEvent = null;
        }
    }
    public class UEvent
    {
        /// <summary>
        /// 事件类别
        /// </summary>
        public string eventType;
        /// <summary>
        /// 参数
        /// </summary>
        public object[] eventParams;
        /// <summary>
        /// 事件抛出者
        /// </summary>
        public object target;
        public UEvent(string eventType, object eventParams = null)
        {
            this.eventType = eventType;
            this.target = eventParams;
        }
        public UEvent()
        {

        }
    }
    public class UEventBase
    {
        public int opcode;
    }
}

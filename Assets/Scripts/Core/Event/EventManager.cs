using System;
using System.Collections.Generic;
using Action = System.Action;

namespace XH
{
    class EventManager : CSharpSingleton<EventManager>
    {
        private Dictionary<EventType, Delegate> m_EventTable = new Dictionary<EventType, Delegate>();

        private void OnListenerAdding(EventType eventType, Delegate action)
        {
            if (!m_EventTable.ContainsKey(eventType))
                m_EventTable.Add(eventType, null);
            Delegate d = m_EventTable[eventType];
            if (d != null && d.GetType() != action.GetType())
                XHLogger.XH_EXCEPTIION(new Exception(string.Format("当前事件{0}所对应的委托是{1}，要添加的委托类型为{2}", 
                    eventType, d.GetType(), action.GetType())));
        }
        private void OnListenerRemoving(EventType eventType, Delegate action)
        {
            if(m_EventTable.TryGetValue(eventType, out Delegate d))
            {
                if (d == null)
                    XHLogger.XH_EXCEPTIION(new Exception(string.Format
                        ("事件{0}没有委托", eventType)));
                else if (d.GetType() != action.GetType())
                {
                    XHLogger.XH_EXCEPTIION(new Exception(string.Format("事件{0}当前委托类型为{1}，要移除的委托类型为{2}", 
                        eventType, d.GetType(), action.GetType())));
                }
                else return;
            }
            XHLogger.XH_EXCEPTIION(new Exception(string.Format("没有事件{0}", eventType)));
        }
        private void OnListenerRemoved(EventType eventType)
        {
            if (m_EventTable[eventType] == null)
                m_EventTable.Remove(eventType);
        }
        
        public void AddListener(EventType eventType, Action action)
        {
            OnListenerAdding(eventType, action);
            m_EventTable[eventType] = (Action)m_EventTable[eventType] + action;
        }
        public void AddListener<T>(EventType eventType, Action<T> action)
        {
            OnListenerAdding(eventType, action);
            m_EventTable[eventType] = (Action<T>)m_EventTable[eventType] + action;
        }
        public void AddListener<T, X>(EventType eventType, Action<T, X> action)
        {
            OnListenerAdding(eventType, action);
            m_EventTable[eventType] = (Action<T, X>)m_EventTable[eventType] + action;
        }
        public void AddListener<T, X, Y>(EventType eventType, Action<T, X, Y> action)
        {
            OnListenerAdding(eventType, action);
            m_EventTable[eventType] = (Action<T, X, Y>)m_EventTable[eventType] + action;
        }
        public void AddListener<T, X, Y, Z>(EventType eventType, Action<T, X, Y, Z> action)
        {
            OnListenerAdding(eventType, action);
            m_EventTable[eventType] = (Action<T, X, Y, Z>)m_EventTable[eventType] + action;
        }
        public void AddListener<T, X, Y, Z, W>(EventType eventType, Action<T, X, Y, Z, W> action)
        {
            OnListenerAdding(eventType, action);
            m_EventTable[eventType] = (Action<T, X, Y, Z, W>)m_EventTable[eventType] + action;
        }
 
        public void RemoveListener(EventType eventType, Action action)
        {
            OnListenerRemoving(eventType, action);
            m_EventTable[eventType] = (Action)m_EventTable[eventType] - action;
            OnListenerRemoved(eventType);
        }
        public void RemoveListener<T>(EventType eventType, Action<T> action)
        {
            OnListenerRemoving(eventType, action);
            m_EventTable[eventType] = (Action<T>)m_EventTable[eventType] - action;
            OnListenerRemoved(eventType);
        }
        public void RemoveListener<T, X>(EventType eventType, Action<T, X> action)
        {
            OnListenerRemoving(eventType, action);
            m_EventTable[eventType] = (Action<T, X>)m_EventTable[eventType] - action;
            OnListenerRemoved(eventType);
        }
        public void RemoveListener<T, X, Y>(EventType eventType, Action<T, X, Y> action)
        {
            OnListenerRemoving(eventType, action);
            m_EventTable[eventType] = (Action<T, X, Y>)m_EventTable[eventType] - action;
            OnListenerRemoved(eventType);
        }
        public void RemoveListener<T, X, Y, Z>(EventType eventType, Action<T, X, Y, Z> action)
        {
            OnListenerRemoving(eventType, action);
            m_EventTable[eventType] = (Action<T, X, Y, Z>)m_EventTable[eventType] - action;
            OnListenerRemoved(eventType);
        }
        public void RemoveListener<T, X, Y, Z, W>(EventType eventType, Action<T, X, Y, Z, W> action)
        {
            OnListenerRemoving(eventType, action);
            m_EventTable[eventType] = (Action<T, X, Y, Z, W>)m_EventTable[eventType] - action;
            OnListenerRemoved(eventType);
        }

        public void Broadcast(EventType eventType, bool isOnce=false)
        {
            if (m_EventTable.TryGetValue(eventType, out Delegate d))
            {
                if (d is Action action)
                {
                    action();
                    if (isOnce)
                        RemoveListener(eventType, action);
                    return;
                }
                XHLogger.XH_EXCEPTIION(new Exception(string.Format("事件{0}对应委托具有不同的类型", eventType)));
            }
            XHLogger.XH_EXCEPTIION(new Exception(string.Format("没有事件{0}", eventType)));
        }
        public void Broadcast<T>(EventType eventType, T arg, bool isOnce=false)
        {
            if (m_EventTable.TryGetValue(eventType, out Delegate d))
            {
                if (d is Action<T> action)
                {
                    action(arg);
                    if (isOnce)
                        RemoveListener(eventType, action);
                    return;
                }
                XHLogger.XH_EXCEPTIION(new Exception(string.Format("事件{0}对应委托具有不同的类型", eventType)));
            }
            XHLogger.XH_EXCEPTIION(new Exception(string.Format("没有事件{0}", eventType)));
        }
        public void Broadcast<T, X>(EventType eventType, T arg1, X arg2, bool isOnce=false)
        {

            if (m_EventTable.TryGetValue(eventType, out Delegate d))
            {
                if (d is Action<T, X> action)
                {
                    action(arg1, arg2);
                    if (isOnce)
                        RemoveListener(eventType, action);
                    return;
                }
                XHLogger.XH_EXCEPTIION(new Exception(string.Format("事件{0}对应委托具有不同的类型", eventType)));
            }
            XHLogger.XH_EXCEPTIION(new Exception(string.Format("没有事件{0}", eventType)));
        }
        public void Broadcast<T, X, Y>(EventType eventType, T arg1, X arg2, Y arg3, bool isOnce=false)
        {

            if (m_EventTable.TryGetValue(eventType, out Delegate d))
            {
                if (d is Action<T, X, Y> action)
                {
                    action(arg1, arg2, arg3);
                    if (isOnce)
                        RemoveListener(eventType, action);
                    return;
                }
                XHLogger.XH_EXCEPTIION(new Exception(string.Format("事件{0}对应委托具有不同的类型", eventType)));
            }
            XHLogger.XH_EXCEPTIION(new Exception(string.Format("没有事件{0}", eventType)));
        }
        public void Broadcast<T, X, Y, Z>(EventType eventType, T arg1, X arg2, Y arg3, Z arg4, bool isOnce=false)
        {
            if (m_EventTable.TryGetValue(eventType, out Delegate d))
            {
                if (d is Action<T, X, Y, Z> action)
                {
                    action(arg1, arg2, arg3, arg4);
                    if (isOnce)
                        RemoveListener(eventType, action);
                    return;
                }
                XHLogger.XH_EXCEPTIION(new Exception(string.Format("事件{0}对应委托具有不同的类型", eventType)));
            }
            XHLogger.XH_EXCEPTIION(new Exception(string.Format("没有事件{0}", eventType)));
        }
        public void Broadcast<T, X, Y, Z, W>(EventType eventType, T arg1, X arg2, Y arg3, Z arg4, W arg5, bool isOnce=false)
        {

            if (m_EventTable.TryGetValue(eventType, out Delegate d))
            {
                if (d is Action<T, X, Y, Z, W> action)
                {
                    action(arg1, arg2, arg3, arg4, arg5);
                    if (isOnce)
                        RemoveListener(eventType, action);
                    return;
                }
                XHLogger.XH_EXCEPTIION(new Exception(string.Format("事件{0}对应委托具有不同的类型", eventType)));
            }
            XHLogger.XH_EXCEPTIION(new Exception(string.Format("没有事件{0}", eventType)));
        }
    }
}
using System;
using System.Collections.Generic;
using Action = System.Action;

namespace XH
{
    class EventManager : CSharpSingleton<EventManager>
    {
        private Dictionary<short, Delegate> m_EventTable = new Dictionary<short, Delegate>();

        public int SERVER_TO_CLIENT_BEGIN = 1;
        public int SERVER_TO_CLIENT_END = 100;
        public int CLIENT_TO_SERVER_BEGIN = 111;
        public int CLIENT_TO_SERVER_END = 210;

        public void AddListener(short eventCode, Action action)
        {
            OnListenerAdding(eventCode, action);
            m_EventTable[eventCode] = (Action)m_EventTable[eventCode] + action;
        }
        public void AddListener<T>(short eventCode, Action<T> action)
        {
            OnListenerAdding(eventCode, action);
            m_EventTable[eventCode] = (Action<T>)m_EventTable[eventCode] + action;
        }
        public void AddListener<T, X>(short eventCode, Action<T, X> action)
        {
            OnListenerAdding(eventCode, action);
            m_EventTable[eventCode] = (Action<T, X>)m_EventTable[eventCode] + action;
        }
        public void AddListener<T, X, Y>(short eventCode, Action<T, X, Y> action)
        {
            OnListenerAdding(eventCode, action);
            m_EventTable[eventCode] = (Action<T, X, Y>)m_EventTable[eventCode] + action;
        }
        public void AddListener<T, X, Y, Z>(short eventCode, Action<T, X, Y, Z> action)
        {
            OnListenerAdding(eventCode, action);
            m_EventTable[eventCode] = (Action<T, X, Y, Z>)m_EventTable[eventCode] + action;
        }
        public void AddListener<T, X, Y, Z, W>(short eventCode, Action<T, X, Y, Z, W> action)
        {
            OnListenerAdding(eventCode, action);
            m_EventTable[eventCode] = (Action<T, X, Y, Z, W>)m_EventTable[eventCode] + action;
        }

        public void RemoveListener(short eventCode, Action action)
        {
            OnListenerRemoving(eventCode, action);
            m_EventTable[eventCode] = (Action)m_EventTable[eventCode] - action;
            OnListenerRemoved(eventCode);
        }
        public void RemoveListener<T>(short eventCode, Action<T> action)
        {
            OnListenerRemoving(eventCode, action);
            m_EventTable[eventCode] = (Action<T>)m_EventTable[eventCode] - action;
            OnListenerRemoved(eventCode);
        }
        public void RemoveListener<T, X>(short eventCode, Action<T, X> action)
        {
            OnListenerRemoving(eventCode, action);
            m_EventTable[eventCode] = (Action<T, X>)m_EventTable[eventCode] - action;
            OnListenerRemoved(eventCode);
        }
        public void RemoveListener<T, X, Y>(short eventCode, Action<T, X, Y> action)
        {
            OnListenerRemoving(eventCode, action);
            m_EventTable[eventCode] = (Action<T, X, Y>)m_EventTable[eventCode] - action;
            OnListenerRemoved(eventCode);
        }
        public void RemoveListener<T, X, Y, Z>(short eventCode, Action<T, X, Y, Z> action)
        {
            OnListenerRemoving(eventCode, action);
            m_EventTable[eventCode] = (Action<T, X, Y, Z>)m_EventTable[eventCode] - action;
            OnListenerRemoved(eventCode);
        }
        public void RemoveListener<T, X, Y, Z, W>(short eventCode, Action<T, X, Y, Z, W> action)
        {
            OnListenerRemoving(eventCode, action);
            m_EventTable[eventCode] = (Action<T, X, Y, Z, W>)m_EventTable[eventCode] - action;
            OnListenerRemoved(eventCode);
        }

        public void Broadcast(short eventCode, bool isOnce = false)
        {
            if (m_EventTable.TryGetValue(eventCode, out Delegate d))
            {
                if (d is Action action)
                {
                    action();
                    if (isOnce)
                        RemoveListener(eventCode, action);
                    return;
                }
                Logger.XH_EXCEPTIION(new Exception(string.Format("事件{0}对应委托具有不同的类型", eventCode)));
                return;
            }
            Logger.XH_EXCEPTIION(new Exception(string.Format("没有事件{0}", eventCode)));
        }
        public void Broadcast<T>(short eventCode, T arg, bool isOnce = false)
        {
            if (m_EventTable.TryGetValue(eventCode, out Delegate d))
            {
                if (d is Action<T> action)
                {
                    action(arg);
                    if (isOnce)
                        RemoveListener(eventCode, action);
                    return;
                }
                Logger.XH_EXCEPTIION(new Exception(string.Format("事件{0}对应委托具有不同的类型", eventCode)));
            }
            Logger.XH_EXCEPTIION(new Exception(string.Format("没有事件{0}", eventCode)));
        }
        public void Broadcast<T, X>(short eventCode, T arg1, X arg2, bool isOnce = false)
        {

            if (m_EventTable.TryGetValue(eventCode, out Delegate d))
            {
                if (d is Action<T, X> action)
                {
                    action(arg1, arg2);
                    if (isOnce)
                        RemoveListener(eventCode, action);
                    return;
                }
                Logger.XH_EXCEPTIION(new Exception(string.Format("事件{0}对应委托具有不同的类型", eventCode)));
                return;
            }
            Logger.XH_EXCEPTIION(new Exception(string.Format("没有事件{0}", eventCode)));
        }
        public void Broadcast<T, X, Y>(short eventCode, T arg1, X arg2, Y arg3, bool isOnce = false)
        {

            if (m_EventTable.TryGetValue(eventCode, out Delegate d))
            {
                if (d is Action<T, X, Y> action)
                {
                    action(arg1, arg2, arg3);
                    if (isOnce)
                        RemoveListener(eventCode, action);
                    return;
                }
                Logger.XH_EXCEPTIION(new Exception(string.Format("事件{0}对应委托具有不同的类型", eventCode)));
                return;
            }
            Logger.XH_EXCEPTIION(new Exception(string.Format("没有事件{0}", eventCode)));
        }
        public void Broadcast<T, X, Y, Z>(short eventCode, T arg1, X arg2, Y arg3, Z arg4, bool isOnce = false)
        {
            if (m_EventTable.TryGetValue(eventCode, out Delegate d))
            {
                if (d is Action<T, X, Y, Z> action)
                {
                    action(arg1, arg2, arg3, arg4);
                    if (isOnce)
                        RemoveListener(eventCode, action);
                    return;
                }
                Logger.XH_EXCEPTIION(new Exception(string.Format("事件{0}对应委托具有不同的类型", eventCode)));
                return;
            }
            Logger.XH_EXCEPTIION(new Exception(string.Format("没有事件{0}", eventCode)));
        }
        public void Broadcast<T, X, Y, Z, W>(short eventCode, T arg1, X arg2, Y arg3, Z arg4, W arg5, bool isOnce = false)
        {

            if (m_EventTable.TryGetValue(eventCode, out Delegate d))
            {
                if (d is Action<T, X, Y, Z, W> action)
                {
                    action(arg1, arg2, arg3, arg4, arg5);
                    if (isOnce)
                        RemoveListener(eventCode, action);
                    return;
                }
                Logger.XH_EXCEPTIION(new Exception(string.Format("事件{0}对应委托具有不同的类型", eventCode)));
                return;
            }
            Logger.XH_EXCEPTIION(new Exception(string.Format("没有事件{0}", eventCode)));
        }

        private void OnListenerAdding(short eventCode, Delegate action)
        {
            if (!m_EventTable.ContainsKey(eventCode))
                m_EventTable.Add(eventCode, null);
            Delegate d = m_EventTable[eventCode];
            if (d != null && d.GetType() != action.GetType())
                Logger.XH_EXCEPTIION(new Exception(string.Format("当前事件{0}所对应的委托是{1}，要添加的委托类型为{2}",
                    eventCode, d.GetType(), action.GetType())));
        }
        private void OnListenerRemoving(short eventCode, Delegate action)
        {
            if (m_EventTable.TryGetValue(eventCode, out Delegate d))
            {
                if (d == null)
                    Logger.XH_EXCEPTIION(new Exception(string.Format
                        ("事件{0}没有委托", eventCode)));
                else if (d.GetType() != action.GetType())
                {
                    Logger.XH_EXCEPTIION(new Exception(string.Format("事件{0}当前委托类型为{1}，要移除的委托类型为{2}",
                        eventCode, d.GetType(), action.GetType())));
                }
                else return;
            }
            Logger.XH_EXCEPTIION(new Exception(string.Format("没有事件{0}", eventCode)));
        }
        private void OnListenerRemoved(short eventCode)
        {
            if (m_EventTable[eventCode] == null)
                m_EventTable.Remove(eventCode);
        }
    }
}
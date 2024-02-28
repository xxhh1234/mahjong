using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace XH
{
    class Util
    {
        public static object CreateClass(Type type)
        { 
            Logger.XH_ASSERT(type != null && !type.IsAbstract && typeof(object).IsAssignableFrom(type), 
                string.Format("创建{0}类失败", type.Name));
            return Activator.CreateInstance(type);
        }

        public static ValueType CreateStruct(Type type)
        {
            Logger.XH_ASSERT(type != null && type.IsValueType,
                string.Format("创建{0}结构体失败", type.Name));
            return Activator.CreateInstance(type) as ValueType;
        }

        public static T Convert<T>(object o)
        {
            string str = JsonConvert.SerializeObject(o);
            T t = JsonConvert.DeserializeObject<T>(str);
            return t;
        }

        public static void AddEventTriggerListener(ref EventTrigger eventTrigger, EventTriggerType triggerType, UnityAction<BaseEventData> action)
        {
            EventTrigger.Entry entry = eventTrigger.triggers.Find((trigger) => trigger.eventID == triggerType);
            if (entry == null)
            {
                entry = new EventTrigger.Entry() { eventID = triggerType };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(action);
        }
        public static void RemoveEventTriggerListener(ref EventTrigger eventTrigger, EventTriggerType triggerType)
        {
            EventTrigger.Entry entry = eventTrigger.triggers.Find((trigger) => trigger.eventID == triggerType);
            if(entry == null)
                return;
            eventTrigger.triggers.Remove(entry);
        }

        public static Delegate CreateDelegate(MethodInfo method, object obj)
        {
            if (method == null || method.IsPublic || method.IsStatic || method.IsGenericMethod)
                return null;
            Type[] types = (from parameter in method.GetParameters() select parameter.ParameterType)
                .Concat(new[] { method.ReturnType }).ToArray();
            return method.CreateDelegate(Expression.GetDelegateType(types), obj);
        }
    }
}

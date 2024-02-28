using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.src.Core.Util
{
    class Util
    {
        public static object CreateClass(Type type)
        {

            // Logger.XH_ASSERT(type != null && !type.IsAbstract && typeof(object).IsAssignableFrom(type), 
            // string.Format("创建{0}类失败", type.Name));
            return Activator.CreateInstance(type);
        }

        public static ValueType CreateStruct(Type type)
        {
            // Logger.XH_ASSERT(type != null && type.IsValueType,
            // string.Format("创建{0}结构体失败", type.Name));
            return Activator.CreateInstance(type) as ValueType;
        }

        public static T Convert<T>(object o)
        {
            string str = JsonConvert.SerializeObject(o);
            T t = JsonConvert.DeserializeObject<T>(str);
            return t;
        }
    }
}

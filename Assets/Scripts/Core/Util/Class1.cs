using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XH
{
    class Class1
    {
        public static object CreateClass(string className)
        { 
            Type type = Type.GetType("XH." + className);
            XHLogger.XH_ASSERT(type != null && !type.IsAbstract && typeof(object).IsAssignableFrom(type), 
                string.Format("创建{0}类失败", className));
            return Activator.CreateInstance(type);
        }

        public static T Convert<T>(object o)
        {
            string str = JsonConvert.SerializeObject(o);
            T t = JsonConvert.DeserializeObject<T>(str);
            return t;
        }
    }
}

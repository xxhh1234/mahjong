using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XH
{
    class Controller
    {
        private Model model;
        private Dictionary<string, Delegate> operationDict = new Dictionary<string, Delegate>();
        private string ctrlName;

        public virtual void Init(string name) 
        {
            model = (Model)Class1.CreateClass(name + "Model");
            model.Init(name + "Model");
            ctrlName = name + "Controller";
            var methodInfos = GetType().GetRuntimeMethods();
            foreach (var methodInfo in methodInfos) 
            {
                Delegate d = CreateDelegate(methodInfo);
                if(d == null)
                    continue;
                AddMethod(methodInfo.Name, d);
            }
        }

        public virtual void UnInit() 
        {
            operationDict.Clear();
        }

        public Delegate GetMethod(string operationName)
        {
            XHLogger.XH_ASSERT(operationDict.ContainsKey(operationName), string.Format("{0}没有操作{1}", ctrlName, operationName));
            return operationDict[operationName];
        }

        private void AddMethod(string operationName, Delegate operation) 
        {
            XHLogger.XH_ASSERT(!operationDict.ContainsKey(operationName), 
                string.Format("{0}已经存在操作{1}", ctrlName, operationName));
            operationDict.Add(operationName, operation);
        }

        private Delegate CreateDelegate(MethodInfo method)
        {
            if (method == null || method.IsPublic || method.IsStatic || method.IsGenericMethod)
                return null;
            Type[] types = (from parameter in method.GetParameters() select parameter.ParameterType)
                .Concat(new[] { method.ReturnType }).ToArray(); 
            return method.CreateDelegate(Expression.GetDelegateType(types), this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;

namespace XH
{
    class Ctrl
    {
        private Model model;
        private Dictionary<string, Delegate> operationDict = new Dictionary<string, Delegate>();
        private string ctrlName;

        public virtual void Init(string name) 
        {
            string modelClassName = name + "Model";
            Type modelType = Type.GetType(modelClassName);
            if(modelType != null)
            { 
                model = (Model)Util.CreateClass(modelType);
                model.Init(name.Split(".")[1] + "Model");
            }
            ctrlName = name.Split(".")[1] + "Ctrl";
            var methodInfos = GetType().GetRuntimeMethods();
            foreach (var methodInfo in methodInfos) 
            {
                Delegate d = Util.CreateDelegate(methodInfo, this);
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
            Logger.XH_ASSERT(operationDict.ContainsKey(operationName), string.Format("{0}没有操作{1}", ctrlName, operationName));
            return operationDict[operationName];
        }

        private void AddMethod(string operationName, Delegate operation) 
        {
            Logger.XH_ASSERT(!operationDict.ContainsKey(operationName), 
                string.Format("{0}已经存在操作{1}", ctrlName, operationName));
            operationDict.Add(operationName, operation);
        }

       
    }
}

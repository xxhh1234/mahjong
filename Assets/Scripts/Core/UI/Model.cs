using System;
using System.Collections.Generic;
using UnityEngine;

namespace XH
{
    class Model
    {
        private string modelName;
        private Action<ValueType> uiAction;

        public virtual void Init(string name) 
        {
            modelName = name;
        }

        public virtual void UnInit() 
        {
        }

        public string GetModelName() {  return modelName; }
        public void InitEvent(short dataEventCode, short uiEventCode)
        {
            uiAction = (ValueType uiEventData) =>
            {
                EventManager.Instance.Broadcast(uiEventCode, uiEventData);
            };
            EventManager.Instance.AddListener(dataEventCode, uiAction);
        }
        public void UnInitEvent(short dataEventCode)
        {
            EventManager.Instance.RemoveListener(dataEventCode, uiAction);
        }
    }

}


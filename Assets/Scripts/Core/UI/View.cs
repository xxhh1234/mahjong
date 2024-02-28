using System;
using UnityEngine;

namespace XH
{
    class View
    {    
        public Ctrl controller;
        private string viewName;
        public GameObject gameObject;
        public Transform transform;
        public bool isClose;

        public virtual void Init(string name)
        {
            string prefix = name.Split("View")[0];
            string ctrlClassName = prefix + "Ctrl";
            Type ctrlType = Type.GetType(ctrlClassName);
            if(ctrlType != null)
            { 
                controller = (Ctrl)Util.CreateClass(ctrlType);
                controller.Init(prefix);
            }
            viewName = name.Split(".")[1];
            ResourceManager.Instance.Load(out GameObject prefab, viewName);
            gameObject = GameObject.Instantiate(prefab);
            gameObject.SetActive(false);
            gameObject.transform.SetParent(UIManager.Instance.UIView.transform, false);
            gameObject.name = viewName;
            transform = gameObject.transform;
            isClose = true;
        }
        
        public virtual void UnInit()
        {
            if(controller != null)
                controller.UnInit();
            GameObject.DestroyImmediate(gameObject);
        }

        public string GetViewName() { return viewName;}

        
    }
}

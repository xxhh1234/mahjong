using UnityEngine;

namespace XH
{
    class View
    {    
        public Controller controller;
        private string viewName;
        public GameObject gameObject;
        public Transform transform;

        public virtual void Init(string name)
        {
            string prefix = name.Split("View")[0];
            controller = (Controller)Class1.CreateClass(prefix + "Controller");
            controller.Init(prefix);
            viewName = name;
            gameObject = ObjectPool.Instance.GetGameObject(viewName);
            transform = gameObject.transform;
        }
        
        public virtual void UnInit()
        {
            controller.UnInit();
        }

        public string GetViewName() { return viewName;}

        
    }
}

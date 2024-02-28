using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


namespace XH
{
    class UIManager : CSharpSingleton<UIManager>
    {
        public GameObject UIView;
        private List<string> viewNames = new List<string>();
        private Dictionary<string, View> uiViews = new Dictionary<string, View>();
        

        public void Init(string spaceName)
        {
            UIView = new GameObject("UIManager");
            foreach (var name in viewNames)
            { 
                InitView(spaceName + name);
            }
        }
        public void UnInit()
        {
            foreach (var kvp in uiViews)
            {
                kvp.Value.UnInit();
            }
            GameObject.DestroyImmediate(UIView);
            uiViews.Clear();
        }
        public void InitView(string viewName)
        {
            Type viewType = Type.GetType(viewName);
            Logger.XH_ASSERT(viewType != null, string.Format("{0}初始化失败", viewName));
            View view = (View)Util.CreateClass(viewType);
            view.Init(viewName);
            if(view.transform.TryGetComponent(out Canvas canvas))
                canvas.worldCamera = GameManager.Instance.MainCamera;

            uiViews.Add(viewName.Split(".")[1], view);
        }

        public View GetView(string viewName)
        {
            Logger.XH_ASSERT(uiViews.ContainsKey(viewName), string.Format("没有名称为{0}的窗口", viewName));
            return uiViews[viewName];
        }
        public void ShowView(string viewName, Transform parent)
        {
            Logger.XH_ASSERT(uiViews.ContainsKey(viewName), string.Format("没有名称为{0}的窗口", viewName));
            Logger.XH_ASSERT(uiViews[viewName].isClose, string.Format("名称为{0}的窗口已打开", viewName));
            View view = uiViews[viewName];
            view.isClose = false;
            view.gameObject.SetActive(true);
            view.transform.SetParent(parent, false);
            // view.transform.localPosition = Vector3.zero;
        }
        public void ShowView(string viewName)
        {
            ShowView(viewName, GameManager.Instance.gameObject.transform);
        }
        public void CloseView(string viewName) 
        {
            Logger.XH_ASSERT(uiViews.ContainsKey(viewName), string.Format("没有名称为{0}的窗口", viewName));
            Logger.XH_ASSERT(!uiViews[viewName].isClose, string.Format("名称为{0}的窗口已关闭", viewName));
            View view = uiViews[viewName];
            view.transform.SetParent(UIView.transform);
            view.gameObject.SetActive(false);
            view.isClose = true;
        }
        public void AddViewName(string viewName)
        {
            viewNames.Add(viewName);
        }
    }
}

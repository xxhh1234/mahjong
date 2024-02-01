using System.Collections.Generic;


namespace XH
{
    class UIManager : CSharpSingleton<UIManager>
    {
        private List<string> viewNames = new List<string>()
        {
            "LoginView",
            "LobbyView",
            "CreateRoomView",
            "JoinRoomView",
            "RoomView",
        };

        private Dictionary<string, View> uiViews = new Dictionary<string, View>();

        public void InitUI()
        {
            foreach (var name in viewNames)
            {
                View view = new View();
                view.Init(name);
                uiViews.Add(name, view);
            }
        }
        
        public void SwitchView(string curViewName, string nextViewName)
        {
            View curView = uiViews[curViewName];
            View nextView = uiViews[nextViewName];
            curView.gameObject.SetActive(false);
            nextView.gameObject.SetActive(true);
        }

        ~UIManager() 
        {
            foreach (var kvp in uiViews)
            {
                kvp.Value.UnInit();
            }
            uiViews.Clear();
        }
    }
}

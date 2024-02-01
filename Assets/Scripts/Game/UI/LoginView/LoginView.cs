using System;
using System.Reflection;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XH
{
    class LoginView : View
    {
        private InputField inputId;
        private Button btnLogin;

        public override void Init(string name)
        {
            inputId = transform.Find("inputId").GetComponent<InputField>();
            btnLogin = transform.Find("btnLogin").GetComponent<Button>();
            btnLogin.onClick.AddListener(() => 
            {  
                Action<string> action = controller.GetMethod("OnButtonLogin") as Action<string>; 
                action(inputId.text);
            });

            EventManager.Instance.AddListener(EventType.UI_RefreshLoginView, (LoginData loginData) => 
            { RefreshLoginView(loginData);});
        }

        public override void UnInit()
        {
            base.UnInit();

            EventManager.Instance.RemoveListener(EventType.UI_RefreshLoginView, (LoginData loginData) =>
            { RefreshLoginView(loginData); });
        }

        public void RefreshLoginView(LoginData loginData)
        {
            inputId.text = loginData.id;
        }
    }
}

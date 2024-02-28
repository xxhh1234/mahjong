using System;
using UnityEngine.UI;
using XH;

namespace mahjong
{
    class LoginView : View
    {
        private InputField inputId;
        private Button btnLogin;

        public override void Init(string name)
        {
            base.Init(name);

            inputId = transform.Find("inputId").GetComponent<InputField>();
            btnLogin = transform.Find("btnLogin").GetComponent<Button>();
            btnLogin.onClick.AddListener(() => 
            {  
                Action<string> action = controller.GetMethod("OnButtonLogin") as Action<string>; 
                action(inputId.text);
            });

            EventManager.Instance.AddListener(UIEvent.RefreshLoginView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    RefreshLoginView(value);
                });
            });
        }

        public override void UnInit()
        {
            base.UnInit();

            EventManager.Instance.RemoveListener(UIEvent.RefreshLoginView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    RefreshLoginView(value);
                });
            });
        }

        public void RefreshLoginView(ValueType value)
        {
            LoginData loginData = (LoginData)value;
            inputId.text = loginData.id;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XH
{
    struct LoginData
    {
        public string id;

        public LoginData(string Id)
        {
            id = Id;
        }
    }

    class LoginModel : Model
    {
        public override void Init(string name)
        {
            base.Init(name);
            DataManager.Instance.AddData("LoginData", new LoginData());
            EventManager.Instance.AddListener<LoginData>(EventType.UI_ChangeLoginData, SetLoginData);
        }

        public override void UnInit()
        {
            base.UnInit();
            EventManager.Instance.RemoveListener<LoginData>(EventType.UI_ChangeLoginData, SetLoginData);
        }

        private void SetLoginData(LoginData loginData)
        {
            EventManager.Instance.Broadcast(EventType.UI_RefreshLoginView, loginData);
        }
    }
}

using XH;

namespace mahjong
{
    struct LoginData
    {
        public string id;
    }

    class LoginModel : Model
    {
        public override void Init(string name)
        {
            base.Init(name);

            ClientDataManager.Instance.AddData("LoginData", new LoginData());
            InitEvent(DataEvent.ChangeLoginData, UIEvent.RefreshLoginView);
        }

        public override void UnInit()
        {
            base.UnInit();
            UnInitEvent(DataEvent.ChangeLoginData);
        }
    }
}

using XH;

namespace mahjong
{
    struct UserData
    {
        public string id;
        public string username;
        public uint coin;
        public uint diamond;
        public ushort sex;
    }

    class LobbyModel : Model
    {
        public override void Init(string name)
        {
            base.Init(name);
            
            ClientDataManager.Instance.AddData("UserData", new UserData());
            InitEvent(DataEvent.ChangeUserData, UIEvent.RefreshLobbyView);
        }

        public override void UnInit()
        {
            base.UnInit();         
            UnInitEvent(DataEvent.ChangeUserData);
        }
    }
}

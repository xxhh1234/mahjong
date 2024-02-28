using XH;

namespace mahjong
{
    struct JoinRoomData
    {
        public string roomName;
    }
    class JoinRoomModel : Model
    {
        public override void Init(string name)
        {
            base.Init(name);

            ClientDataManager.Instance.AddData("JoinRoomData", new JoinRoomData());
            InitEvent(DataEvent.ChangeJoinRoomData, UIEvent.RefreshJoinRoomView);
        }

        public override void UnInit()
        {
            base.UnInit();

            UnInitEvent(DataEvent.ChangeJoinRoomData);
        }
    }
}

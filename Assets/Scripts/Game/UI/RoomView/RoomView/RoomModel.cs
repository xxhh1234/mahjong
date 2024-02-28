using XH;

namespace mahjong
{
    struct RoomData
    {
        public uint remainTile;
        public uint roomName;
        public ushort playerCnt;
        public ushort masterId;
        public RoomState roomState;

    }

    class RoomModel : Model
    {

        public override void Init(string name)
        {
            base.Init(name);
            
            ClientDataManager.Instance.AddData("RoomData", new RoomData());
            InitEvent(DataEvent.ChangeRoomData, UIEvent.RefreshRoomView);
      
        }

        public override void UnInit()
        {
            base.UnInit();
            UnInitEvent(DataEvent.ChangeRoomData);
        }
    }
}

using XH;

namespace mahjong
{
    struct HandTileData
    {
        public ushort handTileId;
        public ushort[] handTile;
        public ushort tileCnt;
    }
    struct PlayTileData
    {
        public bool isADrawTile;
        public bool isCanPlayTile;
        public bool isGangHouPlayTile;
        public ushort playTileNum;
        public ushort playTileIndex;
    }
    struct DrawTileData
    {
        public ushort drawTileId;
        public ushort drawTileNum;

        public DrawTileData(ushort id=0, ushort num=0)
        {
            drawTileId = id;
            drawTileNum = num;
        }
    }
    struct ThrowTileData
    {
        public ushort throwTileId;
        public ushort throwTileNum;

        public ThrowTileData(ushort id = 0, ushort num = 0)
        {
            throwTileId = id;
            throwTileNum = num;
        }
    }
    struct PlayTileReturnData
    {
        public ushort playTileNum;
        public ushort playTileId;
        public HuTileType huTileType;
        public PengGangType pengGangType;
    };
    struct DrawTileReturnData
    {
        public uint remainTile;
        public ushort drawTileNum;
        public ushort drawTileId;
        public bool isGangHouDrawTile;
        public ZiMoType ziMoType;
        public ShangHuaType shangHuaType;
        public PengGangType anGangJiaGangType;
    }
    struct TipData
    {
        public bool isTile;
        public ushort tipNum;
        public ushort tipId;
        public bool isOnce;
    }

    class TileModel : Model
    {
        public override void Init(string name)
        {
            base.Init(name);
            ClientDataManager.Instance.AddData("HandTileData",  new HandTileData());
            ClientDataManager.Instance.AddData("PlayTileData", new PlayTileData());
            ClientDataManager.Instance.AddData("DrawTileData", new DrawTileData());
            ClientDataManager.Instance.AddData("ThrowTileData", new ThrowTileData());
            ClientDataManager.Instance.AddData("TipData", new TipData());

            InitEvent(DataEvent.ChangeHandTileData, UIEvent.RefreshHandTileView);
            InitEvent(DataEvent.ChangeDrawTileData, UIEvent.RefreshDrawTileView);
            InitEvent(DataEvent.ChangeThrowTileData, UIEvent.RefreshThrowTileView);
            InitEvent(DataEvent.ChangeTipData, UIEvent.RefreshTipView);
        }

        public override void UnInit()
        {
            base.UnInit();

            UnInitEvent(DataEvent.ChangeHandTileData);
            UnInitEvent(DataEvent.ChangeDrawTileData);
            UnInitEvent(DataEvent.ChangeThrowTileData);
            UnInitEvent(DataEvent.ChangeTipData);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XH;

namespace mahjong
{
    struct ButtonData
    {
        public ushort buttonIndex; 
        public short param;
        public object obj;
        public short opCode;

        public ButtonData(ushort index=0, short p=0, object o=null, short code=ClientToServer.PlayTileDataReturn)
        {
            buttonIndex=index;
            param=p;
            obj=o;
            opCode=code;
        }
    }
    struct HuTileReturnData
    {
        public ushort huTileId;
        public ushort[] huTile;
        public ushort huTileCnt;
        public ushort score;
    }
    struct PengGangReturnData
    {
        public ushort pengGangId;
        public ushort pengGangTileNum;
        public PengGangType pengGangType;
        public ushort score;
    }
    struct QiangGangData
    {
        public HuTileType huTileType;
    }

    class ButtonModel : Model
    {
        public override void Init(string name)
        {
            base.Init(name);

            ClientDataManager.Instance.AddData("ButtonData", new ButtonData());
            ClientDataManager.Instance.AddData("HuTileReturnData", new HuTileReturnData());
            ClientDataManager.Instance.AddData("PengGangReturnData", new PengGangReturnData());

            InitEvent(DataEvent.ChangeButtonData, UIEvent.RefreshButtonView);
            InitEvent(DataEvent.ChangeHuTileReturnData, UIEvent.RefreshHuTileView);
            InitEvent(DataEvent.ChangePengGangReturnData, UIEvent.RefreshPengGangView);
        }

        public override void UnInit()
        {
            base.UnInit();

            UnInitEvent(DataEvent.ChangeButtonData);
            UnInitEvent(DataEvent.ChangeHuTileReturnData);
            UnInitEvent(DataEvent.ChangePengGangReturnData);

        }
    }
}

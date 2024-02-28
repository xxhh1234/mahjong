using System.Collections.Generic;
// LoginView
struct LoginData
{
    public string id;
}
// LobbyView
struct UserData
{
    public string id;
    public string username;
    public uint coin;
    public uint diamond;
    public ushort sex;
}
// RoomView
struct RoomData
{
    public uint remainTile;
    public uint roomName;
    public ushort playerCnt;
    public ushort masterId;
    public RoomState roomState;

}
// JoinRoomView
struct JoinRoomData
{
    public string roomName;
}
// PlayerView
struct SinglePlayerData
{
    public PlayerPosition playerPos;
    public ushort sex;
    public bool isMaster;
    public TileType queTileType;
    public string playerName;
    public uint score;
    public bool isReady;
    public ushort playerId;
}
struct PlayerData
{
    public SinglePlayerData[] playerDatas;
    public ushort playerCnt;


    public PlayerData(uint size)
    {
        playerDatas = new SinglePlayerData[size];
        playerCnt = 0;
    }
}
struct DingQueData
{
    public ushort dingQueId;
    public TileType queTileType;
}
struct TimerData
{
    public ushort timerId;
    public bool isPlay;
}
// ButtonView
struct ButtonData
{
    public ushort buttonIndex;
    public short param;
    public object obj;
    public short opCode;

    public ButtonData(ushort index = 0, short p = 0, object o = null, short code = ClientToServer.PlayTileDataReturn)
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
    public ushort huTileType;
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
// SelectTileView
struct SelectTileData
{
    public bool isOnSelectTile;
    public ushort selectTileId;
    public List<ushort> selectTiles;

    public SelectTileData(ushort id)
    {
        isOnSelectTile = false;
        selectTileId = id;
        selectTiles = new List<ushort>();
    }
}
// TileView
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
}
struct DrawTileData
{
    public ushort drawTileId;
    public ushort drawTileNum;

    public DrawTileData(ushort id = 0, ushort num = 0)
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
}


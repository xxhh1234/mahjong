public enum OpCode
{
    Test,
    Login,
    CreateRoomPanel,
    JoinRoomPanel,
    LeaveRoomPanel,
    ClickReady,
    SelectTile,
    DingQue,
    PlayTile,
    PlayTileDataReturn,
    DrawTile,
    DrawTileDataReturn,
    QiangGangDataReturn,
    GoOn,
}

public enum EventCode
{
    Test,
    SendPlayerToOther,
    SendLeaveRoomIdToOther,
    SendReadyPlayerIdToOther,
    SendTileToAll,
    SendSelectTileToAll,
    SendQueTileTypeToOther,
    SendPlayTileDataToOther,

    SendOtherPlayTileDataToAll,
    SendPengGangTileDataToAll,
    SendChiTileDataToAll,

    SendToNextDrawTile,

    SendDrawTileDataToOther,
    SendZiMoDataToAll,
    SendShangHuaDataToAll,
    SendAnGangJiaGangDataToAll,
    /// <summary>
    /// 在加杠这一步需要检测是否被抢杠
    /// </summary>
    SendQiangGangDataToOther,
    /// <summary>
    /// 抢杠胡的数据发给所有人
    /// </summary>
    SendQiangGangHuDataToAll,
    SendGameOverToAll,
}

public class ParameterCode
{
    /// <summary>
    /// 用户Id，登录
    /// </summary>
    public const short id = 1;
    public const short userInfo = 2;
    /// <summary>
    /// 房间名称，加入房间
    /// </summary>
    public const short roomName = 3;
    public const short roomInfo = 4;
    /// <summary>
    /// 玩家
    /// </summary>
    public const short player = 5;
    /// <summary>
    /// 玩家列表
    /// </summary>
    public const short playerList = 6;
    /// <summary>
    /// 角色Id，四个玩家都有唯一的actorId
    /// </summary>
    public const short actorId = 7;
    /// <summary>
    /// 13张手牌
    /// </summary>
    public const short handTile = 8;
    public const short selectTileList = 9;
    public const short queTileType = 10;
    /// <summary>
    /// 摸牌数字
    /// </summary>
    public const short drawTileNum = 10;
    public const short lackTileType = 11;
    /// <summary>
    /// 房间剩余牌数
    /// </summary>
    public const short remainTile = 12;
    /// <summary>
    /// 摸牌玩家Id
    /// </summary>
    public const short drawTileId = 13;
    /// <summary>
    /// 出牌数字
    /// </summary>
    public const short playTileNum = 14;
    /// <summary>
    /// 是否可以摸牌
    /// </summary>
    public const short isDrawTile = 15;
    /// <summary>
    /// 胡碰杠类型
    /// </summary>
    public const short otherPlayTileType = 16;
    public const short PengGangType = 17;
    /// <summary>
    /// 出牌玩家Id
    /// </summary>
    public const short playTileId = 18;
    /// <summary>
    /// 胡牌数据列表
    /// </summary>
    public const short otherPlayTileDataList = 19;
    /// <summary>
    /// 四个玩家的手牌
    /// </summary>
    public const short handTileList = 20;
    /// <summary>
    /// 分数列表
    /// </summary>
    public const short scoreList = 22;
    /// <summary>
    /// 碰杠玩家Id
    /// </summary>
    public const short pengGangId = 24;
    /// <summary>
    /// 是否杠后摸牌
    /// </summary>
    public const short isGangHouDrawTile = 26;
    /// <summary>
    /// 错误信息
    /// </summary>
    public const short error = 27;
    /// <summary>
    /// 是否杠后出牌
    /// </summary>
    public const short isGangHouPlayTile = 28;
    /// <summary>
    /// 空
    /// </summary>
    public const short none = 29;
    /// <summary>
    /// 自摸类型，自摸玩家Id，自摸数字
    /// </summary>
    public const short ziMoType = 30;
    public const short ziMoId = 31;
    public const short ziMoNum = 33;
    /// <summary>
    /// 杠上开花类型，杠上开花玩家Id，杠上开花数字
    /// </summary>
    public const short shangHuaType = 34;
    public const short shangHuaId = 35;
    public const short shangHuaNum = 36;
    /// <summary>
    /// 暗杠加杠类型，暗杠加杠玩家Id，暗杠加杠数字
    /// </summary>
    public const short anGangJiaGangType = 37;
    public const short anGangJiaGangId = 38;
    public const short anGangJiaGangNum = 39;

    public const short qiangGangHuDataList = 40;

    public const short noHuPlayerIdList = 41;
}
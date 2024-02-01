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
    // ���������Ϸ
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
    /// �ڼӸ���һ����Ҫ����Ƿ�����
    /// </summary>
    SendQiangGangDataToOther,
    /// <summary>
    /// ���ܺ������ݷ���������
    /// </summary>
    SendQiangGangHuDataToAll,
    SendGameOverToAll,
}

public class ParameterCode
{
    public const short id = 1;
    public const short userInfo = 2;
    /// <summary>
    /// �������ƣ����뷿��
    /// </summary>
    public const short roomName = 3;
    public const short roomInfo = 4;
    /// <summary>
    /// ���
    /// </summary>
    public const short player = 5;
    /// <summary>
    /// ����б�
    /// </summary>
    public const short playerList = 6;
    /// <summary>
    /// ��ɫId���ĸ���Ҷ���Ψһ��actorId
    /// </summary>
    public const short actorId = 7;
    /// <summary>
    /// 13������
    /// </summary>
    public const short handTile = 8;
    public const short selectTileList = 9;
    public const short queTileType = 10;
    /// <summary>
    /// ��������
    /// </summary>
    public const short drawTileNum = 10;
    public const short lackTileType = 11;
    /// <summary>
    /// ����ʣ������
    /// </summary>
    public const short remainTile = 12;
    /// <summary>
    /// �������Id
    /// </summary>
    public const short drawTileId = 13;
    /// <summary>
    /// ��������
    /// </summary>
    public const short playTileNum = 14;
    /// <summary>
    /// �Ƿ��������
    /// </summary>
    public const short isDrawTile = 15;
    /// <summary>
    /// ����������
    /// </summary>
    public const short otherPlayTileType = 16;
    public const short PengGangType = 17;
    /// <summary>
    /// �������Id
    /// </summary>
    public const short playTileId = 18;
    /// <summary>
    /// ���������б�
    /// </summary>
    public const short otherPlayTileDataList = 19;
    /// <summary>
    /// �ĸ���ҵ�����
    /// </summary>
    public const short handTileList = 20;
    /// <summary>
    /// �����б�
    /// </summary>
    public const short scoreList = 22;
    /// <summary>
    /// �������Id
    /// </summary>
    public const short pengGangId = 24;
    /// <summary>
    /// �Ƿ�ܺ�����
    /// </summary>
    public const short isGangHouDrawTile = 26;
    /// <summary>
    /// ������Ϣ
    /// </summary>
    public const short error = 27;
    /// <summary>
    /// �Ƿ�ܺ����
    /// </summary>
    public const short isGangHouPlayTile = 28;
    /// <summary>
    /// ��
    /// </summary>
    public const short none = 29;
    /// <summary>
    /// �������ͣ��������Id����������
    /// </summary>
    public const short ziMoType = 30;
    public const short ziMoId = 31;
    public const short ziMoNum = 33;
    /// <summary>
    /// ���Ͽ������ͣ����Ͽ������Id�����Ͽ�������
    /// </summary>
    public const short shangHuaType = 34;
    public const short shangHuaId = 35;
    public const short shangHuaNum = 36;
    /// <summary>
    /// ���ܼӸ����ͣ����ܼӸ����Id�����ܼӸ�����
    /// </summary>
    public const short anGangJiaGangType = 37;
    public const short anGangJiaGangId = 38;
    public const short anGangJiaGangNum = 39;

    public const short qiangGangHuDataList = 40;

    public const short noHuPlayerIdList = 41;
}
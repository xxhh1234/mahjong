namespace XH
{
    // 1~100
    public struct ServerToClient
    {
        public const short _ServerToClient = 1;
        // RoomView
        public const short SendPlayerToOther = 2;
        public const short SendLeaveRoomIdToOther = 3;
        public const short SendReadyPlayerIdToOther = 4;
        // TileView
        public const short SendTileToAll = 5;
        // SelectTileView
        public const short SendSelectTileToAll = 6;
        // DingQueView
        public const short SendQueTileTypeToOther = 7;
        // TileView
        public const short SendPlayTileDataToOther = 8;
        public const short SendHuTileDataToAll = 9; 
        public const short SendPengGangTileDataToAll = 10;
        public const short SendChiTileDataToAll = 11;
        public const short SendToNextDrawTile = 12;
        public const short SendDrawTileDataToOther = 13;
        public const short SendZiMoDataToAll = 14;
        public const short SendShangHuaDataToAll = 15;
        public const short SendAnGangJiaGangDataToAll = 16;
        public const short SendQiangGangDataToOther = 17;
        public const short SendQiangGangHuDataToAll = 18;
        public const short SendGameOverToAll = 19;
        public const short SendTimerToAll = 20;
    }
    // 111~210
    public struct ClientToServer
    {
        public const short _ClientToServer = 111;
        public const short Login = 112;
        public const short CreateRoom = 113;
        public const short JoinRoom = 114;
        public const short LeaveRoom = 115;
        public const short ClickReady = 116;
        public const short SelectTile = 117;
        public const short DingQue = 118;
        public const short PlayTile = 119;
        public const short PlayTileDataReturn = 120;
        public const short DrawTile = 121;
        public const short DrawTileDataReturn = 122;
        public const short QiangGangDataReturn = 123;
        public const short ContinueGame = 124;
    }
    public struct ParameterCode
    {
        // LoginView
        public const short LoginData = 1;
        // LobbyView
        public const short UserData = 2;
        // CreateRoomView
        public const short CreateRoomData = 3;
        // JoinRoomView
        public const short JoinRoomData = 4;
        // RoomView
        public const short RoomData = 5;
        // SelectTileView
        public const short SelectTileData = 6;
        // PlayerView
        public const short SinglePlayerData = 8;
        public const short PlayerData = 9;
        public const short DingQueData = 10;
        public const short TimerData = 11;
        // TileView
        public const short HandTileData = 12;
        public const short PlayTileData = 13;
        public const short PlayTileReturnData = 14;
        public const short HuTileReturnData = 15;
        public const short PengGangReturnData = 16;
        public const short QiangGangData = 17;
        public const short GameOverData = 18;
        public const short DrawTileData = 19;
        public const short DrawTileReturnData = 20;
        // Common
        public const short playerId = 21;
        public const short error = 22;
        public const short none = 23;
        public const short TileType = 24;
        public const short HuTileType = 25;
        public const short PengGangType = 26;
        public const short ZiMoType = 27;
        public const short ShangHuaType = 28;
    }
}

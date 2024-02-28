using MyServer.src.Core.Data;
using MyServer.src.Core.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TGServer;

namespace MyServer.src.Game
{
    class Peer : PeerBase
    {
        public SinglePlayerData playerData1 = new SinglePlayerData();
        public UserData userData = new UserData();
        public RoomData roomData = new RoomData();
        public Room room = null;
        public Chair chair = null;
        public PlayerPosition[] playerPositions = { PlayerPosition.east, PlayerPosition.south, PlayerPosition.west, PlayerPosition.north };

        public override void OnDisConnected(Exception e)
        {
            Console.WriteLine("OnDisConnected:" + e.ToString());
        }
        public override void OnException(Exception exception)
        {
            Console.WriteLine("OnException:" + exception.ToString());
        }

        private void SendEventToOther(short eventcode, Dictionary<short, object> dict)
        {
            if (room != null && chair != null)
            {
                for (int i = 0; i < room.chairList.Count; ++i)
                {
                    if (chair.playerPos != room.chairList[i].playerPos)
                    {
                        if (room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                        {
                            room.chairList[i].peer.SendEvent(eventcode, dict);
                        }
                    }
                }
            }
        }
        private void SendEventToAll(short eventcode, Dictionary<short, object> dict)
        {
            if (room != null && chair != null)
            {
                for (int i = 0; i < room.chairList.Count; ++i)
                {
                    if (room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                    {
                        room.chairList[i].peer.SendEvent(eventcode, dict);
                    }
                }
            }
        }

        // ClientToServer
        public override void OnOperationRequest(short opCode, Dictionary<short, object> dict)
        {
            switch(opCode)
            {
                case ClientToServer.Login:
                {
                    // 1. 接收客户端发送的Id并向json文件查询Id对应的用户信息
                    string id = Util.Convert<string>(dict[ParameterCode.LoginData]);
                    string username = LocalDataManager.QueryData<UserData, string>(id, "username");
                    uint coin = LocalDataManager.QueryData<UserData, uint>(id, "coin");
                    uint diamond = LocalDataManager.QueryData<UserData, uint>(id, "diamond");
                    ushort sex = LocalDataManager.QueryData<UserData, ushort>(id, "sex");
                    Console.WriteLine("username: " + username + "coin: " + coin + "diamond: " + diamond + "sex: " + sex);
                    // 2.设置userData 1. id 2. 用户名 3. 金币 4. 钻石 5.性别
                    userData.id = id.ToString();
                    userData.username = username;
                    userData.coin = coin;
                    userData.diamond = diamond;
                    userData.sex = sex;
                    playerData1.playerName = userData.username;
                    playerData1.sex = userData.sex;

                    // 3.服务器 => 客户端：
                    Dictionary<short, object> dictUserInfo = new Dictionary<short, object>();
                    dictUserInfo.Add(ParameterCode.UserData, userData);
                    SendResponse(opCode, ReturnCode.success, dictUserInfo);
                    
                    break;
                }
                case ClientToServer.CreateRoom:
                {
                    // 1.设置Room
                    room = new Room();
                    room.roomInfo.remainTile = 108;
                    Random ran = new Random();
                    room.roomInfo.roomName = ran.Next(10001, 100000);
                    ++room.roomInfo.AllActorCounts;
                    room.roomInfo.masterClientID = room.roomInfo.AllActorCounts;
                    chair = room.chairList[0];
                    chair.peer = this;
                    chair.isJoin = true;
                    chair.playerPos = PlayerPosition.east;
                    chair.score = 0;
                    // 2. 设置RoomData
                    roomData = new RoomData();
                    roomData.remainTile = room.roomInfo.remainTile;
                    roomData.roomName = (uint)room.roomInfo.roomName;
                    roomData.playerCnt = (ushort)room.roomInfo.AllActorCounts;
                    roomData.masterId = roomData.playerCnt;
                    // 2.设置SinglePlayerData并将房间添加到房间字典
                    playerData1.playerPos = chair.playerPos;
                    playerData1.sex = userData.sex;
                    playerData1.isMaster = true;
                    playerData1.queTileType = chair.queTileType;
                    playerData1.score = (ushort)chair.score;
                    playerData1.isReady = false;
                    playerData1.playerId = roomData.playerCnt;
                    AllGames.dictRooms.Add(room.roomInfo.roomName, room);
                    // 3.服务器回复房间信息给客户端 1. 房间信息 2. 本家
                    Dictionary<short, object> dictResponse = new Dictionary<short, object>();
                    dictResponse.Add(ParameterCode.RoomData, roomData);
                    dictResponse.Add(ParameterCode.SinglePlayerData, playerData1);
                    SendResponse(opCode, ReturnCode.success, dictResponse);
                    
                    break;
                }
                case ClientToServer.JoinRoom:
                {
                    JoinRoomData joinRoomData = Util.Convert<JoinRoomData>(dict[ParameterCode.JoinRoomData]);
                    int roomName = int.Parse(joinRoomData.roomName);
                    if(AllGames.dictRooms.ContainsKey(roomName))
                    {
                        // 1. 从房间字典中获取房间信息，获取所有玩家的信息 PlayerData
                        room = AllGames.dictRooms[roomName];
                        PlayerData playerData = new PlayerData(4);
                        for (int i = 0; i < room.chairList.Count; ++i)
                        {
                            if (room.chairList[i].isJoin == false && chair == null)
                            {
                                room.chairList[i].peer = this;
                                room.chairList[i].isJoin = true;
                                playerData1.playerPos = room.chairList[i].playerPos;
                                ++room.roomInfo.AllActorCounts;
                                playerData1.playerId = (ushort)room.roomInfo.AllActorCounts;
                                playerData1.score = 0;
                                chair = room.chairList[i]; 
                            }
                            if (room.chairList[i].isJoin == true)
                                playerData.playerDatas[playerData.playerCnt++] = room.chairList[i].peer.playerData1;
                        }
                        // 2. 设置RoomData
                        roomData.remainTile = room.roomInfo.remainTile;
                        roomData.roomName = (uint)room.roomInfo.roomName;
                        roomData.playerCnt = (ushort)room.roomInfo.AllActorCounts;
                        roomData.masterId = (ushort)room.roomInfo.masterClientID;
                        // 3. 服务器发送回复给未加入玩家的客户端： 1. 房间信息 2. 房间的玩家
                        Dictionary<short, object> dictJoinRoom = new Dictionary<short, object>();
                        dictJoinRoom.Add(ParameterCode.RoomData, roomData);
                        dictJoinRoom.Add(ParameterCode.PlayerData,  playerData);
                        SendResponse(opCode, ReturnCode.success, dictJoinRoom);
                        // 4. 服务器通知其他已加入玩家的客户端： 1. 加入房间的玩家
                        Dictionary<short, object> dictSendPlayer = new Dictionary<short, object>();
                        dictSendPlayer.Add(ParameterCode.SinglePlayerData, playerData1);
                        SendEventToOther(ServerToClient.SendPlayerToOther, dictSendPlayer);
                    }

                    break;
                }
                case ClientToServer.LeaveRoom:
                {
                    // 1. 设置椅子和玩家的信息
                    chair.isJoin = false;
                    playerData1.isReady = false;
                    // 2. 如果庄家离开房间， 就要改变masterClinentID
                    if(playerData1.playerId == room.roomInfo.masterClientID)
                    {
                        room.roomInfo.masterClientID = playerData1.playerId == 4 ? 1 : (playerData1.playerId + 1);
                    }

                    // 3. 服务器发送回复给客户端
                    SendResponse(opCode, ReturnCode.success, null);
                    // 4. 服务器通知其他未离开玩家， 有玩家离开房间
                    Dictionary<short, object> dictLeaveRoom = new Dictionary<short, object>();
                    dictLeaveRoom.Add(ParameterCode.playerId, playerData1.playerId);
                    SendEventToOther(ServerToClient.SendLeaveRoomIdToOther, dictLeaveRoom);

                    break;
                }
                case ClientToServer.ClickReady:
                {
                    // 1.设置玩家的信息并回复客户端
                    playerData1.isReady = true;
                    // 2.服务器通知其他玩家，有玩家点击准备
                    Dictionary<short, object> dictReady = new Dictionary<short, object>();
                    dictReady.Add(ParameterCode.playerId, playerData1.playerId);
                    SendResponse(ClientToServer.ClickReady, ReturnCode.success, dictReady);
                    SendEventToOther(ServerToClient.SendReadyPlayerIdToOther, dictReady);
                    bool flag = true;
                    // 3.如果房间人满，且每个人都已准备就发牌
                    for(int i = 0; i < room.chairList.Count; ++i)
                    {
                        if(room.chairList[i].peer == null || room.chairList[i].peer.playerData1.isReady == false)
                        {
                            flag = false;
                        }
                    }
                    if(flag)
                    {
                        // 4.初始化108张牌并且给每个人发送13张手牌
                        room.InitTiles();
                        List<List<ushort>>handTiles = new List<List<ushort>>(4)
                        { 
                            new List<ushort>(13),
                            new List<ushort>(13),
                            new List<ushort>(13),
                            new List<ushort>(13)
                        };
                        for(int i = 0; i < handTiles.Count; ++i)
                        {
                            for(int j = 0; j < 13; ++j)
                                handTiles[i].Add((ushort)room.Tiles.Dequeue());
                        }
                        // 5.设置房间信息：玩家手牌 
                        for(int i = 0; i < room.chairList.Count; ++i)
                        {
                            room.chairList[i].handTile = handTiles[i];
                        }
                        // 6.服务器给每个玩家发送： 1. RoomData 2. HandTileData
                        roomData.remainTile = (uint)room.Tiles.Count;
                        roomData.roomState = RoomState.play;
                        HandTileData handTileData = new HandTileData();
                        for (int i = 0; i < room.chairList.Count; ++i)
                        {
                            Dictionary<short, object> dictSendTile = new Dictionary<short, object>();
                            dictSendTile.Add(ParameterCode.RoomData, roomData);
                            handTileData.tileCnt = (ushort)room.chairList[i].handTile.Count;
                            handTileData.handTile = room.chairList[i].handTile.ToArray();
                            dictSendTile.Add(ParameterCode.HandTileData, handTileData);
                            if(room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                            { 
                                room.chairList[i].peer.SendEvent(ServerToClient.SendTileToAll, dictSendTile);
                            }
                        }
                    }

                    break;
                }
                case ClientToServer.SelectTile:
                {
                    // 1.设置SelectTileData
                    SelectTileData selectTileData = Util.Convert<SelectTileData>(dict[ParameterCode.SelectTileData]);
                    selectTileData.selectTileId = playerData1.playerId;
                    room.roomInfo.selectTileReturnDatas.Add(selectTileData);
                    chair.isSelectTileReturn = true;
                    // 2. 如果四个人的选牌数据都已发送就开始处理
                    bool flag = true;
                    for (int i = 0; i < room.chairList.Count; ++i)
                    {
                        if (room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                        {
                            if (room.chairList[i].isSelectTileReturn == false)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (flag) 
                        room.OnSelectTileDataReturn();

                    break;
                }
                case ClientToServer.DingQue:
                {
                    DingQueData dingQueData = Util.Convert<DingQueData>(dict[ParameterCode.DingQueData]);
                    chair.queTileType = dingQueData.queTileType;
                    dingQueData.dingQueId = playerData1.playerId;
                    dict.Clear();
                    dict.Add(ParameterCode.DingQueData, dingQueData);
                    SendResponse(opCode, ReturnCode.success, dict);
                    SendEventToOther(ServerToClient.SendQueTileTypeToOther, dict);
                    chair.isDingQueReturn = true;
                    // 3. 如果四个玩家都已定缺就让庄家摸牌
                    bool flag = true;
                    for (int i = 0; i < room.chairList.Count; ++i)
                    {
                        if (room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                        {
                            if (room.chairList[i].isDingQueReturn == false)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if(flag)
                    {
                        for (int i = 0; i < room.chairList.Count; ++i)
                        {
                            if (room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                            {
                                if (room.chairList[i].peer.playerData1.playerId == room.roomInfo.masterClientID)
                                {
                                    room.chairList[i].peer.SendEvent(ServerToClient.SendToNextDrawTile, null);
                                    break;
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                    
                    break;
                }
                case ClientToServer.PlayTile:
                {
                    if (room != null)
                    {
                        // 1. 解析PlayTileData
                        PlayTileData playTileData = Util.Convert<PlayTileData>(dict[ParameterCode.PlayTileData]);
                        ushort playtilenum = playTileData.playTileNum;
                        bool isDrawTile = playTileData.isADrawTile;
                        bool isGangHouPlayTile = playTileData.isGangHouPlayTile;
                        room.roomInfo.playTileId = playerData1.playerId;
                        room.roomInfo.playTileNum = playtilenum;
                        // 2. 更新牌属性 
                        if (isDrawTile)
                        {
                            chair.drawTileNum = 0;
                            chair.throwTile.Add(playtilenum);
                        }
                        else
                        {
                            // 移除手牌中的出牌并将出牌添加进抛牌
                            chair.handTile.Remove(playtilenum);
                            if (chair.drawTileNum != 0)
                            {
                                chair.handTile.Add((ushort)chair.drawTileNum);
                                chair.drawTileNum = 0;
                            }
                            chair.throwTile.Add(playtilenum);
                        }
                        // 3. 重置出牌数据是否返回
                        for (int i = 0; i < room.chairList.Count; ++i)
                        {
                            room.chairList[i].isPlayTileReturn = false;
                        }
                        room.roomInfo.isExcutedPlayTileDataReturn = false;
                        chair.isPlayTileReturn = true;
                        // 3. 检测其他玩家是否胡碰杠吃 发送 1. 出牌数字 2.出牌ID 3. 胡的类型 4. 碰杠的类型
                        // 已经胡牌就不用检测
                        for (int i = 0; i < room.chairList.Count; ++i)
                        {
                            Chair chair = room.chairList[i];
                            if (chair.peer == null || chair.peer.IsConnected == false)
                                continue;
                            if(chair.peer.playerData1.playerId == playerData1.playerId)
                                continue;
                            HuTileType huType = HuTileType.buhu;
                            PengGangType pengGangType = PengGangType.none;
                            if(chair.isJoin)
                            {
                                huType = Check.OtherTileHuTileType(chair, playtilenum, isGangHouPlayTile, false);
                                pengGangType = Check.CheckPengGangType(chair, playtilenum);
                            }
                            Dictionary<short, object> dictEv = new Dictionary<short, object>();
                            PlayTileReturnData playTileReturnData = new PlayTileReturnData();
                            playTileReturnData.playTileNum = playtilenum;
                            playTileReturnData.playTileId = playerData1.playerId;
                            playTileReturnData.huTileType = huType;
                            playTileReturnData.pengGangType = pengGangType;
                            dictEv.Add(ParameterCode.PlayTileReturnData, playTileReturnData);
                            room.chairList[i].peer.SendEvent(ServerToClient.SendPlayTileDataToOther, dictEv);
                        }
                    }

                    break;
                }
                case ClientToServer.PlayTileDataReturn:
                {
                    // 1.获取胡牌类型，碰杠类型并保存到room中 
                    #region
                    HuTileType huTileType = HuTileType.buhu;
                    PengGangType pengGangType = PengGangType.none;
                    if(dict.ContainsKey(ParameterCode.HuTileType) && dict[ParameterCode.HuTileType] != null)
                    {
                        huTileType = Util.Convert<HuTileType>(dict[ParameterCode.HuTileType]);
                        if(huTileType != HuTileType.buhu)
                        {
                            HuTileReturnData huTileReturnData = new HuTileReturnData();
                            huTileReturnData.huTileId = playerData1.playerId;
                            huTileReturnData.huTileType = (ushort)huTileType;
                            huTileReturnData.huTile = chair.handTile.ToArray();
                            huTileReturnData.huTileCnt =  (ushort)chair.handTile.Count;
                            huTileReturnData.score = (ushort)chair.score;
                            room.roomInfo.huTileReturnDatas.Add(huTileReturnData);
                            Console.WriteLine("收到玩家请求：" + opCode.ToString() + 
                                playerData1.playerName + huTileType.ToString());
                        }
                    }
                    if (dict.ContainsKey(ParameterCode.PengGangType) && dict[ParameterCode.PengGangType] != null)
                    {
                        pengGangType = Util.Convert<PengGangType>(dict[ParameterCode.PengGangType]);
                        if(pengGangType != PengGangType.none)
                        {
                            PengGangReturnData pengGangReturnData = new PengGangReturnData();
                            pengGangReturnData.pengGangId = playerData1.playerId;
                            pengGangReturnData.pengGangTileNum = (ushort)room.roomInfo.playTileNum;
                            pengGangReturnData.pengGangType = pengGangType;
                            room.roomInfo.pengGangReturnDatas.Add(pengGangReturnData);
                            Console.WriteLine("收到玩家请求：" + opCode.ToString() + 
                                playerData1.playerName + pengGangType.ToString());
                        }
                    }

                    chair.isPlayTileReturn = true;

                    #endregion
                    // 2.如果四个玩家的playTileData都已返回, 则开始处理
                    bool flag = true;
                    for (int i = 0; i < room.chairList.Count; ++i)
                    {
                        if (room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                        {
                            if (room.chairList[i].isPlayTileReturn == false)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (flag) room.OnPlayTileDataReturn();
                    
                    break;
                }
                case ClientToServer.DrawTile:
                {
                    if(room != null)
                    {
                        // 如果已经胡牌就不能摸牌
                        if(chair.isJoin == false) break;
                        // 获取摸牌数字
                        int drawTileNum = 0;
                        if (room.Tiles.Count > 0)
                        {
                            drawTileNum = room.Tiles.Dequeue();
                        }
                        // 黄牌则向所有玩家宣布游戏结束
                        else
                        {
                            room.OnGameOver();
                            break;
                        }
                   
                        for(int i = 0; i < chair.handTile.Count; ++i)
                        {
                            Console.Write(((TileTypeTileNum)chair.handTile[i]).ToString() + " ");
                        }
                        ZiMoType ziMoType = ZiMoType.buzimo;
                        ShangHuaType shangHuaType = ShangHuaType.bushanghua;
                        PengGangType anGangJiaGangType = PengGangType.none;
                        // 1. 更新每个玩家的摸牌数字并判断摸牌玩家摸牌以后的自摸类型，是否杠后出牌，上花类型，暗杠加杠类型
                        for(int i = 0; i < room.chairList.Count; ++i)
                        {
                            if(room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                            {
                                if (room.chairList[i].peer.playerData1.playerId == playerData1.playerId)
                                {
                                    room.chairList[i].drawTileNum = drawTileNum;
                                    if (room.roomInfo.isGangHouPlayTile)
                                    {
                                        shangHuaType = Check.GangShangKaiHuaHuTileType(chair);
                                    }
                                    else
                                    {
                                        ziMoType = Check.ZiMoHuTileType(chair);
                                    }
                                    anGangJiaGangType = Check.CheckAnGangJiaGangType(room.chairList[i], drawTileNum);
                                    if (anGangJiaGangType != PengGangType.none) room.roomInfo.isGangHouPlayTile = true;
                                    else room.roomInfo.isGangHouPlayTile = false;
                                }
                                else room.chairList[i].drawTileNum = 0;
                            }

                        }
                        // 2. 客户端请求摸牌，服务器发送回复给客户端：1.剩余牌数 2.摸牌数字 3. 是否杠后摸牌
                        //                                      4.上花类型 5.自摸类型 6.杠牌类型 
                        Dictionary<short, object> dictResponse = new Dictionary<short, object>();
                        if(drawTileNum != 0)
                        {
                            DrawTileReturnData drawTileReturnData = new DrawTileReturnData(); 
                            drawTileReturnData.remainTile = (ushort)room.Tiles.Count;
                            drawTileReturnData.drawTileNum = (ushort)drawTileNum;
                            drawTileReturnData.isGangHouDrawTile = room.roomInfo.isGangHouPlayTile;
                            drawTileReturnData.shangHuaType = shangHuaType;
                            drawTileReturnData.ziMoType = ziMoType;
                            drawTileReturnData.anGangJiaGangType = anGangJiaGangType;
                            dictResponse.Add(ParameterCode.DrawTileReturnData, drawTileReturnData);
                            SendResponse(opCode, ReturnCode.success, dictResponse);
                            // 3. 服务器发送事件给所有人开启10s倒计时
                            TimerData timerData = new TimerData();
                            timerData.timerId = playerData1.playerId;
                            timerData.isPlay = true;
                            Dictionary<short, object>dictTimer = new Dictionary<short, object>() 
                            { {ParameterCode.TimerData, timerData } };
                            SendEventToAll(ServerToClient.SendTimerToAll, dictTimer);
                            // 4. 服务器发送事件给房间的其他玩家，更新余牌数字，显示其他玩家的摸牌
                            Dictionary<short, object> dictEv =  new Dictionary<short, object>();
                            DrawTileData drawTileData = new DrawTileData();
                            drawTileData.drawTileNum = (ushort)room.Tiles.Count;
                            drawTileData.drawTileId = playerData1.playerId;
                            dictEv.Add(ParameterCode.DrawTileData, drawTileData);
                            SendEventToOther(ServerToClient.SendDrawTileDataToOther, dictEv);
                        }
                        else
                        {
                            dictResponse.Add(ParameterCode.error, "获取摸牌失败");
                            SendResponse(opCode, ReturnCode.fail, dictResponse);
                        }

                    }
                    
                    break;
                }
                case ClientToServer.DrawTileDataReturn:
                {
                    // 获取自摸类型，杠上开花类型，暗杠加杠类型，如果是加杠判断是否其他玩家可以抢杠
                    #region
                    if (dict.ContainsKey(ParameterCode.ZiMoType) && dict[ParameterCode.ZiMoType] != null)
                    {

                        // 1.关闭需要出牌计时
                        Console.WriteLine(playerData1.playerName + "自摸数字" + chair.drawTileNum);
                        // 2. 获取自摸类型，自摸Id，自摸数字
                        ZiMoType ziMoType = Util.Convert<ZiMoType>(dict[ParameterCode.ZiMoType]);
                        ushort ziMoId = playerData1.playerId;
                        ushort ziMoNum = (ushort)chair.drawTileNum;
                        // 3. 获取自摸玩家手牌
                        List<ushort>ziMoHandTile = new List<ushort>(chair.handTile)
                        {
                            ziMoNum
                        };
                        if (chair.cpg.Count > 0)
                        {
                            foreach (var item in chair.cpg)
                            {
                                for (int k = 0; k < 3; ++k)
                                    ziMoHandTile.Add((ushort)item.cpgList[k]);
                            }
                        }
                        // 4.计算并更新四个玩家获得的分数
                        Settle settle = new Settle();
                        uint winScore = (uint)settle.GetZiMoScore(room.roomInfo.roomType, ziMoType, 1);
                        playerData1.score += winScore * 3;
                        for (int i = 0; i < room.chairList.Count; ++i)
                        {
                            if (room.chairList[i].peer != this)
                            {
                                room.chairList[i].peer.playerData1.score -= winScore;
                            }
                        }
                        List<int> scoreList = new List<int>();
                        for (int i = 0; i < room.chairList.Count; ++i)
                        {
                            room.chairList[i].score = (int)room.chairList[i].peer.playerData1.score;
                            scoreList.Add(room.chairList[i].score);
                        }
                        // 5. 更新玩家是否胡牌
                        if(!ziMoType.Equals(ZiMoType.buzimo)) chair.isJoin = false;
                        // 自摸事件返回数据 ： 1.自摸类型 2.自摸玩家Id 3.自摸数字 4.自摸玩家的手牌 5. 每个玩家的分数
                        Dictionary<short, object> dictZiMo = new Dictionary<short, object>();
                        HuTileReturnData huTileReturnData = new HuTileReturnData();
                        huTileReturnData.huTileId = ziMoId;
                        huTileReturnData.huTileType = (ushort)ziMoType;
                        huTileReturnData.huTile = ziMoHandTile.ToArray();
                        huTileReturnData.huTileCnt = (ushort)ziMoHandTile.Count;
                        huTileReturnData.score = (ushort)chair.score;
                        dictZiMo.Add(ParameterCode.HuTileReturnData, huTileReturnData);
                        // 6.向所有人发送自摸事件
                        SendEventToAll(ServerToClient.SendZiMoDataToAll, dictZiMo);
                        // 7.判断是否游戏结束
                        room.OnGameOver();
                        // 8.更新游戏状态 

                    }
                    else if(dict.ContainsKey(ParameterCode.ShangHuaType) && dict[ParameterCode.ShangHuaType] != null)
                    {
                        // 1. 关闭需要出牌计时
                        // 2. 获取上花类型，上花Id，上花数字
                        Console.WriteLine(playerData1.playerName + "上花数字" + chair.drawTileNum);
                        ShangHuaType shangHuaType = Util.Convert<ShangHuaType>(dict[ParameterCode.ShangHuaType]);
                        ushort shangHuaNum = (ushort)chair.drawTileNum;
                        ushort shangHuaId =  playerData1.playerId;
                        // 3. 获取杠上开花玩家的手牌
                        List<ushort> shangHuaHandTile = new List<ushort>(chair.handTile)
                        {
                            shangHuaNum
                        };
                        if (chair.cpg.Count > 0)
                        {
                            foreach (var item in chair.cpg)
                            {
                                for (int k = 0; k < 3; ++k)
                                    shangHuaHandTile.Add((ushort)item.cpgList[k]);
                            }
                        }
                        // 4.  获取每个玩家的分数
                        // 先更新杠上开花玩家的分数，再更新其他玩家的分数
                        Settle settle = new Settle();
                        uint winScore = (uint)settle.GetShangHuaScore(room.roomInfo.roomType, shangHuaType, 1);
                        playerData1.score += winScore * 3;
                        for (int i = 0; i < room.chairList.Count; ++i)
                        {
                            if (room.chairList[i].peer != this)
                            {
                                room.chairList[i].peer.playerData1.score -= winScore;
                            }
                        }
                        // 保存每个玩家的积分到服务器上，并且发送给客户端
                        List<int> scoreList = new List<int>();
                        for (int i = 0; i < room.chairList.Count; ++i)
                        {
                            room.chairList[i].score = (int)room.chairList[i].peer.playerData1.score;
                            scoreList.Add(room.chairList[i].score);
                        }
                        // 5. 更新玩家是否胡牌
                        if(!shangHuaType.Equals(ShangHuaType.bushanghua)) chair.isJoin = false;
                        // 杠上开花事件返回数据:1.杠上开花类型 2.杠上开花玩家Id 3.杠上开花摸牌数字
                        // 4.杠上开花玩家的手牌 5.每个玩家的分数
                        Dictionary<short, object> dictShangHua= new Dictionary<short, object>();
                        HuTileReturnData huTileReturnData = new HuTileReturnData();
                        huTileReturnData.huTileId = shangHuaId;
                        huTileReturnData.huTileType = (ushort)shangHuaType;
                        huTileReturnData.huTile =  shangHuaHandTile.ToArray();
                        huTileReturnData.huTileCnt = (ushort)shangHuaHandTile.Count;
                        huTileReturnData.score = (ushort)chair.score;
                        dictShangHua.Add(ParameterCode.HuTileReturnData, huTileReturnData);
                        // 向所有人发送杠上开花事件
                        SendEventToAll(ServerToClient.SendShangHuaDataToAll, dictShangHua);
                        // 调用OnGameOver并更新游戏状态
                        room.OnGameOver();
                    }
                    else if(dict.ContainsKey(ParameterCode.PengGangType) && dict[ParameterCode.PengGangType] != null)
                    {
                        Console.WriteLine(playerData1.playerName + "暗杠加杠数字" + chair.drawTileNum);
                        // 1. 获取暗杠加杠类型，暗杠加杠Id，暗杠加杠数字
                        PengGangType anGangJiaGangType = Util.Convert<PengGangType>(dict[ParameterCode.PengGangType]);
                        ushort anGangJiaGangId = playerData1.playerId;
                        ushort anGangJiaGangNum = (ushort)chair.drawTileNum;
                        // 将杠牌添加进手牌
                        if(chair.drawTileNum != 0)
                        {
                            chair.handTile.Add((ushort)chair.drawTileNum);
                            chair.drawTileNum = 0;
                        }
                        if(anGangJiaGangType == PengGangType.angang)
                        { 
                            // 将暗杠的四张牌从手牌中删除
                            for(int i = 0; i < 4; ++i)
                            {
                                chair.handTile.Remove(anGangJiaGangNum);
                            }
                            // 将暗杠的四张牌添加进吃碰杠牌中
                            chair.cpg.Add(new CPGProperties 
                            {
                                cpgList = new List<int>
                                {anGangJiaGangNum, anGangJiaGangNum, anGangJiaGangNum, anGangJiaGangNum},
                                cpgType = PengGangType.angang
                            });
                            // 给所有玩家发送暗杠数据：1. 暗杠牌数字 2.暗杠牌玩家Id 3.暗杠牌类型
                            Dictionary<short, object> dictAnGang = new Dictionary<short, object>();
                            PengGangReturnData pengGangReturnData = new PengGangReturnData();
                            pengGangReturnData.pengGangId = anGangJiaGangId;
                            pengGangReturnData.pengGangTileNum = anGangJiaGangNum;
                            pengGangReturnData.pengGangType = anGangJiaGangType;
                            dictAnGang.Add(ParameterCode.PengGangReturnData, pengGangReturnData);
                            SendEventToAll(ServerToClient.SendAnGangJiaGangDataToAll, dictAnGang);

                            // 更新房间是否杠后出牌，暗杠玩家摸牌
                            room.roomInfo.isGangHouPlayTile = true;
                            for(int i = 0; i < room.chairList.Count; ++i)
                            {
                                if(room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                                {
                                    if(room.chairList[i].peer.playerData1.playerId == anGangJiaGangId)
                                    {
                                        room.chairList[i].peer.SendEvent(ServerToClient.SendToNextDrawTile, null);
                                        break;
                                    }
                                    // 下一个摸牌的玩家掉线了
                                    else
                                    {

                                    }
                                }
                            }
                        }
                        else if(anGangJiaGangType == PengGangType.jiagang)
                        {
                            // 删除手牌中的一张杠牌
                            chair.handTile.Remove(anGangJiaGangNum);
                            // 添加一张杠牌到吃碰杠牌
                            for(int i = 0; i < chair.cpg.Count; ++i)
                            {
                                if(chair.cpg[i].cpgList[0] == anGangJiaGangNum)
                                {
                                    chair.cpg[i].cpgList.Add(anGangJiaGangNum);
                                    chair.cpg[i].cpgType = PengGangType.jiagang;
                                    break;
                                }
                            }
                            // 给所有玩家发送加杠数据：1. 加杠牌数字 2.加杠牌玩家Id 3.加杠牌类型
                            Dictionary<short, object> dictJiaGang = new Dictionary<short, object>();
                            PengGangReturnData pengGangReturnData = new PengGangReturnData();
                            pengGangReturnData.pengGangId = anGangJiaGangId;
                            pengGangReturnData.pengGangTileNum = anGangJiaGangNum;
                            pengGangReturnData.pengGangType = anGangJiaGangType;
                            dictJiaGang.Add(ParameterCode.PengGangReturnData, pengGangReturnData);
                            SendEventToAll(ServerToClient.SendAnGangJiaGangDataToAll, dictJiaGang);
                            // 重置抢杠数据
                            for (int i = 0; i < room.chairList.Count; ++i)
                            {
                                room.chairList[i].isQiangGangReturn = false;
                            }
                            room.roomInfo.isExcutedQiangGangDataReturn = false;
                            chair.isQiangGangReturn = true;
                            // 更新加杠玩家Id
                            room.roomInfo.JiaGangId = anGangJiaGangId;
                            // 检测其他玩家是否抢杠并发送抢杠数据: 1.抢杠胡牌类型
                            for (int i = 0; i < room.chairList.Count; ++i)
                            {
                                if(room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                                {
                                    if(room.chairList[i].peer.playerData1.playerId != anGangJiaGangId)
                                    {
                                        Dictionary<short, object> dictQiangGang = new Dictionary<short, object>();
                                        HuTileType huTileType = HuTileType.buhu;
                                        if(chair.isJoin)
                                        {
                                            huTileType = Check.OtherTileHuTileType(chair, anGangJiaGangNum, false, true);
                                        }
                                        QiangGangData qiangGangData = new QiangGangData();
                                        qiangGangData.huTileType = huTileType;
                                        dictQiangGang.Add(ParameterCode.QiangGangData, qiangGangData);
                                        room.chairList[i].peer.SendEvent(ServerToClient.SendQiangGangDataToOther, dictQiangGang);
                                    }
                                }
                            }
                            // 给所有玩家发送开启10s倒计时的事件

                            // 关闭房间内需要出牌计时

                            // 开启房间内抢杠计时
                        }
                    }
                    #endregion

                    break;
                }
                case ClientToServer.QiangGangDataReturn:
                {
                    // 将抢杠数据添加进房间的抢杠数据列表
                    HuTileType huTileType = Util.Convert<QiangGangData>(dict[ParameterCode.QiangGangData]).huTileType;
                    if(huTileType != HuTileType.buhu)
                    {
                        HuTileReturnData huTileReturnData = new HuTileReturnData();
                        huTileReturnData.huTileId = playerData1.playerId;
                        huTileReturnData.huTileType = (ushort)huTileType;
                        huTileReturnData.huTile = chair.handTile.ToArray();
                        huTileReturnData.huTileCnt =  (ushort)chair.handTile.Count;
                        huTileReturnData.score = (ushort)chair.score;
                        room.roomInfo.qiangGangReturnDatas.Add(huTileReturnData);
                    }
                    chair.isQiangGangReturn = true;
                    // 如果四个玩家的QiangGangData都已返回, 则开始处理
                    bool flag = true;
                    for(int i = 0; i < room.chairList.Count; ++i)
                    {
                        if(room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                        {
                            if(room.chairList[i].isQiangGangReturn == false)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if(flag) room.OnQiangGangDataReturn();

                    break;
                }
                case ClientToServer.ContinueGame:
                {
                    playerData1.isReady = false;
                    playerData1.score = 0;
                    chair.score = 0;
                    chair.handTile.Clear();
                    chair.isJoin = true;
                    chair.handTile = new List<ushort>();
                    chair.drawTileNum = 0;
                    chair.throwTile = new List<int>();
                    chair.cpg = new List<CPGProperties>();
                    chair.isSelectTileReturn = false;
                    chair.isDingQueReturn = false;
                    chair.isPlayTileReturn = false;
                    chair.isQiangGangReturn = false;
                    SendResponse(opCode, ReturnCode.success, null);
                }
                break;
                default:
                    break;
            }
        }
    }
}
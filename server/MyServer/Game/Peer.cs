using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGServer;

namespace MyServer
{
    public class Peer : PeerBase
    {
        public Player player = new Player();
        public UserInfo userInfo = new UserInfo();
        public Room room = null;
        public Chair chair = null;
        public PlayerPosition[] playerPositions = {PlayerPosition.east, PlayerPosition.south, PlayerPosition.west, PlayerPosition.north};
        
        /// <summary>
        /// 当客户端断开连接时
        /// </summary>
        /// <param name="e"></param>
        public override void OnDisConnected(Exception e)
        {
            Console.WriteLine("OnDisConnected:" + e.ToString());
        }

        /// <summary>
        /// 当客户端出现异常时
        /// </summary>
        /// <param name="exception"></param>
        public override void OnException(Exception exception)
        {
            Console.WriteLine("OnException:" + exception.ToString());
        }


        /// <summary>
        /// 发送事件给下家，对家，上家
        /// </summary>
        /// <param name="eventcode"></param>
        /// <param name="dict"></param>
        private void SendEventToOther(short eventcode, Dictionary<short, object> dict)
        {
            if (room != null && chair != null)
            {
                for (int i = 0; i < room.chairList.Count; ++i)
                {
                    if (chair != room.chairList[i])
                    {
                        if (room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                        {
                            room.chairList[i].peer.SendEvent(eventcode, dict);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 发送事件给所有玩家
        /// </summary>
        /// <param name="eventcode"></param>
        /// <param name="dict"></param>
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

        /// <summary>
        /// 当客户端发送请求过来时
        /// </summary>
        /// <param name="opCode"></param>
        /// <param name="dict"></param>
        public override void OnOperationRequest(short opCode, Dictionary<short, object> dict)
        {
            OpCode opcode = (OpCode)opCode;
            switch (opcode)
            {
                case OpCode.Test:
                    break;
                case OpCode.Login:
                    {
                        // 1. 接收客户端发送的Id并向json文件查询Id对应的用户信息
                        int id =  Convert<int>(dict[ParameterCode.id]);
                        string username = DataManager.QueryData<UserInfo, string>(id.ToString(), "username");
                        int coin = DataManager.QueryData<UserInfo, int>(id.ToString(), "coin");
                        int diamond = DataManager.QueryData<UserInfo, int>(id.ToString(), "diamond");
                        int sex = DataManager.QueryData<UserInfo, int>(id.ToString(), "sex");
                        Console.WriteLine("username: " + username + "coin: " + coin + "diamond: " + diamond + "sex: " + sex);
                        // 2.设置player信息和用户信息
                        player.username = username;
                        player.score = 0;
                        player.sex = sex;
                        userInfo.id = id;
                        userInfo.username = username;
                        userInfo.coin = coin;
                        userInfo.diamond = diamond;
                        userInfo.sex = sex;
                        // 4.服务器 => 客户端： 1. id 2. 用户名 3. 金币 4. 钻石 5.性别
                        Dictionary<short, object> dictUserInfo = new Dictionary<short, object>();
                        dictUserInfo.Add(ParameterCode.userInfo, userInfo);
                        SendResponse(opCode, ReturnCode.success, dictUserInfo);
                    }
                    break;
                case OpCode.CreateRoomPanel:
                    {
                        // 1.设置房间信息
                        room = new Room();
                        Random ran = new Random();     
                        room.roomInfo.roomName = ran.Next(10001, 100000);
                        ++room.roomInfo.AllActorCounts;
                        room.roomInfo.masterClientID = room.roomInfo.AllActorCounts;
                        room.chairList[0].peer = this;
                        room.chairList[0].isJoin = true;
                        chair = room.chairList[0];
                        // 2.设置玩家信息并将房间添加到房间字典
                        player.actorId = room.roomInfo.AllActorCounts;
                        player.playerPos = PlayerPosition.east;
                        player.isMaster = true;
                        AllGames.dictRooms.Add(room.roomInfo.roomName, room);
                        // 3.服务器回复房间信息给客户端 1. 房间信息 2. 本家
                        Dictionary<short, object> dictResponse = new Dictionary<short, object>();
                        dictResponse.Add(ParameterCode.roomInfo, room.roomInfo);
                        dictResponse.Add(ParameterCode.player, player);
                        SendResponse(opCode, ReturnCode.success, dictResponse);
                    }
                    break;
                case OpCode.JoinRoomPanel:
                    {
                        int roomName = int.Parse(Convert<string>(dict[ParameterCode.roomName]));
                        if(AllGames.dictRooms.ContainsKey(roomName))
                        {
                            // 1. 从房间字典中获取房间信息，加入房间并坐上椅子
                            room = AllGames.dictRooms[roomName];
                            player.score = 0;
                            for(int i = 0; i < room.chairList.Count; ++i)
                            {
                                if(room.chairList[i].isJoin == false)
                                {
                                    chair = room.chairList[i];
                                    room.chairList[i].peer = this;
                                    room.chairList[i].isJoin = true;
                                    player.playerPos = room.chairList[i].playerPos;
                                    ++room.roomInfo.AllActorCounts;
                                    player.actorId = room.roomInfo.AllActorCounts;
                                    break;
                                }
                            }
                            // 2.获取其他玩家的信息 playerList
                            List<Player> playerList = new List<Player>();
                            for(int i = 0; i < room.chairList.Count; ++i)
                            {
                                if(room.chairList[i].isJoin == true)
                                {
                                    playerList.Add(room.chairList[i].peer.player);
                                }
                            }
                            // 3. 服务器发送回复给未加入玩家的客户端： 1. 房间信息 2. 加入房间的玩家 3. 房间里的其他玩家
                            Dictionary<short, object> dictResponse = new Dictionary<short, object>();
                            dictResponse.Add(ParameterCode.roomInfo, room.roomInfo);
                            dictResponse.Add(ParameterCode.player, player);
                            dictResponse.Add(ParameterCode.playerList,  playerList);
                            SendResponse(opCode, ReturnCode.success, dictResponse);
                            // 4. 服务器通知其他已加入玩家的客户端： 1. 加入房间的玩家 2.房间信息
                            Dictionary<short, object> dictSendPlayer = new Dictionary<short, object>();
                            dictSendPlayer.Add(ParameterCode.player, player);
                            dictSendPlayer.Add(ParameterCode.roomInfo, room.roomInfo);
                            SendEventToOther((short)(EventCode.SendPlayerToOther), dictSendPlayer);
                        }
                    }
                    break;
                case OpCode.LeaveRoomPanel:
                    {
                        // 1. 设置椅子和玩家的信息
                        chair.isJoin = false;
                        player.isReady = false;
                        // 2. 如果庄家离开房间， 就要改变masterClinentID
                        if(player.actorId == room.roomInfo.masterClientID)
                        {
                            room.roomInfo.masterClientID = player.actorId == 4 ? 1 : (player.actorId + 1);
                        }

                        // 3. 服务器发送回复给客户端
                        SendResponse(opCode, ReturnCode.success, null);
                        // 4. 服务器通知其他未离开玩家， 有玩家离开房间
                        Dictionary<short, object> dictLeaveRoom = new Dictionary<short, object>();
                        dictLeaveRoom.Add(ParameterCode.actorId, player.actorId);
                        SendEventToOther((short)(EventCode.SendLeaveRoomIdToOther), dictLeaveRoom);
                    }
                    break;
                case OpCode.ClickReady:
                    {
                        // 1.设置玩家的信息并回复客户端
                        player.isReady = true;
                        SendResponse(opCode, ReturnCode.success, null);
                        // 2.服务器通知其他玩家，有玩家点击准备
                        Dictionary<short, object> dictReady = new Dictionary<short, object>();
                        dictReady.Add(ParameterCode.actorId, player.actorId);
                        SendEventToOther((short)(EventCode.SendReadyPlayerIdToOther), dictReady);
                        bool flag = true;
                        // 3.如果房间人满，且每个人都已准备就发牌
                        for(int i = 0; i < room.chairList.Count; ++i)
                        {
                            if(room.chairList[i].peer == null || room.chairList[i].peer.player.isReady == false)
                            {
                                flag = false;
                            }
                        }
                        if(flag)
                        {
                            // 4.初始化108张牌并且给每个人发送13张手牌
                            room.InitTiles();
                            List<List<int>>handTiles = new List<List<int>>(4)
                            { 
                                new List<int>(13),
                                new List<int>(13),
                                new List<int>(13),
                                new List<int>(13)
                            };
                            for(int i = 0; i < handTiles.Count; ++i)
                            {
                                for(int j = 0; j < 13; ++j)
                                    handTiles[i].Add(room.Tiles.Dequeue());
                            }
                            // 5.设置房间信息：玩家手牌 
                            for(int i = 0; i < room.chairList.Count; ++i)
                            {
                                room.chairList[i].handTile = handTiles[i];
                            }
                            // 6.服务器给每个玩家发送： 1. 房间剩余牌数 2.13张牌
                            for (int i = 0; i < room.chairList.Count; ++i)
                            {
                                Dictionary<short, object> dictSendTile = new Dictionary<short, object>();
                                dictSendTile.Add(ParameterCode.remainTile, room.Tiles.Count);
                                dictSendTile.Add(ParameterCode.handTile, room.chairList[i].handTile);
                                if(room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                                { 
                                    room.chairList[i].peer.SendEvent((short)EventCode.SendTileToAll, dictSendTile);
                                }
                            }
                        }
                    }
                    break;
                case OpCode.SelectTile:
                    {
                        // 1.获取选牌列表，选牌Id并添加到房间的选牌数据
                        List<int>selectTileList = Convert<List<int>>(dict[ParameterCode.selectTileList]);
                        int selectTileId = Convert<int>(dict[ParameterCode.actorId]);
                        SelectTileReturnData selectTileReturnData = new SelectTileReturnData();
                        selectTileReturnData.selectTileList = selectTileList;
                        selectTileReturnData.selectTileId = selectTileId;
                        room.roomInfo.selectTileReturnDatas.Add(selectTileReturnData);
                        chair.isSelectTileReturn = true;
                        // 3. 如果四个人的选牌数据都已发送就开始处理
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
                        if (flag) room.OnSelectTileDataReturn();
                    }
                    break;
                case OpCode.DingQue:
                    {
                        TileType queTileType = Convert<TileType>(dict[ParameterCode.queTileType]);
                        chair.queTileType = queTileType;
                        Dictionary<short, object> dictSendQueTileType = new Dictionary<short, object>();
                        dictSendQueTileType.Add(ParameterCode.queTileType, queTileType);
                        dictSendQueTileType.Add(ParameterCode.actorId, chair.peer.player.actorId);
                        SendEventToOther((short)EventCode.SendQueTileTypeToOther, dictSendQueTileType);
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
                                    if (room.chairList[i].peer.player.actorId == room.roomInfo.masterClientID)
                                    {
                                        room.chairList[i].peer.SendEvent((short)EventCode.SendToNextDrawTile, null);
                                        break;
                                    }
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                    break;
                case OpCode.PlayTile:
                    {
                        if (room != null)
                        {
                            int playtilenum = (int)(long)(dict[ParameterCode.playTileNum]);
                            bool isDrawTile = (bool)(dict[ParameterCode.isDrawTile]);
                            bool isGangHouPlayTile = Convert<bool>(dict[ParameterCode.isGangHouPlayTile]);

                            room.roomInfo.playTileId = player.actorId;
                            room.roomInfo.playTileNum = playtilenum;
                            // 更新牌属性 
                            if (isDrawTile)
                            {
                                chair.drawTileNum = 0;
                                chair.throwTile.Add(playtilenum);
                            }
                            else
                            {
                                // 移除手牌中的出牌并将出牌添加进抛牌
                                for (int i = 0; i < chair.handTile.Count; ++i)
                                {
                                    if (playtilenum == chair.handTile[i])
                                    {
                                        chair.handTile.RemoveAt(i);
                                        break;
                                    }
                                }
                                if (chair.drawTileNum != 0)
                                {
                                    chair.handTile.Add(chair.drawTileNum);
                                    chair.drawTileNum = 0;
                                }
                                chair.throwTile.Add(playtilenum);
                            }

                            for (int i = 0; i < room.chairList.Count; ++i)
                            {
                                room.chairList[i].isPlayTileReturn = false;
                            }
                            room.roomInfo.isExcutedPlayTileDataReturn = false;
                            chair.isPlayTileReturn = true;
                            // 检测其他玩家是否胡碰杠吃 发送 1. 出牌数字 2.出牌ID 3. 胡的类型 4. 碰杠的类型
                            // 已经胡牌就不用检测
                            for (int i = 0; i < room.chairList.Count; ++i)
                            {
                                if(room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                                {

                                    if(room.chairList[i].peer.player.actorId != player.actorId)
                                    {
                                        
                                        OtherPlayTileType huType = OtherPlayTileType.buhu;
                                        PengGangType pengGangType = PengGangType.none;
                                        if(room.chairList[i].isJoin)
                                        {
                                            huType = Check.OtherTileHuTileType(room.chairList[i], playtilenum, isGangHouPlayTile, false);
                                            pengGangType = Check.CheckPengGangType(room.chairList[i], playtilenum);
                                            if (!huType.Equals(OtherPlayTileType.buhu))
                                            {
                                                Console.WriteLine(room.chairList[i].peer.player.username + huType.ToString());
                                                for (int j = 0; j < room.chairList[i].handTile.Count; ++j)
                                                {
                                                    Console.Write(room.chairList[i].handTile[j] + " ");
                                                }
                                                Console.WriteLine();
                                            }
                                        }
                                        Dictionary<short, object> dictEv = new Dictionary<short, object>();
                                        dictEv.Add(ParameterCode.playTileNum, playtilenum);
                                        dictEv.Add(ParameterCode.playTileId, player.actorId);
                                        dictEv.Add(ParameterCode.otherPlayTileType, huType);
                                        dictEv.Add(ParameterCode.PengGangType, pengGangType);
                                        room.chairList[i].peer.SendEvent((short)EventCode.SendPlayTileDataToOther, dictEv);
                                        if(!huType.Equals(OtherPlayTileType.buhu))
                                        {
                                            Console.WriteLine(room.chairList[i].peer.player.username + huType.ToString());
                                            for (int j = 0; j < room.chairList[i].handTile.Count; ++j)
                                            {
                                                Console.Write(room.chairList[i].handTile[j] + " ");
                                            }
                                            Console.WriteLine();
                                        }
                                            
                                    }
                                }
                            }
                        }
                    }
                    break;
                case OpCode.PlayTileDataReturn:
                    {
                        // 1.获取胡牌类型，碰杠类型并保存到room中 
                        #region
                        OtherPlayTileType otherPlayTileType = OtherPlayTileType.buhu;
                        PengGangType pengGangType = PengGangType.none;
                        if(dict.ContainsKey(ParameterCode.otherPlayTileType) && dict[ParameterCode.otherPlayTileType] != null)
                        {
                            otherPlayTileType = Convert<OtherPlayTileType>(dict[ParameterCode.otherPlayTileType]);
                            if(otherPlayTileType != OtherPlayTileType.buhu)
                            {
                                OtherPlayTileReturnData otherPlayTileReturnData = new OtherPlayTileReturnData();
                                otherPlayTileReturnData.otherPlayTileActorId = player.actorId;
                                otherPlayTileReturnData.otherPlayTileType = otherPlayTileType;
                                room.roomInfo.otherPlayTileReturnDatas.Add(otherPlayTileReturnData);
                                Console.WriteLine("收到玩家请求：" + opcode.ToString() + player.username + otherPlayTileType.ToString());
                            }
                        }
                        if (dict.ContainsKey(ParameterCode.PengGangType) && dict[ParameterCode.PengGangType] != null)
                        {
                            pengGangType = Convert<PengGangType>(dict[ParameterCode.PengGangType]);
                            if(pengGangType != PengGangType.none)
                            {
                                PengGangReturnData pengGangReturnData = new PengGangReturnData();
                                pengGangReturnData.pengGangActorId = player.actorId;
                                pengGangReturnData.PengGangType = pengGangType;
                                room.roomInfo.pengGangReturnDatas.Add(pengGangReturnData);
                                Console.WriteLine("收到玩家请求：" + opcode.ToString() + player.username + pengGangType.ToString());
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
                    }
                    break;
                case OpCode.DrawTile:
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
                            Console.WriteLine();
                            Console.WriteLine(player.username + "摸到的牌为" + ((TileTypeTileNum)drawTileNum).ToString());
                            Console.WriteLine("当前的手牌为 ");
                            for(int i = 0; i < chair.handTile.Count; ++i)
                            {
                                Console.Write(((TileTypeTileNum)chair.handTile[i]).ToString() + " ");
                            }
                            ZiMoType ziMoType = ZiMoType.buzimo;
                            ShangHuaType shangHuaType = ShangHuaType.bushanghua;
                            PengGangType anGangJiaGangType = PengGangType.none;
                            // 更新每个玩家的摸牌数字并判断摸牌玩家摸牌以后的自摸类型，是否杠后出牌，上花类型，暗杠加杠类型
                            for(int i = 0; i < room.chairList.Count; ++i)
                            {
                                if(room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                                {
                                    if (room.chairList[i].peer.player.actorId == player.actorId)
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
                            // 客户端请求摸牌，服务器发送回复给客户端：1.剩余牌数 2.摸牌数字 3. 是否杠后摸牌
                            //                                      4.上花类型 5.自摸类型 6.杠牌类型 
                            Dictionary<short, object> dictResponse = new Dictionary<short, object>();
                            if(drawTileNum != 0)
                            {
                                dictResponse.Add(ParameterCode.remainTile, room.Tiles.Count);
                                dictResponse.Add(ParameterCode.drawTileNum, drawTileNum);
                                dictResponse.Add(ParameterCode.isGangHouDrawTile, room.roomInfo.isGangHouPlayTile);
                                dictResponse.Add(ParameterCode.shangHuaType, shangHuaType);
                                dictResponse.Add(ParameterCode.ziMoType, ziMoType);
                                dictResponse.Add(ParameterCode.anGangJiaGangType, anGangJiaGangType);
                                SendResponse(opCode, ReturnCode.success, dictResponse);

                                // 服务器发送事件给所有人开启10s倒计时并开启房间内需要出牌计时

                                // 服务器发送事件给房间的其他玩家，更新余牌数字，显示其他玩家的摸牌
                                Dictionary<short, object> dictEv =  new Dictionary<short, object>();
                                dictEv.Add(ParameterCode.remainTile, room.Tiles.Count);
                                dictEv.Add(ParameterCode.drawTileId, player.actorId);
                                SendEventToOther((short)EventCode.SendDrawTileDataToOther, dictEv);
                            }
                            else
                            {
                                dictResponse.Add(ParameterCode.error, "获取摸牌失败");
                                SendResponse(opCode, ReturnCode.fail, dictResponse);
                            }

                        }
                    }
                    break;
                case OpCode.DrawTileDataReturn:
                    {
                        // 获取自摸类型，杠上开花类型，暗杠加杠类型，如果是加杠判断是否其他玩家可以抢杠
                        #region
                        if(dict.ContainsKey(ParameterCode.ziMoType) && dict[ParameterCode.ziMoType] != null)
                        {

                            // 1.关闭需要出牌计时
                            Console.WriteLine(player.username + "自摸数字" + chair.drawTileNum);
                            // 2. 获取自摸类型，自摸Id，自摸数字
                            ZiMoType ziMoType = Convert<ZiMoType>(dict[ParameterCode.ziMoType]);
                            if (!ziMoType.Equals(ZiMoType.buzimo))
                            {
                                Console.WriteLine(player.username + ziMoType.ToString());
                                for (int j = 0; j < chair.handTile.Count; ++j)
                                {
                                    Console.Write(chair.handTile[j] + " ");
                                }
                                Console.WriteLine();
                            }
                            int ziMoId = player.actorId;
                            int ziMoNum = chair.drawTileNum;
                            // 3. 获取自摸玩家手牌
                            List<int>ziMoHandTile = new List<int>(chair.handTile);
                            ziMoHandTile.Add(ziMoNum);
                            if (chair.cpg.Count > 0)
                            {
                                foreach (var item in chair.cpg)
                                {
                                    for (int k = 0; k < 3; ++k)
                                        ziMoHandTile.Add(item.cpgList[k]);
                                }
                            }
                            // 4.计算并更新四个玩家获得的分数
                            Settle settle = new Settle();
                            int winScore = settle.GetZiMoScore(room.roomInfo.roomType, ziMoType, 1);
                            player.score += winScore * 3;
                            for (int i = 0; i < room.chairList.Count; ++i)
                            {
                                if (room.chairList[i].peer != this)
                                {
                                    room.chairList[i].peer.player.score -= winScore;
                                }
                            }
                            List<int> scoreList = new List<int>();
                            for (int i = 0; i < room.chairList.Count; ++i)
                            {
                                room.chairList[i].score = room.chairList[i].peer.player.score;
                                scoreList.Add(room.chairList[i].score);
                            }
                            // 5. 更新玩家是否胡牌
                            if(!ziMoType.Equals(ZiMoType.buzimo)) chair.isJoin = false;
                            // 自摸事件返回数据 ： 1.自摸类型 2.自摸玩家Id 3.自摸数字 4.自摸玩家的手牌 5. 每个玩家的分数
                            Dictionary<short, object> dictZiMo = new Dictionary<short, object>();
                            dictZiMo.Add(ParameterCode.ziMoType, ziMoType);
                            dictZiMo.Add(ParameterCode.ziMoId, ziMoId);
                            dictZiMo.Add(ParameterCode.ziMoNum, ziMoNum);
                            dictZiMo.Add(ParameterCode.handTile, ziMoHandTile);
                            dictZiMo.Add(ParameterCode.scoreList, scoreList);
                            // 6.向所有人发送自摸事件
                            SendEventToAll((short)EventCode.SendZiMoDataToAll, dictZiMo);
                            // 7.判断是否游戏结束
                            room.OnGameOver();
                            // 8.更新游戏状态 

                        }
                        else if(dict.ContainsKey(ParameterCode.shangHuaType) && dict[ParameterCode.shangHuaType] != null)
                        {
                            // 1. 关闭需要出牌计时
                            // 2. 获取上花类型，上花Id，上花数字
                            Console.WriteLine(player.username + "上花数字" + chair.drawTileNum);
                            ShangHuaType shangHuaType = Convert<ShangHuaType>(dict[ParameterCode.shangHuaType]);
                            int shangHuaNum = chair.drawTileNum;
                            int shangHuaId =  player.actorId;
                            // 3. 获取杠上开花玩家的手牌
                            List<int> shangHuaHandTile = new List<int>(chair.handTile);
                            shangHuaHandTile.Add(shangHuaNum);
                            if (chair.cpg.Count > 0)
                            {
                                foreach (var item in chair.cpg)
                                {
                                    for (int k = 0; k < 3; ++k)
                                        shangHuaHandTile.Add(item.cpgList[k]);
                                }
                            }
                            // 4.  获取每个玩家的分数
                            // 先更新杠上开花玩家的分数，再更新其他玩家的分数
                            Settle settle = new Settle();
                            int winScore = settle.GetShangHuaScore(room.roomInfo.roomType, shangHuaType, 1);
                            player.score += winScore * 3;
                            for (int i = 0; i < room.chairList.Count; ++i)
                            {
                                if (room.chairList[i].peer != this)
                                {
                                    room.chairList[i].peer.player.score -= winScore;
                                }
                            }
                            // 保存每个玩家的积分到服务器上，并且发送给客户端
                            List<int> scoreList = new List<int>();
                            for (int i = 0; i < room.chairList.Count; ++i)
                            {
                                room.chairList[i].score = room.chairList[i].peer.player.score;
                                scoreList.Add(room.chairList[i].score);
                            }
                            // 5. 更新玩家是否胡牌
                            if(!shangHuaType.Equals(ShangHuaType.bushanghua)) chair.isJoin = false;
                            // 杠上开花事件返回数据:1.杠上开花类型 2.杠上开花玩家Id 3.杠上开花摸牌数字
                            // 4.杠上开花玩家的手牌 5.每个玩家的分数
                            Dictionary<short, object> dictShangHua= new Dictionary<short, object>();
                            dictShangHua.Add(ParameterCode.shangHuaType, shangHuaType);
                            dictShangHua.Add(ParameterCode.shangHuaId, shangHuaId);
                            dictShangHua.Add(ParameterCode.shangHuaNum, shangHuaNum);
                            dictShangHua.Add(ParameterCode.handTile, shangHuaHandTile);
                            dictShangHua.Add(ParameterCode.scoreList, scoreList);
                            dictShangHua.Add(ParameterCode.roomInfo, room.roomInfo);
                            // 向所有人发送杠上开花事件
                            SendEventToAll((short)EventCode.SendShangHuaDataToAll, dictShangHua);
                            // 调用OnGameOver并更新游戏状态
                            room.OnGameOver();
                        }
                        else if(dict.ContainsKey(ParameterCode.PengGangType) && dict[ParameterCode.PengGangType] != null)
                        {
                            Console.WriteLine(player.username + "暗杠加杠数字" + chair.drawTileNum);
                            // 1. 获取暗杠加杠类型，暗杠加杠Id，暗杠加杠数字
                            PengGangType anGangJiaGangType = Convert<PengGangType>(dict[ParameterCode.PengGangType]);
                            int anGangJiaGangId = player.actorId;
                            int anGangJiaGangNum = chair.drawTileNum;
                            // 将杠牌添加进手牌
                            if(chair.drawTileNum != 0)
                            {
                                chair.handTile.Add(chair.drawTileNum);
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
                                Dictionary<short, object> dictAnGangJiaGang = new Dictionary<short, object>();
                                dictAnGangJiaGang.Add(ParameterCode.anGangJiaGangNum, anGangJiaGangNum);
                                dictAnGangJiaGang.Add(ParameterCode.anGangJiaGangId, anGangJiaGangId);
                                dictAnGangJiaGang.Add(ParameterCode.anGangJiaGangType, anGangJiaGangType);
                                SendEventToAll((short)EventCode.SendAnGangJiaGangDataToAll, dictAnGangJiaGang);

                                // 更新房间是否杠后出牌，暗杠玩家摸牌
                                room.roomInfo.isGangHouPlayTile = true;
                                for(int i = 0; i < room.chairList.Count; ++i)
                                {
                                    if(room.chairList[i].peer != null && room.chairList[i].peer.IsConnected)
                                    {
                                        if(room.chairList[i].peer.player.actorId == anGangJiaGangId)
                                        {
                                            room.chairList[i].peer.SendEvent((short)EventCode.SendToNextDrawTile, null);
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
                                dictJiaGang.Add(ParameterCode.anGangJiaGangNum, anGangJiaGangNum);
                                dictJiaGang.Add(ParameterCode.anGangJiaGangId, anGangJiaGangId);
                                dictJiaGang.Add(ParameterCode.anGangJiaGangType, anGangJiaGangType);
                                SendEventToAll((short)EventCode.SendAnGangJiaGangDataToAll, dictJiaGang);
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
                                        if(room.chairList[i].peer.player.actorId != anGangJiaGangId)
                                        {
                                            Dictionary<short, object> dictQiangGang = new Dictionary<short, object>();
                                            OtherPlayTileType otherPlayTileType = OtherPlayTileType.buhu;
                                            if(chair.isJoin)
                                            {
                                                otherPlayTileType = Check.OtherTileHuTileType(chair, anGangJiaGangNum, false, true);
                                            }
                                            dictQiangGang.Add(ParameterCode.otherPlayTileType, otherPlayTileType);
                                            room.chairList[i].peer.SendEvent((short)EventCode.SendQiangGangDataToOther, dictQiangGang);
                                        }
                                    }
                                }
                                // 给所有玩家发送开启10s倒计时的事件

                                // 关闭房间内需要出牌计时

                                // 开启房间内抢杠计时
                            }
                        }
                        #endregion
                    }
                    break;
                case OpCode.QiangGangDataReturn:
                    {
                        // 将抢杠数据添加进房间的抢杠数据列表
                        OtherPlayTileType otherPlayTileType = Convert<OtherPlayTileType>(dict[ParameterCode.otherPlayTileType]);
                        if(otherPlayTileType != OtherPlayTileType.buhu)
                        {
                            QiangGangReturnData qiangGangReturnData = new QiangGangReturnData
                            { 
                                qiangGangActorId = player.actorId,
                                qiangGangHuTileType = otherPlayTileType
                            };
                            room.roomInfo.qiangGangReturnDatas.Add(qiangGangReturnData);
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
                    }
                    break;
                case OpCode.GoOn:
                    {
                        player.isReady = false;
                        player.score = 0;
                        chair.score = 0;
                        chair.handTile.Clear();
                        chair.isJoin = true;
                        chair.handTile = new List<int>();
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

        public T Convert<T>(object o)
        {
            string str = JsonConvert.SerializeObject(o);
            T t = JsonConvert.DeserializeObject<T>(str);
            return t;
        }
    }
}
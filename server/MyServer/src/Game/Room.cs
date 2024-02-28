using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.src.Game
{
    class RoomInfo
    {
        public int roomName = 0;
        public uint remainTile=0;
        public int AllActorCounts = 0;
        public int masterClientID = 0;
        /// <summary>
        /// 房间此时的出牌数字
        /// </summary>
        public int playTileNum = 0;
        /// <summary>
        /// 房间此时的出牌玩家Id
        /// </summary>
        public int playTileId = 0;
        public List<SelectTileData> selectTileReturnDatas = new List<SelectTileData>();
        /// <summary>
        /// 房间四个玩家胡碰杠吃的数据
        /// </summary>
        public List<HuTileReturnData> huTileReturnDatas = new List<HuTileReturnData>();
        public List<PengGangReturnData> pengGangReturnDatas = new List<PengGangReturnData>();
        public bool isExcutedPlayTileDataReturn = false;
        /// <summary>
        /// 房间类型
        /// </summary>
        public RoomType roomType = RoomType.SiChuan;
        /// <summary>
        /// 房间是否杠后出牌
        /// </summary>
        public bool isGangHouPlayTile = false;
        /// <summary>
        /// 房间加杠玩家的Id
        /// </summary>
        public int JiaGangId = 0;
        /// <summary>
        /// 房间四个玩家抢杠胡的数据是否全部返回
        /// </summary>
        public List<HuTileReturnData> qiangGangReturnDatas = new List<HuTileReturnData>();
        public bool isExcutedQiangGangDataReturn = false;
    }

    public class CPGProperties
    {
        public PengGangType cpgType;
        public List<int> cpgList;
    }

    class Chair
    {
        public Chair(PlayerPosition pos) { playerPos = pos; }
        public bool isJoin = false;
        public PlayerPosition playerPos = PlayerPosition.east;
        public Peer peer = null;
        public List<ushort> handTile = new List<ushort>();
        public TileType queTileType=TileType.none;
        // 摸牌数字
        public int drawTileNum = 0;
        public List<int> throwTile = new List<int>();
        public List<CPGProperties> cpg = new List<CPGProperties>();
        public bool isSelectTileReturn = false;
        public bool isDingQueReturn = false;
        public bool isPlayTileReturn = false;
        public int score = 0;
        public bool isQiangGangReturn = false;
    }


    class Room
    {
        public RoomInfo roomInfo = new RoomInfo();
        public List<Chair> chairList = new List<Chair>()
        {
            new Chair(PlayerPosition.east),
            new Chair(PlayerPosition.south),
            new Chair(PlayerPosition.west),
            new Chair(PlayerPosition.north)
        };
        public Queue<int> Tiles = new Queue<int>(136);

        /// <summary>
        /// 获取牌的类型
        /// </summary>
        /// <param name="num">牌的数字</param>
        /// <returns></returns>
        public TileTypeTileNum GetTypeTileNum(int num)
        {
            int left = 1, right = 4, start = (int)TileTypeTileNum.tiao_1, end;
            for (int i = 1; i <= 10; ++i)
            {
                if (i <= 3) end = start + 9;
                else end = start + 1;
                for (; start < end; ++start)
                {
                    if (left <= num && num <= right) return (TileTypeTileNum)start;
                    left += 4; right += 4;
                }
                start = end + 11;
            }
            return TileTypeTileNum.bai;
        }

        public void InitTiles()
        {
            Tiles.Clear();
            // 1. 获取1到108不重复的108个随机数
            Random ran = new Random();
            Queue<int> tile = new Queue<int>(108);
            while (true)
            {
                int num = ran.Next(1, 109);
                if (!tile.Contains(num)) tile.Enqueue(num);
                if (tile.Count == 108) break;
            }
            // 2. 获取对应的牌型数字并且添加到房间的牌堆
            int count = tile.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    int num = tile.Dequeue();
                    Tiles.Enqueue(GetTypeTileNum(num).GetHashCode());
                }
            }
        }

        public void OnSelectTileDataReturn()
        {

            // 1. 先删除3张牌，再添加3张牌
            Random random = new Random();
            int ran = random.Next(0, 3);
            for (int i = 0; i < roomInfo.selectTileReturnDatas.Count; ++i)
            {
                for (int j = 0; j < chairList.Count; ++j)
                {
                    if (chairList[j].peer != null && chairList[j].peer.IsConnected)
                    {
                        SelectTileData selectTileData = roomInfo.selectTileReturnDatas[i];
                        if (selectTileData.selectTileId == chairList[j].peer.playerData1.playerId)
                        {
                            for (int k = 0; k < 3; ++k)
                                chairList[j].handTile.Remove(selectTileData.selectTiles[k]);
                            Dictionary<short, object> dictSelectTile = new Dictionary<short, object>();
                            dictSelectTile.Add(ParameterCode.SelectTileData, selectTileData);
                            int sendEventIndex = 0;
                            if(ran == 0)
                                sendEventIndex = (j + 3) % 4;
                            else if(ran == 1)
                                sendEventIndex = (j + 1) % 4;
                            else if(ran == 2)
                                sendEventIndex = (j + 2) % 4;
                            chairList[sendEventIndex].peer.SendEvent(ServerToClient.SendSelectTileToAll, dictSelectTile);
                            for (int k = 0; k < 3; ++k)
                                chairList[sendEventIndex].handTile.Add(roomInfo.selectTileReturnDatas[i].selectTiles[k]);

                            break;
                        }
                    }
                    else
                    {

                    }
                }
            }
            Console.WriteLine("选完牌后四个玩家的手牌");
            for (int i = 0; i < chairList.Count; ++i)
            {
                Console.WriteLine(chairList[i].peer.playerData1.playerName);
                for (int j = 0; j < chairList[i].handTile.Count; ++j)
                {
                    Console.Write(chairList[i].handTile[j] + " ");
                }
            }
            roomInfo.selectTileReturnDatas.Clear();
        }
        public void OnPlayTileDataReturn()
        {
            if (roomInfo.isExcutedPlayTileDataReturn == false)
            {
                roomInfo.isExcutedPlayTileDataReturn = true;
                #region
                // 优先级： 胡>杠>碰>吃>不胡不杠不碰不吃
                // 胡牌发送信息：1.出牌数字 2. 出牌玩家Id 3.胡牌玩家的数据 4. 胡牌玩家的手牌 5.每个玩家的分数
                // 碰杠牌发送信息：1. 出牌数字 2. 出牌玩家Id 3.碰杠玩家Id 4.每个玩家的分数
                // 不胡不碰不杠不吃牌发送信息： 

                Dictionary<short, object> dictEv = new Dictionary<short, object>();
                if (roomInfo.huTileReturnDatas.Count > 0)
                {
                    // 1. 更新房间是否杠后出牌
                    roomInfo.isGangHouPlayTile = false;
                    // 2. 更新胡牌返回数据
                    List<HuTileReturnData> huTileReturnDatas = roomInfo.huTileReturnDatas;
                    Settle settle = new Settle();
                    int lostScore = 0;
                    for (int i = 0; i < huTileReturnDatas.Count; ++i)
                    {
                        HuTileReturnData data = huTileReturnDatas[i];
                        ushort huTileId = data.huTileId;
                        int tempScore = settle.GetHuScore(roomInfo.roomType, (HuTileType)data.huTileType, 1);
                        lostScore += tempScore;
                        for (int j = 0; j < chairList.Count; ++j)
                        {
                            if (chairList[j].peer != null && chairList[j].peer.IsConnected)
                            {
                                if (chairList[j].peer.playerData1.playerId == roomInfo.playTileId)
                                {
                                    chairList[j].peer.playerData1.score -= (uint)lostScore;
                                    data.score = (ushort)chairList[j].peer.playerData1.score;
                                    break;
                                }
                            }
                            else
                            {

                            }
                        }
                        
                        for (int j = 0; j < chairList.Count; ++j)
                        {
                            if (chairList[j].peer.playerData1.playerId == huTileId)
                            {
                                chairList[j].peer.playerData1.score += (uint)tempScore;
                                chairList[j].isJoin = false;
                                List<ushort> handTile = new List<ushort>(chairList[j].handTile);
                                handTile.Add((ushort)roomInfo.playTileNum);
                                if (chairList[j].cpg.Count > 0)
                                {
                                    foreach (var item in chairList[j].cpg)
                                    {
                                        for (int k = 0; k < 3; ++k)
                                            handTile.Add((ushort)item.cpgList[k]);
                                    }
                                }
                                data.huTile = handTile.ToArray();
                                data.huTileCnt = (ushort)handTile.Count;
                                data.score = (ushort)chairList[j].peer.playerData1.score;

                                break;
                            }
                        }
                        huTileReturnDatas[i] = data;
                    }
          
     
                    // 3.给所有玩家发送胡牌返回数据
                    dictEv.Add(ParameterCode.HuTileReturnData, huTileReturnDatas);
                    for (int i = 0; i < chairList.Count; i++)
                    {
                        if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                        {
                            chairList[i].peer.SendEvent(ServerToClient.SendHuTileDataToAll, dictEv);
                        }
                        else
                        {

                        }
                    }
                    // 4.判断是否游戏结束
                    OnGameOver();
                    // 5.更新游戏状态
                }
                else if (roomInfo.pengGangReturnDatas.Count > 0)
                {
                    // 1.解析数据
                    PengGangReturnData pengGangReturnData = roomInfo.pengGangReturnDatas[0];
                    PengGangType pengGangType = pengGangReturnData.pengGangType;
                    bool isPeng = pengGangType.Equals(PengGangType.peng);
                    // 2. 更新房间信息
                    roomInfo.isGangHouPlayTile = !isPeng;
                    dictEv.Add(ParameterCode.PengGangReturnData, pengGangReturnData);
                    for (int i = 0; i < chairList.Count; ++i)
                    {
                        if (chairList[i].peer == null || chairList[i].peer.IsConnected == false)
                        { 
                            continue;
                        }
                        // 3. 给所有玩家发送碰杠牌数据
                        chairList[i].peer.SendEvent(ServerToClient.SendPengGangTileDataToAll, dictEv);
                        // 4. 更新房间内的吃碰杠牌与手牌, 给杠牌玩家发送摸牌的事件
                        if (chairList[i].peer.playerData1.playerId == pengGangReturnData.pengGangId)
                        {
                            CPGProperties cpg = new CPGProperties();
                            cpg.cpgType = pengGangType;
                            cpg.cpgList = new List<int>();
                            int addTileNum = isPeng ? 3 : 4;
                            for (int j = 0; j < addTileNum; ++j)
                                cpg.cpgList.Add(pengGangReturnData.pengGangTileNum);
                            chairList[i].cpg.Add(cpg);
                            for (int j = 0; j < addTileNum - 1; ++j)
                            {
                                chairList[i].handTile.Remove(pengGangReturnData.pengGangTileNum);
                            }
                            if (!isPeng)
                                chairList[i].peer.SendEvent(ServerToClient.SendToNextDrawTile, null);
                        }
                        // 5. 当有人碰牌时, 给所有玩家发送出牌计时
                        if(isPeng)
                        {
                            TimerData timerData = new TimerData();
                            timerData.timerId = pengGangReturnData.pengGangId;
                            timerData.isPlay = true;
                            Dictionary<short, object> dictTimer = new Dictionary<short, object>()
                            { {ParameterCode.TimerData, timerData } };
                            chairList[i].peer.SendEvent(ServerToClient.SendTimerToAll, dictTimer);
                            continue;
                        }
                       
                    }
                   
                }
                else
                {
                    // 1.更新房间是否杠后出牌
                    roomInfo.isGangHouPlayTile = false;
                    // 2.给下一个未胡牌玩家发送摸牌事件
                    int nextDrawTileId = GetNextDrawTileId();
                    for (int i = 0; i < chairList.Count; ++i)
                    {
                        if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                        {
                            if (chairList[i].peer.playerData1.playerId == nextDrawTileId)
                            {
                                chairList[i].peer.SendEvent(ServerToClient.SendToNextDrawTile, null);
                                break;
                            }
                            // 下一个摸牌的玩家掉线了
                            else
                            {

                            }
                        }
                        else
                        {

                        }
                    }
                }
                #endregion
            }


            roomInfo.huTileReturnDatas.Clear();
            roomInfo.pengGangReturnDatas.Clear();
        }
        public void OnQiangGangDataReturn()
        {
            if (roomInfo.isExcutedQiangGangDataReturn == false)
            {
                roomInfo.isExcutedQiangGangDataReturn = true;
                #region

                if (roomInfo.qiangGangReturnDatas.Count > 0)
                {
                    // 1.更新房间是否杠后出牌
                    roomInfo.isGangHouPlayTile = false;
                    // 2. 更新每个玩家的分数, 是否胡牌, 获取胡牌数据中的胡牌玩家手牌
                    Settle settle = new Settle();
                    int lostScore = 0;
                    List<HuTileReturnData> qiangGangReturnDatas = roomInfo.qiangGangReturnDatas;
                    for (int i = 0; i < roomInfo.qiangGangReturnDatas.Count; ++i)
                    {
                        HuTileReturnData qiangGangReturnData = qiangGangReturnDatas[i];
                        int tempScore = settle.GetHuScore(roomInfo.roomType, (HuTileType)qiangGangReturnData.huTileType, 1);
                        lostScore += tempScore;
                        for (int j = 0; j < chairList.Count; ++j)
                        {
                            if (chairList[j].peer != null && chairList[j].peer.IsConnected)
                            {
                                if (chairList[j].peer.playerData1.playerId == roomInfo.playTileId)
                                {
                                    chairList[j].peer.playerData1.score -= (uint)lostScore;
                                    qiangGangReturnData.score = (ushort)chairList[j].peer.playerData1.score;
                                    break;
                                }
                            }
                            else
                            {

                            }
                        }


                        for (int j = 0; j < chairList.Count; ++j)
                        {
                            if (chairList[j].peer != null && chairList[j].peer.IsConnected)
                            {
                                if (chairList[j].peer.playerData1.playerId == qiangGangReturnData.huTileId)
                                {
                                    chairList[j].peer.playerData1.score += (uint)tempScore;
                                    chairList[j].isJoin = false;
                                    List<ushort> handTile = new List<ushort>(chairList[j].handTile);
                                    handTile.Add((ushort)roomInfo.playTileNum);
                                    if (chairList[j].cpg.Count > 0)
                                    {
                                        foreach (var item in chairList[j].cpg)
                                        {
                                            for (int k = 0; k < 3; ++k)
                                                handTile.Add((ushort)item.cpgList[k]);
                                        }
                                    }
                                    qiangGangReturnData.score = (ushort)chairList[j].peer.playerData1.score;
                                    qiangGangReturnData.huTile = handTile.ToArray();
                                    qiangGangReturnData.huTileCnt = (ushort)handTile.Count;
                                    break;
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                    // 3. 服务器发送抢杠胡事件给所有玩家：
                    Dictionary<short, object> dictQiangGang = new Dictionary<short, object>();
                    dictQiangGang.Add(ParameterCode.HuTileReturnData, qiangGangReturnDatas);
                    for (int i = 0; i < chairList.Count; i++)
                    {
                        if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                        {
                            chairList[i].peer.SendEvent(ServerToClient.SendQiangGangHuDataToAll, dictQiangGang);
                        }
                        else
                        {

                        }
                    }
                    // 判断是否游戏结束并更新游戏状态
                    OnGameOver();
                }
                else
                {
                    // 1.更新房间是否杠后出牌
                    roomInfo.isGangHouPlayTile = true;
                    // 2.没有人抢杠，加杠玩家摸牌
                    for (int i = 0; i < chairList.Count; ++i)
                    {
                        if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                        {
                            if (chairList[i].peer.playerData1.playerId == roomInfo.JiaGangId)
                            {
                                chairList[i].peer.SendEvent(ServerToClient.SendToNextDrawTile, null);
                                break;
                            }
                            // 下一个摸牌的玩家掉线了
                            else
                            {

                            }
                            break;
                        }
                        else
                        {

                        }
                    }
                }
                roomInfo.qiangGangReturnDatas.Clear();
                #endregion
            }

            roomInfo.qiangGangReturnDatas.Clear();
        }

        /// <summary>
        /// 如果有三个玩家已经胡牌或者黄牌，则向所有玩家宣布游戏结束，否则让下一个未胡牌玩家摸牌
        /// </summary>
        public void OnGameOver()
        {
            // 获取胡牌玩家个数和未胡牌玩家的手牌, Id
            int huPlayerNum = 0;
            List<HuTileReturnData> huTileReturnDatas = new List<HuTileReturnData>();
            for (int i = 0; i < chairList.Count; i++)
            {
                if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                {
                    if (chairList[i].isJoin == false) 
                        ++huPlayerNum;
                    else
                    {
                        HuTileReturnData huTileReturnData = new HuTileReturnData();
                        huTileReturnData.huTileId = chairList[i].peer.playerData1.playerId;
                        huTileReturnData.huTileType = (ushort)HuTileType.buhu;
                        huTileReturnData.score = (ushort)chairList[i].score;
                        List<ushort> handTile = new List<ushort>(chairList[i].handTile);
                        handTile.Add((ushort)roomInfo.playTileNum);
                        if (chairList[i].cpg.Count > 0)
                        {
                            foreach (var item in chairList[i].cpg)
                            {
                                for (int k = 0; k < 3; ++k)
                                    handTile.Add((ushort)item.cpgList[k]);
                            }
                        }
                        huTileReturnData.huTile = handTile.ToArray();
                        huTileReturnData.huTileCnt = (ushort)handTile.Count;
                        huTileReturnDatas.Add(huTileReturnData);
                    }
                }
                else
                {

                }
            }

            if (huPlayerNum == 3 || Tiles.Count == 0)
            {
                // 发送信息：1. 未胡牌玩家的手牌 2. 未胡牌玩家的Id 3. 每个玩家的分数
                Dictionary<short, object> dictGameOver = new Dictionary<short, object>();
                List<int> scoreList = new List<int>();
                dictGameOver.Add(ParameterCode.HuTileReturnData, huTileReturnDatas);
                for (int i = 0; i < chairList.Count; ++i)
                {
                    if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                    {
                        if (chairList[i].isJoin) chairList[i].isJoin = false;
                        chairList[i].peer.SendEvent(ServerToClient.SendGameOverToAll, dictGameOver);
                    }
                }
            }
            else
            {
                // 获取下一个出牌玩家的Id
                int nextDrawTileId = GetNextDrawTileId();
                for (int i = 0; i < chairList.Count; ++i)
                {
                    if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                    {
                        if (chairList[i].peer.playerData1.playerId == nextDrawTileId)
                        {
                            chairList[i].peer.SendEvent(ServerToClient.SendToNextDrawTile, null);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取下一个未胡牌玩家的Id
        /// </summary>
        /// <returns></returns>
        public int GetNextDrawTileId()
        {
            List<int> playerIdList = new List<int>();
            for (int i = 1; i <= 3; ++i)
            {
                if (roomInfo.playTileId + i > 4) playerIdList.Add(roomInfo.playTileId + i - 4);
                else playerIdList.Add(roomInfo.playTileId + i);
            }
            for (int i = 0; i < playerIdList.Count; ++i)
            {
                for (int j = 0; j < chairList.Count; ++j)
                {
                    if (chairList[j].peer != null && chairList[j].peer.IsConnected)
                    {
                        if (chairList[j].peer.playerData1.playerId == playerIdList[i] && chairList[j].isJoin)
                        {
                            return playerIdList[i];
                        }
                    }
                }
            }
            return 0;
        }
    }
}

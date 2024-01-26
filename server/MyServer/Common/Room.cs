using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer
{
    public class SelectTileReturnData
    {
        public int selectTileId;
        public List<int>selectTileList = new List<int>();
    }

    public class OtherPlayTileReturnData
    {
        public int otherPlayTileActorId = 0;
        public OtherPlayTileType otherPlayTileType = OtherPlayTileType.buhu;
    }

    public class PengGangReturnData
    {
        public int pengGangActorId = 0;
        public PengGangType PengGangType = PengGangType.none;
    }

    public class ChiReturnData
    {
        public int chiTileActorID = 0;
        public List<int> chiList = new List<int>();
    }

    public class QiangGangReturnData
    {
        public int qiangGangActorId = 0;
        public OtherPlayTileType qiangGangHuTileType = OtherPlayTileType.buhu;
    }


    public class RoomInfo
    {
        public int roomName = 0;
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
        public List<SelectTileReturnData> selectTileReturnDatas = new List<SelectTileReturnData>();
        /// <summary>
        /// 房间四个玩家胡碰杠吃的数据
        /// </summary>
        public List<OtherPlayTileReturnData> otherPlayTileReturnDatas = new List<OtherPlayTileReturnData>();
        public List<PengGangReturnData> pengGangReturnDatas = new List<PengGangReturnData>();
        public List<ChiReturnData> chiReturnDatas = new List<ChiReturnData>();
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
        public List<QiangGangReturnData> qiangGangReturnDatas = new List<QiangGangReturnData>();
        public bool isExcutedQiangGangDataReturn = false;
    }

    public class CPGProperties
    {
        public PengGangType cpgType;
        public List<int>cpgList;
    }

    public class Chair
    {
        public Chair(PlayerPosition pos) { playerPos = pos;}
        public bool isJoin = false;
        public PlayerPosition playerPos = PlayerPosition.east;
        public Peer peer = null;
        public List<int>handTile = new List<int>();
        public TileType queTileType;
        // 摸牌数字
        public int drawTileNum = 0; 
        public List<int>throwTile = new List<int>();
        public List<CPGProperties> cpg = new List<CPGProperties>();
        public bool isSelectTileReturn = false;
        public bool isDingQueReturn = false;
        public bool isPlayTileReturn = false;
        public int score = 0;
        public bool isQiangGangReturn = false;
    }
    

    public class Room
    {
        public RoomInfo roomInfo = new RoomInfo();
        public List<Chair> chairList = new List<Chair>()
        {
            new Chair(PlayerPosition.east),
            new Chair(PlayerPosition.south),
            new Chair(PlayerPosition.west),
            new Chair(PlayerPosition.north)
        };
        public Queue<int>Tiles = new Queue<int>(136);

        /// <summary>
        /// 获取牌的类型
        /// </summary>
        /// <param name="num">牌的数字</param>
        /// <returns></returns>
        public TileTypeTileNum GetTypeTileNum(int num)
        {
            int left = 1, right = 4, start = (int)TileTypeTileNum.bamboo1, end; 
            for(int i = 1; i <= 10; ++i)
            {
                if(i <= 3) end = start + 9;
                else       end = start + 1;
                for (; start < end; ++start)
                {
                    if (left <= num && num <= right) return (TileTypeTileNum)start;
                    left += 4; right += 4;
                }
                start = end + 11;
            }
            return TileTypeTileNum.WhiteDragon;
        }

        public void InitTiles()
        {
            Tiles.Clear();
            // 1. 获取1到108不重复的108个随机数
            Random ran = new Random();
            Queue<int>tile = new Queue<int>(108);
            while(true)
            {
                int num = ran.Next(1, 109);
                if(!tile.Contains(num))tile.Enqueue(num);
                if(tile.Count == 108) break;
            }
            // 2. 获取对应的牌型数字并且添加到房间的牌堆
            int count = tile.Count;
            if(count > 0)
            {
                for(int i = 0; i < count; ++i)
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
                for(int j = 0; j < chairList.Count; ++j)
                {
                    if(chairList[j].peer != null && chairList[j].peer.IsConnected)
                    {
                        if(roomInfo.selectTileReturnDatas[i].selectTileId == chairList[j].peer.player.actorId)
                        {
                            for(int k = 0; k < 3; ++k)
                            {
                                chairList[j].handTile.Remove(roomInfo.selectTileReturnDatas[i].selectTileList[k]);
                            }
                            Dictionary<short, object> dictSelectTile = new Dictionary<short, object>();
                            if(ran == 0)
                            {
                                dictSelectTile.Add(ParameterCode.selectTileList, roomInfo.selectTileReturnDatas[i].selectTileList);
                                chairList[(j + 3) % 4].peer.SendEvent((short)EventCode.SendSelectTileToAll, dictSelectTile);
                                for (int k = 0; k < 3; ++k)
                                {
                                    chairList[(j + 3) % 4].handTile.Add(roomInfo.selectTileReturnDatas[i].selectTileList[k]);
                                }
                            }
                            else if(ran == 1)
                            {
                                dictSelectTile.Add(ParameterCode.selectTileList, roomInfo.selectTileReturnDatas[i].selectTileList);
                                chairList[(j + 1) % 4].peer.SendEvent((short)EventCode.SendSelectTileToAll, dictSelectTile);
                                for (int k = 0; k < 3; ++k)
                                {
                                    chairList[(j + 1) % 4].handTile.Add(roomInfo.selectTileReturnDatas[i].selectTileList[k]);
                                }
                            }
                            else if(ran == 2)
                            {
                                dictSelectTile.Add(ParameterCode.selectTileList, roomInfo.selectTileReturnDatas[i].selectTileList);
                                chairList[(j + 2) % 4].peer.SendEvent((short)EventCode.SendSelectTileToAll, dictSelectTile);
                                for (int k = 0; k < 3; ++k)
                                {
                                    chairList[(j + 2) % 4].handTile.Add(roomInfo.selectTileReturnDatas[i].selectTileList[k]);
                                }
                            }
                            break;
                        }
                    }
                    else
                    {

                    }
                }
            }
            Console.WriteLine("选完牌后四个玩家的手牌");
            for(int i = 0; i < chairList.Count; ++i)
            {
                Console.WriteLine(chairList[i].peer.player.username);
                for(int j = 0; j < chairList[i].handTile.Count; ++j)
                {
                    Console.Write(chairList[i].handTile[j] + " ");
                }
            }
            roomInfo.selectTileReturnDatas.Clear();
        }
        
        /// <summary>
        /// 处理四个玩家胡碰杠吃返回的数据
        /// </summary>
        public void OnPlayTileDataReturn()
        {
            if(roomInfo.isExcutedPlayTileDataReturn == false)
            {
                roomInfo.isExcutedPlayTileDataReturn = true;
                #region
                // 优先级： 胡>杠>碰>吃>不胡不杠不碰不吃
                // 胡牌发送信息：1.出牌数字 2. 出牌玩家Id 3.胡牌玩家的数据 4. 胡牌玩家的手牌 5.每个玩家的分数
                // 碰杠牌发送信息：1. 出牌数字 2. 出牌玩家Id 3.碰杠玩家Id 4.每个玩家的分数
                // 不胡不碰不杠不吃牌发送信息： 
                
                Dictionary<short, object> dictEv = new Dictionary<short, object>();
                dictEv.Add(ParameterCode.playTileNum, roomInfo.playTileNum);
                dictEv.Add(ParameterCode.playTileId, roomInfo.playTileId);
                if (roomInfo.otherPlayTileReturnDatas.Count > 0)
                {
                    // 1. 更新房间是否杠后出牌
                    roomInfo.isGangHouPlayTile = false;
                    // 2. 更新每个玩家的分数, 是否胡牌, 获取胡牌数据中的胡牌玩家手牌
                    Settle settle = new Settle();
                    int lostScore = 0;
                    List<List<int>> huHandTileList = new List<List<int>>();
                    for (int i = 0; i < roomInfo.otherPlayTileReturnDatas.Count; ++i)
                    {
                        int tempScore =
                            settle.GetHuScore(roomInfo.roomType, roomInfo.otherPlayTileReturnDatas[i].otherPlayTileType, 1);
                        lostScore += tempScore;
                        int otherPlayTileActorId = roomInfo.otherPlayTileReturnDatas[i].otherPlayTileActorId;
                        for (int j = 0; j < chairList.Count; ++j)
                        {
                            if (chairList[j].peer.player.actorId == otherPlayTileActorId)
                            {
                                chairList[i].peer.player.score += tempScore;
                                chairList[j].isJoin = false;
                                List<int>handTile = new List<int>(chairList[j].handTile);
                                handTile.Add(roomInfo.playTileNum);
                                if (chairList[j].cpg.Count > 0)
                                {
                                    foreach (var item in chairList[j].cpg)
                                    {
                                        for (int k = 0; k < 3; ++k)
                                            handTile.Add(item.cpgList[k]);
                                    }
                                }
                                huHandTileList.Add(handTile);
                                break;
                            }
                        }
                    }
                    for (int i = 0; i < chairList.Count; ++i)
                    {
                        if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                        {
                            if (chairList[i].peer.player.actorId == roomInfo.playTileId)
                            {
                                chairList[i].peer.player.score -= lostScore;
                                break;
                            }
                        }
                        else
                        {

                        }
                    }
                    List<int> scoreList = new List<int>();
                    for (int i = 0; i < chairList.Count; ++i)
                    {
                        chairList[i].score = chairList[i].peer.player.score;
                        scoreList.Add(chairList[i].score);
                    }
                    // 3.给所有玩家发送胡牌返回数据
                    dictEv.Add(ParameterCode.otherPlayTileDataList, roomInfo.otherPlayTileReturnDatas);
                    dictEv.Add(ParameterCode.handTileList, huHandTileList);
                    dictEv.Add(ParameterCode.scoreList, scoreList);
                    for (int i = 0; i < chairList.Count; i++)
                    {
                        if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                        {
                            chairList[i].peer.SendEvent((short)EventCode.SendOtherPlayTileDataToAll, dictEv);
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
                    // 1.给所有玩家发送碰杠牌数据
                    dictEv.Add(ParameterCode.pengGangId, roomInfo.pengGangReturnDatas[0].pengGangActorId);
                    dictEv.Add(ParameterCode.PengGangType, roomInfo.pengGangReturnDatas[0].PengGangType);
                    for (int i = 0; i < chairList.Count; i++)
                    {
                        if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                        {
                            chairList[i].peer.SendEvent((short)EventCode.SendPengGangTileDataToAll, dictEv);
                        }
                        else
                        {

                        }
                    }
                    // 2.更新吃碰杠牌与手牌,并给杠牌玩家发送摸牌事件
                    int pengGangId = roomInfo.pengGangReturnDatas[0].pengGangActorId;
                    PengGangType PengGangType = roomInfo.pengGangReturnDatas[0].PengGangType;
                    if (PengGangType == PengGangType.peng)
                    {
                        // 更新碰牌玩家的吃碰杠牌与手牌
                        for (int i = 0; i < chairList.Count; ++i)
                        {
                            if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                            {
                                if (chairList[i].peer.player.actorId == pengGangId)
                                {
                                    chairList[i].cpg.Add(new CPGProperties()
                                    {
                                        cpgType = PengGangType.peng,
                                        cpgList = new List<int>() { roomInfo.playTileNum, roomInfo.playTileNum, roomInfo.playTileNum }
                                    });
                                    // 从碰牌玩家的手牌中删除两张playTileNum
                                    for (int j = 0; j < 2; ++j)
                                    {
                                        chairList[i].handTile.Remove(roomInfo.playTileNum);
                                    }
                                    break;
                                }
                            }
                            else
                            {

                            }
                        }
                        // 更新房间信息
                        roomInfo.isGangHouPlayTile = false;
                        // 开启房间出牌倒计时
                    }
                    else if (PengGangType == PengGangType.gang)
                    {
                        // 更新杠牌玩家的吃碰杠牌与手牌
                        for (int i = 0; i < chairList.Count; ++i)
                        {
                            if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                            {
                                if (chairList[i].peer.player.actorId == pengGangId)
                                {
                                    chairList[i].cpg.Add(new CPGProperties()
                                    {
                                        cpgType = PengGangType.gang,
                                        cpgList = new List<int>()
                                    { roomInfo.playTileNum, roomInfo.playTileNum, roomInfo.playTileNum, roomInfo.playTileNum }
                                    });
                                    // 从杠牌玩家的手牌中删除三张playTileNum
                                    for (int j = 0; j < 3; ++j)
                                    {
                                        chairList[i].handTile.Remove(roomInfo.playTileNum);
                                    }
                                    break;
                                }
                            }
                            else
                            {

                            }
                        }
                        // 更新房间信息
                        roomInfo.isGangHouPlayTile = true;
                        int nextdrawTileId = pengGangId;
                        // 给杠牌玩家发送摸牌的事件
                        for (int i = 0; i < chairList.Count; ++i)
                        {
                            if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                            {
                                if (chairList[i].peer.player.actorId == nextdrawTileId)
                                {
                                    chairList[i].peer.SendEvent((short)EventCode.SendToNextDrawTile, null);
                                    break;
                                }
                                // 下一个摸牌的玩家掉线了
                                else
                                {

                                }
                            }
                        }
                    }
                }
                else
                {
                    // 1.更新房间是否杠后出牌
                    roomInfo.isGangHouPlayTile = false;
                    // 2.给下一个未胡牌玩家发送摸牌事件
                    int nextDrawTileId = GetNextDrawTileId();
                    Console.WriteLine(nextDrawTileId);
                    for (int i = 0; i < chairList.Count; ++i)
                    {
                        if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                        {
                            if (chairList[i].peer.player.actorId == nextDrawTileId)
                            {
                                chairList[i].peer.SendEvent((short)EventCode.SendToNextDrawTile, null);
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


            roomInfo.otherPlayTileReturnDatas.Clear();
            roomInfo.pengGangReturnDatas.Clear();
            roomInfo.chiReturnDatas.Clear();
        }

        /// <summary>
        /// 处理四个玩家抢杠返回的数据
        /// </summary>
        public void OnQiangGangDataReturn()
        {
            if(roomInfo.isExcutedQiangGangDataReturn == false)
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
                    List<List<int>>huHandTileList = new List<List<int>>();
                    for (int i = 0; i < roomInfo.qiangGangReturnDatas.Count; ++i)
                    {
                        int tempScore =
                            settle.GetHuScore(roomInfo.roomType, roomInfo.qiangGangReturnDatas[i].qiangGangHuTileType, 1);
                        lostScore += tempScore;
                        int qiangGangHuId = roomInfo.qiangGangReturnDatas[i].qiangGangActorId;
                        for (int j = 0; j < chairList.Count; ++j)
                        {
                            if (chairList[j].peer != null && chairList[j].peer.IsConnected)
                            {
                                if (chairList[j].peer.player.actorId == qiangGangHuId)
                                {
                                    chairList[j].peer.player.score += tempScore;
                                    chairList[j].isJoin = false;
                                    List<int> handTile = new List<int>(chairList[j].handTile);
                                    handTile.Add(roomInfo.playTileNum);
                                    if (chairList[j].cpg.Count > 0)
                                    {
                                        foreach (var item in chairList[j].cpg)
                                        {
                                            for (int k = 0; k < 3; ++k)
                                                handTile.Add(item.cpgList[k]);
                                        }
                                    }
                                    huHandTileList.Add(handTile);
                                    break;
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                    for (int i = 0; i < chairList.Count; ++i)
                    {
                        if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                        {
                            if (chairList[i].peer.player.actorId == roomInfo.playTileId)
                            {
                                chairList[i].peer.player.score -= lostScore;
                                break;
                            }
                        }
                        else
                        {

                        }
                    }
                    List<int> scoreList = new List<int>();
                    for (int i = 0; i < chairList.Count; ++i)
                    {
                        chairList[i].score = chairList[i].peer.player.score;
                        scoreList.Add(chairList[i].score);
                    }
                    // 服务器发送抢杠胡事件给所有玩家：
                    // 1.抢杠胡牌数据 3.抢杠胡牌玩家的手牌 4. 每个玩家的分数
                    Dictionary<short, object> dictQiangGang = new Dictionary<short, object>();
                    dictQiangGang.Add(ParameterCode.qiangGangHuDataList, roomInfo.qiangGangReturnDatas);
                    dictQiangGang.Add(ParameterCode.handTileList, huHandTileList);
                    dictQiangGang.Add(ParameterCode.scoreList, scoreList);
                    for (int i = 0; i < chairList.Count; i++)
                    {
                        if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                        {
                            chairList[i].peer.SendEvent((short)EventCode.SendQiangGangHuDataToAll, dictQiangGang);
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
                            if (chairList[i].peer.player.actorId == roomInfo.JiaGangId)
                            {
                                chairList[i].peer.SendEvent((short)EventCode.SendToNextDrawTile, null);
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
            List<List<int>> noHuHandTileList = new List<List<int>>();
            List<int> noHuPlayerIdList = new List<int>();
             for (int i = 0; i < chairList.Count; i++)
            {
                if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                {
                    if (chairList[i].isJoin == false) ++huPlayerNum;
                    else
                    {
                        List<int> handTile = new List<int>(chairList[i].handTile);
                        handTile.Add(roomInfo.playTileNum);
                        if (chairList[i].cpg.Count > 0)
                        {
                            foreach (var item in chairList[i].cpg)
                            {
                                for (int k = 0; k < 3; ++k)
                                    handTile.Add(item.cpgList[k]);
                            }
                        }
                        noHuHandTileList.Add(handTile);
                        noHuPlayerIdList.Add(chairList[i].peer.player.actorId);
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
                dictGameOver.Add(ParameterCode.handTileList, noHuHandTileList);
                dictGameOver.Add(ParameterCode.noHuPlayerIdList, noHuPlayerIdList);
                dictGameOver.Add(ParameterCode.scoreList, scoreList);
                for (int i = 0; i < chairList.Count; ++i)
                {
                    if (chairList[i].peer != null && chairList[i].peer.IsConnected)
                    {
                        if (chairList[i].isJoin) chairList[i].isJoin = false;
                        chairList[i].peer.SendEvent((short)EventCode.SendGameOverToAll, dictGameOver);
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
                        if (chairList[i].peer.player.actorId == nextDrawTileId)
                        {
                            chairList[i].peer.SendEvent((short)EventCode.SendToNextDrawTile, null);
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
            List<int>playerIdList = new List<int>();
            for(int i = 1; i <= 3; ++i)
            {
                if(roomInfo.playTileId + i > 4) playerIdList.Add(roomInfo.playTileId + i - 4);
                else                            playerIdList.Add(roomInfo.playTileId + i);
            }
            for(int i = 0; i < playerIdList.Count; ++i)
            {
                for(int j = 0; j < chairList.Count; ++j)
                {
                    if(chairList[j].peer != null && chairList[j].peer.IsConnected)
                    {
                        if(chairList[j].peer.player.actorId == playerIdList[i] && chairList[j].isJoin)
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

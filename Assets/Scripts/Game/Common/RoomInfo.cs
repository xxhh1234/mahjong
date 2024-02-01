using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XH
{
    public class SelectTileReturnData
    {
        public int selectTileId;
        public List<int> selectTileList = new List<int>();
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
        public bool isChow = true;
        public int currentNumber = 0;
        public int allNumber = 4;
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
        public Net playTilePeer = null;
        public List<SelectTileReturnData> selectTileReturnDatas = new List<SelectTileReturnData>();
        public bool isExcutedSelectTileDataReturn = false;
        /// <summary>
        /// 房间四个玩家胡碰杠吃的数据
        /// </summary>
        public List<OtherPlayTileReturnData> otherPlayTileReturnDatas = new List<OtherPlayTileReturnData>();
        public List<PengGangReturnData> pengGangReturnDatas = new List<PengGangReturnData>();
        public List<ChiReturnData> chiReturnDatas = new List<ChiReturnData>();
        /// <summary>
        /// 房间四个玩家胡碰杠吃的数据是否全部返回
        /// </summary>
        public bool isExcutedPlayTileDataReturn = false;
        /// <summary>
        /// 房间是否为组队模式
        /// </summary>
        public bool isZuDui = false;
        /// <summary>
        /// 房间类型
        /// </summary>
        public RoomType roomType = RoomType.SiChuan;
        /// <summary>
        /// 房间需要出牌的玩家Id
        /// </summary>
        public int needPlayTileId = 0;
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
        /// <summary>
        /// 房间四个玩家抢杠胡的数据是否全部返回
        /// </summary>
        public bool isExcutedQiangGangTileDataReturn = false;

    }

    public class CPGProperties : MonoBehaviour
    {
        public PengGangType cpgType;
        public List<int> cpgList;
    }
}


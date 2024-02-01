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
        /// �����ʱ�ĳ�������
        /// </summary>
        public int playTileNum = 0;
        /// <summary>
        /// �����ʱ�ĳ������Id
        /// </summary>
        public int playTileId = 0;
        public Net playTilePeer = null;
        public List<SelectTileReturnData> selectTileReturnDatas = new List<SelectTileReturnData>();
        public bool isExcutedSelectTileDataReturn = false;
        /// <summary>
        /// �����ĸ���Һ����ܳԵ�����
        /// </summary>
        public List<OtherPlayTileReturnData> otherPlayTileReturnDatas = new List<OtherPlayTileReturnData>();
        public List<PengGangReturnData> pengGangReturnDatas = new List<PengGangReturnData>();
        public List<ChiReturnData> chiReturnDatas = new List<ChiReturnData>();
        /// <summary>
        /// �����ĸ���Һ����ܳԵ������Ƿ�ȫ������
        /// </summary>
        public bool isExcutedPlayTileDataReturn = false;
        /// <summary>
        /// �����Ƿ�Ϊ���ģʽ
        /// </summary>
        public bool isZuDui = false;
        /// <summary>
        /// ��������
        /// </summary>
        public RoomType roomType = RoomType.SiChuan;
        /// <summary>
        /// ������Ҫ���Ƶ����Id
        /// </summary>
        public int needPlayTileId = 0;
        /// <summary>
        /// �����Ƿ�ܺ����
        /// </summary>
        public bool isGangHouPlayTile = false;
        /// <summary>
        /// ����Ӹ���ҵ�Id
        /// </summary>
        public int JiaGangId = 0;
        /// <summary>
        /// �����ĸ�������ܺ��������Ƿ�ȫ������
        /// </summary>
        public List<QiangGangReturnData> qiangGangReturnDatas = new List<QiangGangReturnData>();
        /// <summary>
        /// �����ĸ�������ܺ��������Ƿ�ȫ������
        /// </summary>
        public bool isExcutedQiangGangTileDataReturn = false;

    }

    public class CPGProperties : MonoBehaviour
    {
        public PengGangType cpgType;
        public List<int> cpgList;
    }
}


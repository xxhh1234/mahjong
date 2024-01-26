using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer
{
    public class Settle
    {
        /// <summary>
        /// 计算其他人出牌以后，玩家胡牌的分数
        /// </summary>
        /// <param name="roomType">房间类型</param>
        /// <param name="otherPlayTileType">其他人出牌以后，玩家胡牌的类型</param>
        /// <param name="baseScore">底分</param>
        /// <returns></returns>
        public int GetHuScore(RoomType roomType, OtherPlayTileType otherPlayTileType, int baseScore=400)
        {
            switch (otherPlayTileType)
            {
                case OtherPlayTileType.buhu:
                    return 0;
                case OtherPlayTileType.pinhu:
                    return 400;
                
            }

            return 0;
        }

        /// <summary>
        /// 计算自己摸牌以后，玩家胡牌的分数
        /// </summary>
        /// <param name="roomType">房间类型</param>
        /// <param name="ziMoType">自摸类型</param>
        /// <param name="baseScore">底分</param>
        /// <returns></returns>
        public int GetZiMoScore(RoomType roomType, ZiMoType ziMoType, int baseScore=400)
        {
            return 0;
        }

        /// <summary>
        /// 计算自己杠后摸牌以后，玩家胡牌的分数
        /// </summary>
        /// <param name="roomType">房间类型</param>
        /// <param name="ShangHuaType">杠上开花类型</param>
        /// <param name="baseScore">底分</param>
        /// <returns></returns>
        public int GetShangHuaScore(RoomType roomType, ShangHuaType ShangHuaType, int baseScore=400)
        {
            return 0;
        }
    }
}

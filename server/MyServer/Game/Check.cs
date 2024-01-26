using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer
{
    public class Check
    {
        /// <summary>
        /// 普通胡牌条件：回溯删除将牌，顺子，刻子
        /// </summary>
        /// <param name="sortedDict"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool BackStrackDeleteTile(SortedDictionary<int, int> sortedDict, int n)
        {
            if (n <= 0) return true;
            while (sortedDict.First().Value == 0) sortedDict.Remove(sortedDict.First().Key);
            var it = sortedDict.First();
            // 删除将牌
            if (n % 3 != 0 && it.Value >= 2)
            {
                sortedDict[it.Key] -= 2;
                if (BackStrackDeleteTile(sortedDict, n - 2)) return true;
                if(sortedDict.ContainsKey(it.Key)) sortedDict[it.Key] += 2;
                else                              sortedDict.Add(it.Key, 2);
            }
            // 删除刻子
            if (it.Value >= 3)
            {
                sortedDict[it.Key] -= 3;
                if (BackStrackDeleteTile(sortedDict, n - 3)) return true;
                if (sortedDict.ContainsKey(it.Key)) sortedDict[it.Key] += 3;
                else                                sortedDict.Add(it.Key, 3);
            }
            // 删除顺子
            if (sortedDict.ContainsKey(it.Key) && sortedDict.ContainsKey(it.Key + 1) && sortedDict.ContainsKey(it.Key + 2)
                && sortedDict[it.Key] >= 1 && sortedDict[it.Key + 1] >= 1 && sortedDict[it.Key + 2] >= 1)
            {
                sortedDict[it.Key] -= 1;
                sortedDict[it.Key + 1] -= 1;
                sortedDict[it.Key + 2] -= 1;
                if (BackStrackDeleteTile(sortedDict, n - 3)) return true;
                if (sortedDict.ContainsKey(it.Key)) sortedDict[it.Key] += 1;
                else sortedDict.Add(it.Key, 1);
                if (sortedDict.ContainsKey(it.Key + 1)) sortedDict[it.Key + 1] += 1;
                else sortedDict.Add(it.Key + 1, 1);
                if (sortedDict.ContainsKey(it.Key + 2)) sortedDict[it.Key + 2] += 1;
                else sortedDict.Add(it.Key + 2, 1);
            }
            return false;
        }
        /// <summary>
        /// 4个顺子/刻子+ 1对将牌即可胡牌
        /// </summary>
        /// <param name="tileList"></param>
        public static bool IsHu(List<int> tileList)
        {
            SortedDictionary<int, int> sortedDict = new SortedDictionary<int, int>();
            for (int i = 0; i < tileList.Count; ++i)
            {
                if(sortedDict.ContainsKey(tileList[i])) sortedDict[tileList[i]] += 1;
                else                                    sortedDict.Add(tileList[i], 1);
            }
            return BackStrackDeleteTile(sortedDict, tileList.Count);
        }
        /// <summary>
        /// 小七对胡牌条件：七对相同的牌
        /// </summary>
        /// <param name="tileList"></param>
        /// <returns></returns>
        public static bool IsXiaoQiDui(List<int>tileList)
        {
            if(tileList.Count != 14) return false;
            for (int i = 0; i < tileList.Count - 1; ++i)
            {
                if (tileList[i] != tileList[i + 1])
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 根据牌的数字获取牌的类型
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static TileType GetTileType(int num)
        {
            if(num >= (int)TileTypeTileNum.bamboo1 && num <= (int)TileTypeTileNum.bamboo9)            return TileType.bamboo;
            else if(num >= (int)TileTypeTileNum.circle1 && num <=(int)TileTypeTileNum.circle9)        return TileType.circle;
            else if(num >= (int)TileTypeTileNum.character1 && num <= (int)TileTypeTileNum.character9) return TileType.character;
            else if(num == (int)TileTypeTileNum.wind1 || num == (int)TileTypeTileNum.wind2
                || num == (int)TileTypeTileNum.wind3 || num == (int)TileTypeTileNum.wind4
                || num == (int)TileTypeTileNum.RedDragon || num == (int)TileTypeTileNum.GreenDragon
                || num == (int)TileTypeTileNum.WhiteDragon)                                           return TileType.honor;                                           
            else return TileType.honor;
        }
        /// <summary>
        ///  混一色和清一色胡牌条件： 只有两种花色为混一色， 只有一种花色为清一色
        /// </summary>
        /// <param name="tileList"></param>
        /// <returns></returns>
        public static int GetTileTypeNum(List<int> tileList)
        {
            if(tileList.Count != 14) return 0;
            HashSet<TileType>hst = new HashSet<TileType>();
            for(int i = 0; i < tileList.Count; ++i)
            {
                hst.Add(GetTileType(tileList[i]));
            }
            return hst.Count;
        }
        /// <summary>
        /// 大对子胡牌条件，4个刻子+一对将牌
        /// </summary>
        /// <param name="tileList"></param>
        /// <returns></returns>
        public static bool IsDaDuiZi(List<int> tileList)
        {
            if(tileList.Count != 14) return false;
            HashSet<int> hst = new HashSet<int>();
            for(int i = 0; i < tileList.Count; ++i)
            {
                hst.Add(tileList[i]);
            }
            if(hst.Count == 5) return true;
            else               return false;
        }
        /// <summary>
        /// 大三元胡牌条件， 有中发白三个刻子
        /// </summary>
        /// <param name="tileList"></param>
        /// <returns></returns>
        public static bool IsDaSanYuan(List<int> tileList)
        {
            if (tileList.Count != 14) return false;
            int zfb = 0;
            for(int i = 0; i < tileList.Count; ++i)
            {
                if(tileList[i] == (int)TileTypeTileNum.RedDragon
                || tileList[i] == (int)TileTypeTileNum.GreenDragon
                || tileList[i] == (int)TileTypeTileNum.WhiteDragon) ++zfb; 
            }
            if(zfb >= 9) return true;
            else         return false;
        }

        /// <summary>
        /// 判断玩家出牌后，其他玩家胡牌的类型
        /// </summary>
        /// <param name="chair"></param>
        /// <param name="playTileNum"></param>
        /// <param name="isGangHouChuPai"></param>
        /// <param name="isQiangGang"></param>
        /// <returns></returns>
        public static OtherPlayTileType OtherTileHuTileType(Chair chair, int playTileNum, bool isGangHouChuPai, bool isQiangGang)
        {
            // 1.加入出牌并判断是否有缺牌花色
            List<int>tileList = new List<int>(chair.handTile);
            tileList.Add(playTileNum);
            for (int i = 0; i < tileList.Count; ++i)
            {
                if (GetTileType(tileList[i]) == chair.queTileType)
                {
                    return OtherPlayTileType.buhu;
                }
            }
            tileList.Sort();
            // 2.判断是否胡牌
            bool isHu = IsHu(tileList);
            // 3.加入吃碰杠牌并判断是否有小七对
            if (chair.cpg.Count > 0)
            {
                foreach (var item in chair.cpg)
                {
                    for (int i = 0; i < 3; ++i)
                        tileList.Add(item.cpgList[i]);
                }
            }
            tileList.Sort();
            bool isXiaoQiDui = IsXiaoQiDui(tileList);
            if(!isHu && !isXiaoQiDui) return OtherPlayTileType.buhu;
            // 4. 判断清一色还是混一色
            bool isQingYiSe = false, isHunYiSe = false;
            int tileTypeNum = GetTileTypeNum(tileList);
            if(tileTypeNum == 1) isQingYiSe = true;
            else if(tileTypeNum == 2) isHunYiSe = true;
            // 5.判断是否是大对子或者大三元
            bool isDaDuiZi = IsDaDuiZi(tileList);

            bool isDaSanYuan = IsDaSanYuan(tileList);

            // bool isShuangGang = false;
            // 总共四十种牌型
            if(isGangHouChuPai)
            {
                if(isQingYiSe)
                {
                    if(isXiaoQiDui)      return OtherPlayTileType.gsp_qingyise_xiaoqidui;
                    else if(isDaDuiZi)   return OtherPlayTileType.gsp_qingyise_daduizi;
                    else if(isDaSanYuan) return OtherPlayTileType.gsp_qingyise_dasanyuan;
                    else                 return OtherPlayTileType.gsp_qingyise;
                }
                else if(isHunYiSe)
                {
                    if (isXiaoQiDui)      return OtherPlayTileType.gsp_hunyise_xiaoqidui;
                    else if (isDaDuiZi)   return OtherPlayTileType.gsp_hunyise_daduizi;
                    else if (isDaSanYuan) return OtherPlayTileType.gsp_hunyise_dasanyuan;
                    else                  return OtherPlayTileType.gsp_hunyise;
                }
                else
                {
                    if (isXiaoQiDui) return OtherPlayTileType.gsp_xiaoqidui;
                    else if(isDaSanYuan)
                    {
                        if(isDaDuiZi) return OtherPlayTileType.gsp_daduizi_dasanyuan;
                        else          return OtherPlayTileType.gsp_dasanyuan;
                    }
                    else if (isDaDuiZi) return OtherPlayTileType.gsp_daduizi;
                    else                return OtherPlayTileType.gsp_pinhu;
                }
            }
            else if(isQiangGang)
            {
                if(isQingYiSe)
                {
                    if (isXiaoQiDui) return OtherPlayTileType.qg_qingyise_xiaoqidui;
                    else if (isDaDuiZi) return OtherPlayTileType.qg_qingyise_daduizi;
                    else if (isDaSanYuan) return OtherPlayTileType.qg_qingyise_dasanyuan;
                    else                  return OtherPlayTileType.qg_qingyise;
                }
                else if(isHunYiSe)
                {
                    if (isXiaoQiDui)      return OtherPlayTileType.qg_hunyise_xiaoqidui;
                    else if (isDaDuiZi)   return OtherPlayTileType.qg_hunyise_daduizi;
                    else if (isDaSanYuan) return OtherPlayTileType.qg_hunyise_dasanyuan;
                    else                  return OtherPlayTileType.gsp_hunyise;
                }
                else
                {
                    if (isXiaoQiDui) return OtherPlayTileType.qg_xiaoqidui;
                    else if(isDaSanYuan)
                    {
                        if(isDaDuiZi) return OtherPlayTileType.qg_daduizi_dasanyuan;
                        else          return OtherPlayTileType.qg_dasanyuan;
                    }
                    else if (isDaDuiZi) return OtherPlayTileType.qg_daduizi;
                    else                return OtherPlayTileType.qg_pinhu;
                }
            }
            else
            {
                if(isQingYiSe)
                {
                    if (isXiaoQiDui)      return OtherPlayTileType.qingyise_xiaoqidui;
                    else if (isDaDuiZi)   return OtherPlayTileType.qingyise_daduizi;
                    else if (isDaSanYuan) return OtherPlayTileType.qingyise_dasanyuan;
                    else                  return OtherPlayTileType.qingyise;
                }
                else if(isHunYiSe)
                {
                    if (isXiaoQiDui) return OtherPlayTileType.hunyise_xiaoqidui;
                    else if (isDaDuiZi) return OtherPlayTileType.hunyise_daduizi;
                    else if (isDaSanYuan) return OtherPlayTileType.hunyise_dasanyuan;
                    else                  return OtherPlayTileType.hunyise;
                }
                else
                {
                    if(isXiaoQiDui) return OtherPlayTileType.xiaoqidui;
                    else if (isDaSanYuan)
                    {
                        if (isDaDuiZi) return OtherPlayTileType.daduizi_dasanyuan;
                        else return OtherPlayTileType.dasanyuan;
                    }
                    else if (isDaDuiZi) return OtherPlayTileType.daduizi;
                    else                return OtherPlayTileType.pinhu;
                }
            }
        }
        
        /// <summary>
        /// 判断自己摸牌后，本家胡牌的类型
        /// </summary>
        /// <param name="handTile"></param>
        /// <param name="cpgList"></param>
        /// <param name="drawTileNum"></param>
        /// <returns></returns>
        public static ZiMoType ZiMoHuTileType(Chair chair)
        {
            // 1.加入摸牌，判断是否有缺牌花色
            List<int> tileList = new List<int>(chair.handTile);
            tileList.Add(chair.drawTileNum);           
            for (int i = 0; i < tileList.Count; ++i)
            {
                if (GetTileType(tileList[i]) == chair.queTileType)
                {
                    return ZiMoType.buzimo;
                }
            }
            // 2.如果牌的数量等于14则门前清
            bool isMenQing = false;
            if (tileList.Count == 14) isMenQing = true;
            // 3.判断是否胡牌
            bool isHu = IsHu(tileList);
            // 4.加入吃碰杠牌并判断是否有小七对
            if (chair.cpg.Count > 0)
            {
                foreach (var item in chair.cpg)
                {
                    for (int i = 0; i < 3; ++i)
                        tileList.Add(item.cpgList[i]);
                }
            }
            tileList.Sort();
            bool isXiaoQiDui = IsXiaoQiDui(tileList);
            if (!isHu && !isXiaoQiDui) return ZiMoType.buzimo;
            // 5. 判断清一色还是混一色
            bool isQingYiSe = false, isHunYiSe = false;
            int tileTypeNum = GetTileTypeNum(tileList);
            if (tileTypeNum == 1) isQingYiSe = true;
            else if (tileTypeNum == 2) isHunYiSe = true;
            // 6.判断是否是大对子或者大三元
            bool isDaDuiZi = IsDaDuiZi(tileList);

            bool isDaSanYuan = IsDaSanYuan(tileList);
            if (isMenQing)
            {
                if (isQingYiSe)
                {
                    if(isXiaoQiDui) return ZiMoType.mq_qingyise_xiaoqidui;
                    else if (isDaDuiZi) return ZiMoType.mq_qingyise_daduizi;
                    else if (isDaSanYuan) return ZiMoType.mq_qingyise_dasanyuan;
                    else return ZiMoType.mq_qingyise;
                }
                else if (isHunYiSe)
                {
                    if(isXiaoQiDui) return ZiMoType.mq_hunyise_xiaoqidui;
                    else if (isDaDuiZi) return ZiMoType.mq_hunyise_daduizi;
                    else if (isDaSanYuan) return ZiMoType.mq_hunyise_dasanyuan;
                    else return ZiMoType.mq_hunyise;
                }
                else
                {
                    if(isXiaoQiDui) return ZiMoType.mq_xiaoqidui;
                    else if (isDaSanYuan)
                    {
                        if (isDaDuiZi) return ZiMoType.mq_daduizi_dasanyuan;
                        else return ZiMoType.mq_dasanyuan;
                    }
                    else if (isDaDuiZi) return ZiMoType.mq_daduizi;
                    else return ZiMoType.mq_zimo;
                }
            }
            else
            {
                if (isQingYiSe)
                {
                    if(isXiaoQiDui) return ZiMoType.qingyise_xiaoqidui;
                    else if (isDaDuiZi) return ZiMoType.qingyise_daduizi;
                    else if (isDaSanYuan) return ZiMoType.qingyise_dasanyuan;
                    else return ZiMoType.qingyise;
                }
                else if (isHunYiSe)
                {
                    if(isXiaoQiDui) return ZiMoType.hunyise_xiaoqidui;
                    else if (isDaDuiZi) return ZiMoType.hunyise_daduizi;
                    else if (isDaSanYuan) return ZiMoType.hunyise_dasanyuan;
                    else return ZiMoType.hunyise;
                }
                else
                {
                    if(isXiaoQiDui) return ZiMoType.xiaoqidui;
                    else if (isDaSanYuan)
                    {
                        if (isDaDuiZi) return ZiMoType.daduizi_dasanyuan;
                        else return ZiMoType.dasanyuan;
                    }
                    else if (isDaDuiZi) return ZiMoType.daduizi;
                    else return ZiMoType.zimo;
                }
            }
        }

        /// <summary>
        /// 判断杠后自己摸牌，本家胡牌的类型
        /// </summary>
        /// <param name="handTile"></param>
        /// <param name="cpgList"></param>
        /// <param name="drawTileNum"></param>
        /// <returns></returns>
        public static ShangHuaType GangShangKaiHuaHuTileType(Chair chair)
        {
            // 1.杠后摸牌并判断是否有缺牌花色
            List<int> tileList = new List<int>(chair.handTile);
            tileList.Add(chair.drawTileNum);
            for (int i = 0; i < tileList.Count; ++i)
            {
                if (GetTileType(tileList[i]) == chair.queTileType)
                {
                    return ShangHuaType.bushanghua;
                }
            }
            // 2.如果牌的数量等于14则门前清
            bool isMenQing = false;
            if(tileList.Count == 14) isMenQing = true;
            // 3. 判断是否胡牌
            bool isHu = IsHu(tileList);
            // 4.加入吃碰杠区域的牌并判断是否是小七对
            if (chair.cpg.Count > 0)
            {
                foreach (var item in chair.cpg)
                {
                    for (int i = 0; i < 3; ++i)
                        tileList.Add(item.cpgList[i]);
                }
            }
            tileList.Sort();
            bool isXiaoQiDui = IsXiaoQiDui(tileList);
            if (!isHu && !isXiaoQiDui) return ShangHuaType.bushanghua;
            // 5. 判断清一色还是混一色
            bool isQingYiSe = false, isHunYiSe = false;
            int tileTypeNum = GetTileTypeNum(tileList);
            if (tileTypeNum == 1) isQingYiSe = true;
            else if (tileTypeNum == 2) isHunYiSe = true;

            bool isDaDuiZi = IsDaDuiZi(tileList);

            bool isDaSanYuan = IsDaSanYuan(tileList);
            if(isMenQing)
            {
                if (isQingYiSe)
                {
                    if (isDaDuiZi)        return ShangHuaType.mq_qingyise_dadui_hua;
                    else if (isDaSanYuan) return ShangHuaType.mq_qingyise_dasanyuan_hua;
                    else                  return ShangHuaType.mq_qingyise_hua;
                }
                else if (isHunYiSe)
                {
                    if (isDaDuiZi)        return ShangHuaType.mq_hunyise_dadui_hua;
                    else if (isDaSanYuan) return ShangHuaType.mq_hunyise_dasanyuan_hua;
                    else                  return ShangHuaType.mq_hunyise_hua;
                }
                else
                {
                    if (isDaSanYuan)
                    {
                        if (isDaDuiZi) return ShangHuaType.mq_dadui_dasanyuan_hua;
                        else           return ShangHuaType.mq_dasanyuan_hua;
                    }
                    else if (isDaDuiZi) return ShangHuaType.mq_dadui_hua;
                    else return ShangHuaType.mq_gangshanghua;
                }
            }
            else
            {
                if (isQingYiSe)
                {
                    if (isDaDuiZi)        return ShangHuaType.qingyise_dadui_hua;
                    else if (isDaSanYuan) return ShangHuaType.qingyise_dasanyuan_hua;
                    else                  return ShangHuaType.qingyise_hua;
                }
                else if (isHunYiSe)
                {
                    if (isDaDuiZi)        return ShangHuaType.hunyise_dadui_hua;
                    else if (isDaSanYuan) return ShangHuaType.hunyise_dasanyuan_hua;
                    else                  return ShangHuaType.hunyise_hua;
                }
                else
                {
                    if (isDaSanYuan)
                    {
                        if (isDaDuiZi) return ShangHuaType.dadui_dasanyuan_hua;
                        else           return ShangHuaType.dasanyuan_hua;
                    }
                    else if (isDaDuiZi) return ShangHuaType.dadui_hua;
                    else return ShangHuaType.gangshanghua;
                }
            }
        }

        /// <summary>
        /// 判断玩家出牌后，其他玩家碰杠牌的类型
        /// </summary>
        /// <param name="chair"></param>
        /// <param name="playTileNum"></param>
        /// <returns></returns>
        public static PengGangType CheckPengGangType(Chair chair, int playTileNum)
        {
            if(GetTileType(playTileNum) == chair.queTileType) return PengGangType.none;
            int sameNum = 0;
            for(int i = 0; i < chair.handTile.Count; ++i)
            {
                if(chair.handTile[i] == playTileNum)
                    ++sameNum;
            }
            if(sameNum == 2)      return PengGangType.peng;
            else if(sameNum == 3) return PengGangType.gang;
            else                  return PengGangType.none;
        }

        /// <summary>
        /// 判断自己摸牌后，本家暗杠加杠牌的类型
        /// </summary>
        /// <param name="chair"></param>
        /// <param name="drawTileNum"></param>
        /// <returns></returns>
        public static PengGangType CheckAnGangJiaGangType(Chair chair, int drawTileNum)
        {
            if (GetTileType(drawTileNum) == chair.queTileType) return PengGangType.none;
            Dictionary<int, int>dictHandTile = new Dictionary<int, int>();
            for(int i = 0; i < chair.handTile.Count; ++i)
            {
                if(dictHandTile.ContainsKey(chair.handTile[i])) ++dictHandTile[chair.handTile[i]];
                else                                            dictHandTile.Add(chair.handTile[i], 1);
            }
            if(dictHandTile.ContainsKey(drawTileNum) && dictHandTile[drawTileNum] == 3) return PengGangType.angang;
            for(int i = 0; i < chair.cpg.Count; ++i)
            {
                if(chair.cpg[i].cpgType == PengGangType.peng && chair.cpg[i].cpgList[0] == drawTileNum)
                    return PengGangType.jiagang;
            }
            return PengGangType.none;
        }

        /// <summary>
        /// 判断玩家出牌后，下家是否吃玩家出牌
        /// </summary>
        /// <param name="chair"></param>
        /// <param name="playTileNum"></param>
        /// <returns></returns>
        public static List<int> CheckDownChi(Chair chair, int playTileNum)
        {
            List<int>tempList = new List<int>();
            TileType tileType = GetTileType(playTileNum);
            if(!tileType.Equals(TileType.wind))
            {
                List<int>handTile = new List<int>(chair.handTile);
                if(handTile.Contains(playTileNum + 1) && handTile.Contains(playTileNum + 2))
                {
                    tempList.Add(playTileNum);
                    tempList.Add(playTileNum + 1);
                    tempList.Add(playTileNum + 2);
                }
                if (handTile.Contains(playTileNum - 1) && handTile.Contains(playTileNum + 1))
                {
                    tempList.Add(playTileNum - 1);
                    tempList.Add(playTileNum);
                    tempList.Add(playTileNum + 1);
                }
                if (handTile.Contains(playTileNum - 1) && handTile.Contains(playTileNum - 2))
                {
                    tempList.Add(playTileNum - 2);
                    tempList.Add(playTileNum - 1);
                    tempList.Add(playTileNum);
                }
            }
            return tempList;
        }

    }
}

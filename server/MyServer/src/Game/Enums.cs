public enum RoomType
{
    SiChuan, WuHan
}

public enum RoomState
{
    Idle, play, settle
}


public enum PlayerPosition
{
    east, south, west, north
}

public enum TileType
{
    none, wan, tiao, tong, feng, zi, hua
}

public enum TileTypeTileNum
{
    tiao_1 = 1, tiao_2, tiao_3, tiao_4, tiao_5, tiao_6, tiao_7, tiao_8, tiao_9,
    tong_1 = 21, tong_2, tong_3, tong_4, tong_5, tong_6, tong_7, tong_8, tong_9,
    wan_1 = 41, wan_2, wan_3, wan_4, wan_5, wan_6, wan_7, wan_8, wan_9,
    dong = 61, nan = 73, xi = 85, bei = 97,
    zhong = 109, fa = 121, bai = 133
}

public enum HuTileType
{
    buhu, pinhu, xiaoqidui,
    hunyise, qingyise, daduizi, dasanyuan,
    hunyise_daduizi, hunyise_dasanyuan, hunyise_xiaoqidui,
    qingyise_daduizi, qingyise_dasanyuan, qingyise_xiaoqidui,
    daduizi_dasanyuan,

    // 单吊将的情况

    // 杠后出牌，杠上炮的情况
    gsp_pinhu, gsp_xiaoqidui,
    gsp_hunyise, gsp_qingyise, gsp_daduizi, gsp_dasanyuan,
    gsp_hunyise_daduizi, gsp_hunyise_dasanyuan, gsp_hunyise_xiaoqidui,
    gsp_qingyise_daduizi, gsp_qingyise_dasanyuan, gsp_qingyise_xiaoqidui,
    gsp_daduizi_dasanyuan,
    // 加杠时，其它玩家抢杠胡的情况
    qg_pinhu, qg_xiaoqidui,
    qg_hunyise, qg_qingyise, qg_daduizi, qg_dasanyuan,
    qg_hunyise_daduizi, qg_hunyise_dasanyuan, qg_hunyise_xiaoqidui,
    qg_qingyise_daduizi, qg_qingyise_dasanyuan, qg_qingyise_xiaoqidui,
    qg_daduizi_dasanyuan,
    // 双杠以后胡的情况
    // 三杠以后胡的情况
}

public enum ZiMoType
{
    buzimo, zimo, xiaoqidui,
    hunyise, qingyise, daduizi, dasanyuan,
    hunyise_daduizi, hunyise_dasanyuan, hunyise_xiaoqidui,
    qingyise_daduizi, qingyise_dasanyuan, qingyise_xiaoqidui,
    daduizi_dasanyuan,

    mq_zimo, mq_xiaoqidui,
    mq_hunyise, mq_qingyise, mq_daduizi, mq_dasanyuan,
    mq_hunyise_daduizi, mq_hunyise_dasanyuan, mq_hunyise_xiaoqidui,
    mq_qingyise_daduizi, mq_qingyise_dasanyuan, mq_qingyise_xiaoqidui,
    mq_daduizi_dasanyuan,

}

public enum ShangHuaType
{
    bushanghua,
    //2番
    gangshanghua,
    hunyise_hua, dadui_hua,
    qingyise_hua, dasanyuan_hua,
    hunyise_dadui_hua, hunyise_dasanyuan_hua,
    qingyise_dadui_hua, qingyise_dasanyuan_hua,
    dadui_dasanyuan_hua,

    //门清
    mq_gangshanghua,
    mq_hunyise_hua, mq_dadui_hua,
    mq_qingyise_hua, mq_dasanyuan_hua,
    mq_hunyise_dadui_hua, mq_hunyise_dasanyuan_hua,
    mq_qingyise_dadui_hua, mq_qingyise_dasanyuan_hua,
    mq_dadui_dasanyuan_hua,

}

public enum PengGangType
{
    peng, gang, angang, jiagang, qianggang, none
}
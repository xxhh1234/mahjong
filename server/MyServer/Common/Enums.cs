public enum RoomType
{
    SiChuan, WuHan
}

public enum PlayerPosition
{
    east, south, west, north
}

public enum TileType
{
    bamboo, circle, character, wind, honor  // 条子，饼子，万子，番子（wind，dragon） 
}

public enum TileTypeTileNum
{
    bamboo1 = 1, bamboo2, bamboo3, bamboo4, bamboo5, bamboo6, bamboo7, bamboo8, bamboo9,
    circle1 = 21, circle2, circle3, circle4, circle5, circle6, circle7, circle8, circle9,
    character1 = 41, character2, character3, character4, character5, character6, character7, character8, character9,
    wind1 = 61, wind2 = 73, wind3 = 85, wind4 = 97,
    RedDragon = 109, GreenDragon = 121, WhiteDragon = 133 // 红中， 发财， 白板
}

public enum OtherPlayTileType
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
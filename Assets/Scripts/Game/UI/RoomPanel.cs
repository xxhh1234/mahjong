using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XH;

public class RoomPanel : MonoBehaviour
{
    /// <summary>
    /// 四个玩家的信息： 顺序为逆时针， 本家下家对家上家
    /// 1. 四个玩家 2.玩家属性（头像，名称，分数，准备显示） 3.玩家手牌 
    /// 4.玩家摸牌 5.玩家抛牌第一行 6.玩家抛牌第二行 7. 玩家出牌提示区域 
    /// </summary>
    public List<GameObject> goPlayers = new List<GameObject>();
    public List<PlayerProperties> playerProperties = new List<PlayerProperties>();
    public List<GameObject> goHandTile = new List<GameObject>();
    public List<GameObject> goDrawTile = new List<GameObject>();
    public List<GameObject> goThrowTileLine1 = new List<GameObject>();
    public List<GameObject> goThrowTileLine2 = new List<GameObject>();
    public List<GameObject> goCPGArea = new List<GameObject>();
    public List<GameObject> goTipArea = new List<GameObject>();
    

    /// <summary>
    /// 房间信息: 
    /// 1.房间名称 2.房间中控 3.房间离开按钮 4.房间准备按钮 5. 房间剩余牌数 6.房间里的玩家是否能出牌 
    /// 7.房间里的玩家是否杠后出牌 8.房间吃碰杠胡按钮的父物体 9.房间过按钮的父物体 10.房间指向牌的箭头
    /// 11. 选择手牌界面 12. 定缺界面 13. 结算界面
    /// </summary>
    public Text txtRoomName;
    public Image imgController;
    public Button btnLeave;
    public Button btnReady;
    public Text txtRemainTile;
    public static bool isCanSelectTile = false;
    public static List<int>selectTileList = new List<int>();

    public bool isFirstJoinRoom = true;
    public static bool isCanPlayTile = false;
    public static bool isGangHouPlayTile = false;
    

    public GameObject goChiPengGangHu, goGuo;
    public GameObject goArrow;
    public GameObject SelectTilePanel, DingQuePanel, SettlePanel;

    private void Awake()
    {
        
        GameObject goOwn = transform.Find("own").gameObject;
        GameObject goDown = transform.Find("down").gameObject;
        GameObject goOpposite = transform.Find("opposite").gameObject;
        GameObject goUp = transform.Find("up").gameObject;
        goPlayers.Add(goOwn);
        goPlayers.Add(goDown);
        goPlayers.Add(goOpposite);
        goPlayers.Add(goUp);
        for(int i = 0; i < goPlayers.Count; ++i)
        {
            playerProperties.Add(goPlayers[i].transform.GetComponent<PlayerProperties>());
            playerProperties[i].imgHead = goPlayers[i].transform.Find("imgHead").GetComponent<Image>();
            playerProperties[i].imgMaster = goPlayers[i].transform.Find("imgHead/imgMaster").GetComponent<Image>();
            playerProperties[i].txtQue = goPlayers[i].transform.Find("imgHead/txtQue").GetComponent<Text>();
            playerProperties[i].txtUsername = goPlayers[i].transform.Find("panUsername/txtUsername").GetComponent<Text>();
            playerProperties[i].txtScore = goPlayers[i].transform.Find("panScore/txtScore").GetComponent<Text>();
            playerProperties[i].goRight = goPlayers[i].transform.Find("goRight").gameObject;
            goHandTile.Add(goPlayers[i].transform.Find("goHandTile").gameObject);
            goDrawTile.Add(goPlayers[i].transform.Find("goDrawTile").gameObject);
            goThrowTileLine1.Add(goPlayers[i].transform.Find("goThrowTileLine1").gameObject);
            goThrowTileLine2.Add(goPlayers[i].transform.Find("goThrowTileLine2").gameObject);
            goCPGArea.Add(goPlayers[i].transform.Find("goCPGArea").gameObject);
            goTipArea.Add(goPlayers[i].transform.Find("goTipArea").gameObject);
        }

        txtRoomName = transform.Find("imgRoomName/txtRoomName").GetComponent<Text>();
        imgController = transform.Find("imgController").GetComponent<Image>();
        btnLeave = transform.Find("btnLeave").GetComponent<Button>();
        btnReady = transform.Find("btnReady").GetComponent<Button>();
        txtRemainTile = transform.Find("imgRemainTile/txtRemainTile").GetComponent<Text>();
        goChiPengGangHu = transform.Find("goChiPengGangHu").gameObject;
        goGuo = transform.Find("goGuo").gameObject;
        goArrow = transform.Find("goArrow").gameObject;
        SelectTilePanel = transform.Find("SelectTilePanel").gameObject;
        DingQuePanel = transform.Find("DingQuePanel").gameObject;
        SettlePanel = transform.Find("SettlePanel").gameObject;

        btnLeave.onClick.AddListener(OnButtonLeave);
        btnReady.onClick.AddListener(OnButtonReady);
    }

    private void OnButtonLeave()
    {
        NetManager.peerClient.SendRequest((short)OpCode.LeaveRoomPanel, null);
        btnLeave.gameObject.SetActive(false);
    }
    private void OnButtonReady()
    {
        NetManager.peerClient.SendRequest((short)OpCode.ClickReady, null);
        btnReady.gameObject.SetActive(false);
        playerProperties[0].IsReady = true;
    }
}

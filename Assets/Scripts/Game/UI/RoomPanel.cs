using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XH;

public class RoomPanel : MonoBehaviour
{
    /// <summary>
    /// �ĸ���ҵ���Ϣ�� ˳��Ϊ��ʱ�룬 �����¼ҶԼ��ϼ�
    /// 1. �ĸ���� 2.������ԣ�ͷ�����ƣ�������׼����ʾ�� 3.������� 
    /// 4.������� 5.������Ƶ�һ�� 6.������Ƶڶ��� 7. ��ҳ�����ʾ���� 
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
    /// ������Ϣ: 
    /// 1.�������� 2.�����п� 3.�����뿪��ť 4.����׼����ť 5. ����ʣ������ 6.�����������Ƿ��ܳ��� 
    /// 7.�����������Ƿ�ܺ���� 8.��������ܺ���ť�ĸ����� 9.�������ť�ĸ����� 10.����ָ���Ƶļ�ͷ
    /// 11. ѡ�����ƽ��� 12. ��ȱ���� 13. �������
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

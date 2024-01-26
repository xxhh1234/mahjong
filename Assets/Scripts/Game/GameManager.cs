using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    // ��Ϸ��Ϣ
    #region
    public Peer peerClient = new Peer();
    /// <summary>
    /// �����ʵ��
    /// </summary>
    public GameObject goMainCamera;
    /// <summary>
    /// ��¼����ʵ��
    /// </summary>
    public GameObject goLoginPanel;
    /// <summary>
    /// ��־����
    /// </summary>
    public GameObject goLogPanel;
    /// <summary>
    /// ��������ʵ��
    /// </summary>
    public GameObject goLobbyPanel;
    /// <summary>
    /// �������ʵ��
    /// </summary>
    public GameObject goRoomPanel;
    /// <summary>
    /// roomʵ��
    /// </summary>
    public RoomPanel room;
    /// <summary>
    /// �ĸ���ҵ�λ��
    /// </summary>
    public List<PlayerPosition> playerPositions = new List<PlayerPosition>
    { PlayerPosition.east, PlayerPosition.south, PlayerPosition.west, PlayerPosition.north };
    /// <summary>
    /// ���Ƶ��ƶ��ٶ�
    /// </summary>
    public float tileMoveSpeed = 20.0f;
    #endregion

    /// <summary>
    /// ����������Դ����ʵ����������͵�¼����(��־����)
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitGame()
    {
        GameResources.Instance.LoadAssets();
    }

    private void Update()
    {
        if(peerClient != null)
        {
            peerClient.Service();
        }
    }

    /// <summary>
    /// ���ҵ�¼
    /// </summary>
    /// <param name="userInfo">�û���Ϣ</param>
    public void Login(UserInfo userInfo)
    {
       StartCoroutine(IELogin(userInfo));
    }
    IEnumerator IELogin(UserInfo userInfo)
    {
        // 1.�رյ�¼���沢��ʼ����Ϸ��Դ
        ObjectPool.Instance.CollectGameObject(goLoginPanel);
        yield return new WaitForSeconds(0.2f);
        for(int i = 0; i < GameResources.Instance.prefabButtons.transform.childCount; ++i)
        {
            GameResources.Instance.prefabButtonList.Add
                (GameResources.Instance.prefabButtons.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < GameResources.Instance.prefabEffects.transform.childCount; ++i)
        {
            GameResources.Instance.prefabEffectList.Add
                (GameResources.Instance.prefabEffects.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < GameResources.Instance.prefabHandTile.transform.childCount; ++i)
        {
            GameResources.Instance.prefabHandTileList.Add
                (GameResources.Instance.prefabHandTile.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < GameResources.Instance.prefabThrowTile.transform.childCount; ++i)
        {
            GameResources.Instance.prefabThrowTileList.Add
                (GameResources.Instance.prefabThrowTile.transform.GetChild(i).gameObject);
        }
        GameResources.Instance.spriteController.Add(GameResources.Instance.spriteDNXB);
        GameResources.Instance.spriteController.Add(GameResources.Instance.spriteNXBD);
        GameResources.Instance.spriteController.Add(GameResources.Instance.spriteXBDN);
        GameResources.Instance.spriteController.Add(GameResources.Instance.spriteBDNX);
        // 2. �򿪴������沢��ʼ��
        goLobbyPanel = ObjectPool.Instance.GetGameObject("LobbyPanel", GameResources.Instance.prefabLobbyPanel);
        goLobbyPanel.GetComponent<LobbyPanel>().txtId.text = "ID: " + goLoginPanel.GetComponent<LoginPanel>().inputId.text;
        goLobbyPanel.GetComponent<LobbyPanel>().txtUsername.text = userInfo.username;
        goLobbyPanel.GetComponent<LobbyPanel>().txtCoin.text = userInfo.coin.ToString();
        goLobbyPanel.GetComponent<LobbyPanel>().txtDiamond.text = userInfo.diamond.ToString();
        if (userInfo.sex == 1)
        {
            goLobbyPanel.GetComponent<LobbyPanel>().imgHead.sprite = GameResources.Instance.spriteBoy;
        }
        else if (userInfo.sex == 2)
        {
            goLobbyPanel.GetComponent<LobbyPanel>().imgHead.sprite = GameResources.Instance.spriteGirl;
        }
        else
        {
            goLobbyPanel.GetComponent<LobbyPanel>().imgHead.sprite = GameResources.Instance.spriteDefault;
        }
    }

    /// <summary>
    /// ���Ҵ����������
    /// </summary>
    /// <param name="player">�����������</param>
    /// <param name="RoomInfo">������Ϣ</param>
    public void CreateRoomPanel(Player player, RoomInfo roomInfo)
    {
        // 1.�رմ�������
        ObjectPool.Instance.CollectGameObject(goLobbyPanel);
        goLobbyPanel.GetComponent<LobbyPanel>().goCreateRoomPanel.SetActive(false);
        // 2.���Ŵ������������
        
        // 3.�򿪷�����沢�ҳ�ʼ��
        goRoomPanel = ObjectPool.Instance.GetGameObject("RoomPanel", GameResources.Instance.prefabRoomPanel);
        // 4.���÷�����Ϣ�� 1. �������� 2. ���䵱ǰ����/�����ܾ��� 3. �����п�ͼƬ
        room = goRoomPanel.GetComponent<RoomPanel>();
        InitRoom();
        room.txtRoomName.text = roomInfo.roomName.ToString();
        room.imgController.sprite = GameResources.Instance.spriteController[0];
        // 5.���ñ������ԣ� 1.���λ�� 2.���Id 3.������� 4.��ҷ��� 5.����Ա� 6.����Ƿ�׼�� 
        room.playerProperties[0].playerPos = PlayerPosition.east;
        room.playerProperties[0].actorId = player.actorId;
        room.playerProperties[0].Username = "�ǳ�:" + player.username;
        room.playerProperties[0].Score = "����:" + player.score;
        room.playerProperties[0].Sex = player.sex;
        room.playerProperties[0].IsReady = player.isReady;
        room.playerProperties[0].IsMaster = player.isMaster;
        // 6.������Ϸ״̬
        
    }

    /// <summary>
    /// ���Ҽ��뷿��
    /// </summary>
    /// <param name="player">�������</param>
    /// <param name="RoomInfo">������Ϣ</param>
    /// <param name="playerList">��������������</param>
    public void JoinRoomPanel(Player player, RoomInfo roomInfo, List<Player> playerList)
    {
        // 1.�رմ�������
        goLobbyPanel.SetActive(false);
        goLobbyPanel.GetComponent<LobbyPanel>().goJoinRoomPanel.SetActive(false);
        // 2.���ż��뷿������

        // 3.��ʾ������沢�ҳ�ʼ��
        goRoomPanel = ObjectPool.Instance.GetGameObject("RoomPanel", GameResources.Instance.prefabRoomPanel);
        // 4.���÷�����Ϣ�� 1.�������� 2.�����п�ͼƬ 
        room = goRoomPanel.GetComponent<RoomPanel>();
        InitRoom();
        room.txtRoomName.text = roomInfo.roomName.ToString();
        for(int i = 0; i < playerPositions.Count; ++i)
        {
            if(player.playerPos == playerPositions[i])
            {
                room.imgController.sprite = GameResources.Instance.spriteController[i];
                break;
            }
        }
        playerList.Add(player);
        // m������λ�õ�ֵ
        int m = (int)player.playerPos; 
        for (int i = 0; i < playerList.Count; ++i)
        {
            int n = (int)playerList[i].playerPos;
            int j = n - m +  (n - m < 0 ? 4 : 0);
            room.playerProperties[j].playerPos = playerList[i].playerPos;
            room.playerProperties[j].actorId = playerList[i].actorId;
            if(j == 0 || j == 2)
            {
                room.playerProperties[j].Username = "�ǳ�:" + playerList[i].username;
                room.playerProperties[j].Score = "����:" + playerList[i].score;
            }
            else
            {
                room.playerProperties[j].Username = "�ǳ�:" +  "\n" + playerList[i].username;
                room.playerProperties[j].Score = "����:" + "\n" + playerList[i].score;
            }
            room.playerProperties[j].Sex = playerList[i].sex;
            room.playerProperties[j].IsReady = playerList[i].isReady;   
            room.playerProperties[j].IsMaster = playerList[i].isMaster;
        }
        // 6.������Ϸ״̬
    }

    /// <summary>
    /// ������Ҽ��뷿��
    /// </summary>
    /// <param name="otherPlayer">�������</param>
    /// <param name="RoomInfo">������Ϣ</param>
    public void OtherJoinRoomPanel(Player otherPlayer, RoomInfo roomInfo)
    {
        // 1.����������Ҽ��뷿�������

        // 2.��������������ҵ����ԣ�1.���λ�� 2.���Id 3.������� 4.��ҷ������� 5.����Ա� 6.����Ƿ�׼�� 
        // m������λ�õ�ֵ
        int m = (int)room.playerProperties[0].playerPos; 
        // n�����������λ�õ�ֵ
        int n = (int)otherPlayer.playerPos;
        int j = n - m + (n - m < 0 ? 4 : 0);
        room.playerProperties[j].playerPos = otherPlayer.playerPos;
        room.playerProperties[j].actorId = otherPlayer.actorId;
        if (j == 0 || j == 2)
        {
            room.playerProperties[j].Username = "�ǳ�:" + otherPlayer.username;
            room.playerProperties[j].Score = "����:" + otherPlayer.score;
        }
        else
        {
            room.playerProperties[j].Username = "�ǳ�:" + "\n" + otherPlayer.username;
            room.playerProperties[j].Score = "����:" + "\n" + otherPlayer.score;
        }
        room.playerProperties[j].Sex = otherPlayer.sex;
        room.playerProperties[j].IsReady = otherPlayer.isReady;
        room.playerProperties[j].IsMaster = otherPlayer.isMaster;
    }

    /// <summary>
    /// �����뿪����
    /// </summary>
    public void LeaveRoomPanel()
    {
        // 1.����RoomPanel���沢����ʾLobbyPanel����
        ObjectPool.Instance.CollectGameObject(goRoomPanel);
        goLobbyPanel = ObjectPool.Instance.GetGameObject("LobbyPanel", GameResources.Instance.prefabLobbyPanel);
    }

    /// <summary>
    /// ��������뿪����
    /// </summary>
    /// <param name="leaveId">�����뿪��ҵ�Id</param>
    public void OtherLeaveRoomPanel(int leaveId)
    {
        // 1.���������뿪�������: 1.���λ�� 2.���Id 3.������� 4.��ҷ������� 5.����Ա� 6.����Ƿ�׼�� 
        for (int i = 0; i < 4; ++i)
        {

            if (room.playerProperties[i].actorId == leaveId)
            {
                room.playerProperties[i].playerPos = playerPositions[0];
                room.playerProperties[i].actorId = 0;
                room.playerProperties[i].Username = "";
                room.playerProperties[i].Sex = 0;
                room.playerProperties[i].IsReady = false;
                // �Ƿ���ׯ����Ϊfalse
                if(room.playerProperties[i].IsMaster)
                {
                    room.playerProperties[i].IsMaster = false;
                    room.playerProperties[(i + 1) % 4].IsMaster = true;
                }
                break;
            }
        }
    }

    /// <summary>
    /// ���ҵ��׼��
    /// </summary>
    public void ClickReady()
    {
        // 1.����׼������������ʾ׼����־
        room.btnReady.gameObject.SetActive(false);
        room.playerProperties[0].IsReady = true;
        // 2.������Ϸ״̬
    }

    /// <summary>
    /// ������ҵ��׼��
    /// </summary>
    /// <param name="readyId">����׼����ҵ�Id</param>
    public void OtherClickReady(int readyId)
    {
        // 1.��ʾ������ҵ�׼����־
        for (int i = 1; i < 4; ++i)
        {
            if (room.playerProperties[i].actorId == readyId)
            {
                room.playerProperties[i].IsReady = true;
                break;
            }
        }
    }
   
    /// <summary>
    /// �����ĸ���ҵ�����
    /// </summary>
    /// <param name="currentNumber">��ǰ����</param>
    /// <param name="allNumber">�ܾ���</param>
    /// <param name="remainTile">ʣ������</param>
    /// <param name="tileList"></param>
    /// <param name="drawTile">��������</param>
    /// <param name="drawTileId">�������Id</param>
    public void SendTile(int remainTile, List<int> tileList)
    {
        StartCoroutine(IESendTile(remainTile, tileList));
    }
    IEnumerator IESendTile(int remainTile, List<int> tileList)
    {
        // 1. ���뿪���䰴ť�ر�
        room.btnLeave.gameObject.SetActive(false);
        // 2. ����ɫ�Ӷ���

        // 3. �ȴ�1.5s�����ɱ��ҵ�����
        yield return new WaitForSeconds(1.5f);
        tileList.Sort();
        tileList.Reverse();
        //  �Ӵ�С���ɲ�����
        for (int i = 0; i < tileList.Count; ++i)
        {
            int tileNum = tileList[i];
            GameObject prefabTile = GetTile(tileNum, GameResources.Instance.prefabHandTileList[0]);
            GameObject tileOwn = ObjectPool.Instance.GetGameObject(prefabTile.name, prefabTile);
            tileOwn.transform.SetParent(room.goHandTile[0].transform, false);
            tileOwn.transform.localPosition = new Vector3(-115f * i, 0, 0);
            // ��ӿ����ƵĽű�
            tileOwn.TryGetComponent<Tile>(out Tile tile);
            if(tile == null) tile = tileOwn.AddComponent<Tile>();
            tile.tileNum = tileNum;
            tile.isDrawTile = false;
            if(tileOwn.GetComponent<ClickTileController>() == null)
                tileOwn.AddComponent<ClickTileController>();
        }
        // 4. �����¼ң��Լң��ϼҵ�����
        for (int i = 1; i < 4; ++i)
        {
            GameObject prefabHandTile = GameResources.Instance.prefabHandTileList[i];
            GameObject goHandTile =  ObjectPool.Instance.GetGameObject(prefabHandTile.name, prefabHandTile);
            goHandTile.transform.SetParent(room.goHandTile[i].transform, false);
            goHandTile.transform.localPosition = Vector3.zero;
        }
        // 5. �Ѹ��ԵĹ�ȡ��
        for (int i = 0; i < room.playerProperties.Count; ++i)
        {
            room.playerProperties[i].IsReady = false;
        }
        // 6. ���·���ʣ����������Ϸ״̬
        room.txtRemainTile.text = remainTile.ToString();
        // 7. ����ѡ��3��ͬ��ɫ���ƽ��沢��������ʱ
        RoomPanel.isCanSelectTile = true;
        room.SelectTilePanel.SetActive(true);
    }

    public void SelectTile(List<int>selectTileList)
    {
        // 1.������������������ѡ�е�3����
        Dictionary<short, object> dictSelectTile = new Dictionary<short, object>();
        dictSelectTile.Add(ParameterCode.selectTileList, RoomPanel.selectTileList);
        dictSelectTile.Add(ParameterCode.actorId, room.playerProperties[0].actorId);
        peerClient.SendRequest((short)OpCode.SelectTile, dictSelectTile);
        // 2. ��ȡ��ѡ�����Ƶ�����
        List<int>tileList = new List<int>();
        for(int i = 0; i < room.goHandTile[0].transform.childCount; ++i)
        {
            GameObject goTile = room.goHandTile[0].transform.GetChild(i).gameObject;
            tileList.Add(goTile.GetComponent<Tile>().tileNum);
        }
        for(int i = 0; i < selectTileList.Count; ++i)
        {
            tileList.Remove(selectTileList[i]);
        }
        // 3. ������������
        CreateHandTile(tileList);
    }

    public void AddThreeTile(List<int>selectTileList)
    {
        StartCoroutine(IESelectTileMove(selectTileList));
    }

    public void  OtherDingQue(TileType queTileType, int queId)
    {
        // 1.��ʾ������ҵĶ�ȱ��ɫ
        for(int i = 1; i < 4; ++i)
        {
            if(room.playerProperties[i].actorId == queId)
            {
                room.playerProperties[i].QueTileType = queTileType.ToString();
                break;
            }
        }
    }


    /// <summary>
    /// �����ĸ���ҵ�����
    /// </summary>
    /// <param name="isDrawTile">�Ƿ�����</param>
    /// <param name="tileNum">��������</param>
    public void ThrowTile(bool isDrawTile, int tileNum)
    {
        // 1.���ɱ�������
        GameObject prefabThrowTile = GetTile(tileNum, GameResources.Instance.prefabThrowTileList[0]);
        GameObject goThrowTile = ObjectPool.Instance.GetGameObject(prefabThrowTile.name, prefabThrowTile);
        if (room.goThrowTileLine1[0].transform.childCount < 12)
        {
            goThrowTile.transform.SetParent(room.goThrowTileLine1[0].transform, false);
            goThrowTile.transform.localPosition = new Vector3((room.goThrowTileLine1[0].transform.childCount - 1) * 70, 0, 0);
        }
        else if (room.goThrowTileLine2[0].transform.childCount < 12)
        {
            goThrowTile.transform.SetParent(room.goThrowTileLine2[0].transform, false);
            goThrowTile.transform.localPosition = new Vector3((room.goThrowTileLine2[0].transform.childCount - 1) * 70, 0, 0);
        }
        // 2.��ʾ��ʾ��ͷ
        room.goArrow.transform.SetParent(goThrowTile.transform, false);
        room.goArrow.transform.localPosition = new Vector3(0, 67, 0);
        // 3.�����ʱ�����ܺ������а�ť��ɾ��
        int n = room.goChiPengGangHu.transform.childCount;
        for (int i = n - 1; i >= 0; --i)
        {
            int m = room.goChiPengGangHu.transform.GetChild(i).childCount;
            for (int j = m - 1; j >= 0; --j)
            {
                ObjectPool.Instance.CollectGameObject(room.goChiPengGangHu.transform.GetChild(i).GetChild(j).gameObject);
            }
            ObjectPool.Instance.CollectGameObject(room.goChiPengGangHu.transform.GetChild(i).gameObject);
        }
        n = room.goGuo.transform.childCount;
        // 4.����������а�ť��ɾ��
        for (int i = n - 1; i >= 0; --i)
        {
            int m = room.goGuo.transform.GetChild(i).childCount;
            for (int j = m - 1; j >= 0; --j)
            {
                ObjectPool.Instance.CollectGameObject(room.goGuo.transform.GetChild(i).GetChild(j).gameObject);
            }
            ObjectPool.Instance.CollectGameObject(room.goGuo.transform.GetChild(i).gameObject);
        }

        // 5.���isDrawTileΪtrue��ɾ�����ƣ������������������ 
        if (isDrawTile)
        {
            if(room.goDrawTile[0].transform.childCount > 0)
            {
                n = room.goDrawTile[0].transform.childCount;
                for (int i = n - 1; i >= 0; --i)
                {
                    ObjectPool.Instance.CollectGameObject(room.goDrawTile[0].transform.GetChild(i).gameObject);
                }
            }
        }
        else
        {
            StartCoroutine(IEHandTileMove());
        }
    }

    /// <summary>
    /// ���ݳ������ݸ��³�����ҵ���Ϣ
    /// </summary>
    /// <param name="playTileNum">��������</param>
    /// <param name="playTileId">�������Id</param>
    /// <param name="otherPlayTileType">������ҳ����Ժ���Ƶ�����</param>
    /// <param name="PengGangType">����������</param>
    /// <param name="chiTileList">�����б�</param>
    public void PlayTileData(int playTileNum, int playTileId,
                                OtherPlayTileType otherPlayTileType, PengGangType PengGangType)
    {
        StartCoroutine(IEPlayTileData(playTileNum, playTileId, otherPlayTileType, PengGangType));
    }
     IEnumerator IEPlayTileData(int playTileNum, int playTileId,
                                OtherPlayTileType otherPlayTileType, PengGangType pengGangType)
    {
        // 1.���ɹ��İ�ť�ͺ��İ�ť
        if (!otherPlayTileType.Equals(OtherPlayTileType.buhu))
        {
            // �������İ�ť�Ϳ����岢���ؿ�����
            GameObject goEmpty = CreateGuoAndEmpty(ParameterCode.otherPlayTileType, OtherPlayTileType.buhu);
            // �������İ�ť�����ص�goEmpty
            CreateButton(3, goEmpty.transform, ParameterCode.otherPlayTileType, otherPlayTileType);
        }
        // 2. ���ɹ��İ�ť�����ܵİ�ť
        if (pengGangType.Equals(PengGangType.peng) || pengGangType.Equals(PengGangType.gang))
        {
            // �������İ�ť�Ϳ����岢���ؿ�����
            GameObject goEmpty = CreateGuoAndEmpty(ParameterCode.PengGangType, PengGangType.none);
            // �������İ�ť�����ص�go_emptyObject
            CreateButton(1, goEmpty.transform, ParameterCode.PengGangType, PengGangType.peng);
            if (pengGangType.Equals(PengGangType.gang))
            {
                // �������İ�ť�Ϳ����岢���ؿ�����
                GameObject goEmpty2 = CreateGuoAndEmpty(ParameterCode.PengGangType, PengGangType.none);
                // �����ܵİ�ť�����ص�go_emptyObject
                CreateButton(2, goEmpty2.transform, ParameterCode.PengGangType, PengGangType.gang);
            }
        }
       
        // 4. ��ʾ������ҳ���
        for (int i = 0; i < room.playerProperties.Count; ++i)
        {
            if (playTileId == room.playerProperties[i].actorId)
            {
                // ���ų�������

                // ���ɳ�����ʾ������������ʽ��ʾ����
                
                GameObject prefabTileTip = GetTile(playTileNum, GameResources.Instance.prefabHandTileList[0]);
                GameObject playTileTip = ObjectPool.Instance.GetGameObject(prefabTileTip.name, prefabTileTip);
                playTileTip.transform.SetParent(room.goTipArea[i].transform, false);
                playTileTip.transform.localPosition = Vector3.zero;
                if(playTileTip != null) LogPanel.Log(room.playerProperties[i].username + " " + playTileNum);
                Vector2 rect = playTileTip.GetComponent<RectTransform>().sizeDelta;
                playTileTip.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 200);
                yield return new WaitForSeconds(2f);
                playTileTip.GetComponent<RectTransform>().sizeDelta = rect;
                ObjectPool.Instance.CollectGameObject(playTileTip);
                // �ڷų���λ�ã���������ʽ��ʾ����
                Vector3 pos = Vector3.zero;
                GameObject prefabPlayTile = GetTile(playTileNum, GameResources.Instance.prefabThrowTileList[i]);
                GameObject goPlayTile =  ObjectPool.Instance.GetGameObject(prefabPlayTile.name, prefabPlayTile);
                if (room.goThrowTileLine1[i].transform.childCount < 12)
                {
                    goPlayTile.transform.SetParent(room.goThrowTileLine1[i].transform, false);
                    if (i == 1) goPlayTile.transform.SetAsFirstSibling();
                    if (i == 1)
                    {
                        pos = new Vector3(0, 50 * (room.goThrowTileLine1[i].transform.childCount - 1), 0);
                    }
                    else if (i == 2)
                    {
                        pos = new Vector3((-70) * (room.goThrowTileLine1[i].transform.childCount - 1), 0, 0);
                    }
                    else if (i == 3)
                    {
                        pos = new Vector3(0, (-50) * (room.goThrowTileLine1[i].transform.childCount - 1), 0);

                    }
                    goPlayTile.transform.localPosition = pos;

                }
                else if (room.goThrowTileLine2[i].transform.childCount < 12)
                {
                    goPlayTile.transform.SetParent(room.goThrowTileLine2[i].transform, false);
                    if (i == 1) goPlayTile.transform.SetAsFirstSibling();
                    if (i == 1)
                    {
                        pos = new Vector3(0, 50 * (room.goThrowTileLine2[i].transform.childCount - 1), 0);
                    }
                    else if (i == 2)
                    {
                        pos = new Vector3((-70) * (room.goThrowTileLine2[i].transform.childCount - 1), 0, 0);
                    }
                    else if (i == 3)
                    {
                        pos = new Vector3(0, (-50) * (room.goThrowTileLine2[i].transform.childCount - 1), 0);

                    }
                    goPlayTile.transform.localPosition = pos;
                }
                // ʹ��ͷָ�����
                room.goArrow.transform.SetParent(goPlayTile.transform, false);
                room.goArrow.transform.localPosition = new Vector3(0, 67, 0);
                // ȡ��������ҵ�������ʾ
                int n;
                if (i == 0)
                {
                    // ɾ������
                    n = room.goDrawTile[i].transform.childCount;
                    if (n > 0)
                        ObjectPool.Instance.CollectGameObject(room.goDrawTile[i].transform.GetChild(0).gameObject);
                }
                else
                {
                    // ��������
                    room.goDrawTile[i].gameObject.SetActive(false);
                }
                break;
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="playTileNum"></param>
    /// <param name="playTileId"></param>
    /// <param name="otherPlayTileDataList"></param>
    /// <param name="huHandTileList"></param>
    /// <param name="scoreList"></param>
    public void OtherPlayTileData(int playTileNum, int playTileId, List<OtherPlayTileReturnData> otherPlayTileDataList,
                                  List<List<int>>huHandTileList, List<int> scoreList)
    {
        StartCoroutine(IEOtherPlayTileData(playTileNum, playTileId, otherPlayTileDataList, huHandTileList, scoreList));
    }
    IEnumerator IEOtherPlayTileData(int playTileNum, int playTileId, List<OtherPlayTileReturnData> otherPlayTileDataList,
                                 List<List<int>> huHandTileList, List<int> scoreList)
    {    
        // 1.���ź������������ɺ�����Ч, ɾ��������ҵ����ƣ��������ƣ� ����������ʽ��ʾ�ڳ���������
        for (int i = 0; i < otherPlayTileDataList.Count; ++i)
        {
            for (int j = 0; j < room.playerProperties.Count; ++j)
            {
                if (otherPlayTileDataList[i].otherPlayTileActorId == room.playerProperties[j].actorId)
                {
                    GameObject prefabHu = GameResources.Instance.prefabEffectList[3];
                    GameObject goHu = ObjectPool.Instance.GetGameObject(prefabHu.name, prefabHu);
                    goHu.transform.SetParent(room.goTipArea[j].transform, false);
                    goHu.transform.localPosition = Vector3.zero;
                    yield return new WaitForSeconds(0.2f);

                    List<int>huTile = new List<int>(huHandTileList[i]);
                    CreateOverTile(huTile, j);
                }
            }
        }
        // 2. �����ĸ���ҵĻ���
        #region
        /*
        for (int i = 0; i < room.playerProperties.Count; ++i)
        {
            for (int j = 0; j < playerPositions.Count; ++j)
            {
                if (room.playerProperties[i].playerPos == playerPositions[j])
                {
                    room.playerProperties[i].Score = "����:" + scoreList[j];
                }
            }
        }
        */
        #endregion
        // 3. �����Ϊ�ɳ���״̬��������Ϸ״̬
        RoomPanel.isCanPlayTile = true;
    }

    /// <summary>
    /// �����������ݸ��·ǳ�����������Ƶ���Ϣ
    /// </summary>
    /// <param name="playTileNum">��������</param>
    /// <param name="pengGangId">�������Id</param>
    /// <param name="pengGangType">��������</param>
    public void PengGangTileData(int playTileNum, int pengGangId, PengGangType pengGangType)
    {
       StartCoroutine(IEPengGangTileData(playTileNum, pengGangId, pengGangType));
    }
    IEnumerator IEPengGangTileData(int playTileNum, int pengGangId, PengGangType pengGangType)
    {
        for (int i = 0; i < room.playerProperties.Count; ++i)
        {
            if (room.playerProperties[i].actorId == pengGangId)
            {
                if (pengGangType == PengGangType.peng)
                {
                    // 1.������������

                    // 2.����������Ч�����������ʾ������ʾ
                    GameObject prefabEffectPeng = GameResources.Instance.prefabEffectList[1];
                    GameObject goEffectPeng = ObjectPool.Instance.GetGameObject(prefabEffectPeng.name, prefabEffectPeng);
                    goEffectPeng.transform.SetParent(room.goTipArea[i].transform, false);
                    goEffectPeng.transform.localPosition = Vector3.zero;
                    yield return new WaitForSeconds(2f);
                    ObjectPool.Instance.CollectGameObject(goEffectPeng);
                    // 3. ɾ�������е��������ƣ��������� 
                    if (i == 0)
                    {
                        List<int> tileList = new List<int>();
                        for(int j = 0; j < room.goHandTile[i].transform.childCount; ++j)
                        {
                              tileList.Add(room.goHandTile[i].transform.GetChild(j).GetComponent<Tile>().tileNum);
                        }
                        tileList.Remove(playTileNum);
                        tileList.Remove(playTileNum);
                        yield return new WaitForSeconds(0.2f);
                        CreateHandTile(tileList);
                    }
                    else
                    {
                        for (int j = 0; j < 2; ++j)
                        {
                            int n = room.goHandTile[i].transform.GetChild(0).childCount;
                            ObjectPool.Instance.CollectGameObject
                                (room.goHandTile[i].transform.GetChild(0).GetChild(n - 1).gameObject);
                        }
                    }
                    // 4. ��ҳ��������������������ƣ������Ϊ�ɳ���״̬
                    GameObject prefabEmpty = GameResources.Instance.prefabEmpty;
                    GameObject goEmpty = ObjectPool.Instance.GetGameObject(prefabEmpty.name, prefabEmpty);
                    goEmpty.transform.SetParent(room.goCPGArea[i].transform, false);
                    if (i == 0)
                    {
                        goEmpty.transform.localPosition = new Vector3((room.goCPGArea[i].transform.childCount - 1) * 230, 0, 0);
                    }
                    else if (i == 1)
                    {
                        goEmpty.transform.localPosition = new Vector3(0, (room.goCPGArea[i].transform.childCount - 1) * 170, 0);
                    }
                    else if (i == 2)
                    {
                        goEmpty.transform.localPosition = new Vector3((room.goCPGArea[i].transform.childCount - 1) * (-230), 0, 0);
                    }
                    else if (i == 3)
                    {
                        goEmpty.transform.localPosition = new Vector3(0, (room.goCPGArea[i].transform.childCount - 1) * (-170), 0);
                    }
                    // ��ӳ���������
                    goEmpty.TryGetComponent<CPGProperties>(out CPGProperties cpg);
                    if(cpg == null) cpg = goEmpty.AddComponent<CPGProperties>();
                    cpg.cpgType = PengGangType.peng;
                    cpg.cpgList = new List<int>() { playTileNum, playTileNum, playTileNum };
                    // �����������
                    for (int j = 0; j < 3; ++j)
                    {
                        GameObject prefabPeng = GetTile(playTileNum, GameResources.Instance.prefabThrowTileList[i]);
                        GameObject goPeng = ObjectPool.Instance.GetGameObject(prefabPeng.name, prefabPeng);
                        goPeng.transform.SetParent(goEmpty.transform, false);
                        if (i == 0)
                        {
                            goPeng.transform.localPosition = new Vector3((goEmpty.transform.childCount - 1) * 70, 0, 0);
                        }
                        else if (i == 1)
                        {
                            goPeng.transform.localPosition = new Vector3(0, (3 - goEmpty.transform.childCount) * 50, 0);
                        }
                        else if (i == 2)
                        {
                            goPeng.transform.localPosition = new Vector3((goEmpty.transform.childCount - 1) * (-70), 0, 0);
                        }
                        else if (i == 3)
                        {
                            goPeng.transform.localPosition = new Vector3(0, (goEmpty.transform.childCount - 1) * (-50), 0);
                        }
                    }
                    if (i == 0) RoomPanel.isCanPlayTile = true;
                }
                else if (pengGangType == PengGangType.gang)
                {
                    // 1.���Ÿܵ�����

                    // 2.���ɸܵ���Ч�����������ʾ������ʾ
                    GameObject prefabEffectGang = GameResources.Instance.prefabEffectList[2];
                    GameObject goEffectGang = ObjectPool.Instance.GetGameObject(prefabEffectGang.name, prefabEffectGang);
                    goEffectGang.transform.SetParent(room.goTipArea[i].transform, false);
                    goEffectGang.transform.localPosition = Vector3.zero;
                    yield return new WaitForSeconds(2f);
                    ObjectPool.Instance.CollectGameObject(goEffectGang);

                    // 3. ɾ�������е����Ÿ��ƣ��������� 
                    if (i == 0)
                    {
                        List<int> tileList = new List<int>();
                        for (int j = 0; j < room.goHandTile[i].transform.childCount; ++j)
                        {
                            tileList.Add(room.goHandTile[i].transform.GetChild(j).GetComponent<Tile>().tileNum);
                        }
                        tileList.Remove(playTileNum);
                        tileList.Remove(playTileNum);
                        tileList.Remove(playTileNum);
                        yield return new WaitForSeconds(0.2f);
                        CreateHandTile(tileList);
                    }
                    else
                    {
                        for (int j = 0; j < 3; ++j)
                        {
                            int n = room.goHandTile[i].transform.GetChild(0).childCount;
                            ObjectPool.Instance.CollectGameObject
                                (room.goHandTile[i].transform.GetChild(0).GetChild(n - 1).gameObject);
                        }
                    }
                    // 4. ��ҳ����������������Ÿ��ƣ������Ϊ�ɳ���״̬
                    GameObject prefabEmpty = GameResources.Instance.prefabEmpty;
                    GameObject goEmpty = ObjectPool.Instance.GetGameObject(prefabEmpty.name, prefabEmpty);
                    goEmpty.transform.SetParent(room.goCPGArea[i].transform, false);
                    if (i == 0)
                    {
                        goEmpty.transform.localPosition = new Vector3((room.goCPGArea[i].transform.childCount - 1) * 230, 0, 0);
                    }
                    else if (i == 1)
                    {
                        goEmpty.transform.localPosition = new Vector3(0, (room.goCPGArea[i].transform.childCount - 1) * 170, 0);
                    }
                    else if (i == 2)
                    {
                        goEmpty.transform.localPosition = new Vector3((room.goCPGArea[i].transform.childCount - 1) * (-230), 0, 0);
                    }
                    else if (i == 3)
                    {
                        goEmpty.transform.localPosition = new Vector3(0, (room.goCPGArea[i].transform.childCount - 1) * (-170), 0);
                    }
                    // ��ӳ���������
                    goEmpty.TryGetComponent<CPGProperties>(out CPGProperties cpg);
                    if(cpg == null) cpg = goEmpty.AddComponent<CPGProperties>();
                    cpg.cpgType = PengGangType.gang;
                    cpg.cpgList = new List<int>() { playTileNum, playTileNum, playTileNum, playTileNum };
                    // ������Ÿ���
                    for (int j = 0; j < 4; ++j)
                    {
                        GameObject prefabGang = GetTile(playTileNum, GameResources.Instance.prefabThrowTileList[i]);
                        GameObject goGang = ObjectPool.Instance.GetGameObject(prefabGang.name, prefabGang);
                        goGang.transform.SetParent(goEmpty.transform, false);
                        if (i == 0)
                        {
                            if (j == 3) goGang.transform.localPosition = new Vector3(70, 30, 0);
                            else goGang.transform.localPosition = new Vector3((goEmpty.transform.childCount - 1) * 70, 0, 0);
                        }
                        else if (i == 1)
                        {
                            if (j == 3) goGang.transform.localPosition = new Vector3(-30, 50, 0);
                            else goGang.transform.localPosition = new Vector3(0, (3 - goEmpty.transform.childCount) * 50, 0);
                        }
                        else if (i == 2)
                        {
                            if (j == 3) goGang.transform.localPosition = new Vector3(-70, -30, 0);
                            else goGang.transform.localPosition = new Vector3((goEmpty.transform.childCount - 1) * (-70), 0, 0);
                        }
                        else if (i == 3)
                        {
                            if (j == 3) goGang.transform.localPosition = new Vector3(30, -50, 0);
                            else goGang.transform.localPosition = new Vector3(0, (goEmpty.transform.childCount - 1) * (-50), 0);
                        }
                        if (i == 0) RoomPanel.isCanPlayTile = true;
                    }
                    
                }
                
                break;
            }
        }
    }


    /// <summary>
    /// �����������ݸ���������ҵ���Ϣ
    /// </summary>
    /// <param name="remainTile">����ʣ������</param>
    /// <param name="drawTileNum">��������</param>
    /// <param name="isGangHouDrawTile">�Ƿ�ܺ�����</param>
    /// <param name="ziMoType">��������</param>
    /// <param name="ShangHuaType">���Ͽ�������</param>
    /// <param name="anGangJiaGangType">���ܼӸ�����</param>
    public void DrawTileData(int remainTile, int drawTileNum, bool isGangHouDrawTile,
                                ZiMoType ziMoType, ShangHuaType ShangHuaType, PengGangType anGangJiaGangType)
    {
        StartCoroutine(IEDrawTileData(remainTile, drawTileNum, isGangHouDrawTile,
                                    ziMoType, ShangHuaType, anGangJiaGangType));
    }
    IEnumerator IEDrawTileData(int remainTile, int drawTileNum, bool isGangHouDrawTile,
                                 ZiMoType ziMoType, ShangHuaType shangHuaType, PengGangType anGangJiaGangType)
    {
        // 1.����ʣ������
        room.txtRemainTile.text = remainTile.ToString();
        // 2.��������
        GameObject prefabDrawTile = GetTile(drawTileNum, GameResources.Instance.prefabHandTileList[0]);
        GameObject goDrawTile = ObjectPool.Instance.GetGameObject(prefabDrawTile.name, prefabDrawTile);
        goDrawTile.transform.SetParent(room.goDrawTile[0].transform, false);
        goDrawTile.transform.localPosition = Vector3.zero;
        // ����ƵĿ��ƽű�
        goDrawTile.TryGetComponent<Tile>(out Tile tile);
        if(tile == null) tile = goDrawTile.AddComponent<Tile>();
        tile.tileNum = drawTileNum;
        tile.isDrawTile = true;
        if(goDrawTile.GetComponent<ClickTileController>() == null)
            goDrawTile.AddComponent<ClickTileController>();
        room.goDrawTile[0].SetActive(true);
        // 3. ����������ҵ�����
        for (int i = 1; i < 4; ++i)
        {
            room.goDrawTile[i].SetActive(false);
        }
        yield return new WaitForSeconds(0.2f);
        // 4.���·����Ƿ�ܺ���ƣ� �Ƿ��ܳ���, �����������ϻ���ť�� ����������ܣ�ֱ����������������Ʒ�������
        #region
        RoomPanel.isGangHouPlayTile = isGangHouDrawTile;
        if (ziMoType.Equals(ZiMoType.buzimo) && shangHuaType.Equals(ShangHuaType.bushanghua)
                                        && anGangJiaGangType.Equals(PengGangType.none))
        {
            RoomPanel.isCanPlayTile = true;
        }
        else
        {
            RoomPanel.isCanPlayTile = false;
            if (!ziMoType.Equals(ZiMoType.buzimo))
            {
                // �������İ�ť�Ϳ����岢���ؿ�����
                GameObject goEmpty = CreateGuoAndEmpty(ParameterCode.ziMoType, ZiMoType.buzimo,
                                                                            (short)OpCode.DrawTileDataReturn);
                // ���������İ�ť�����ص�goEmpty
                CreateButton(5, goEmpty.transform, ParameterCode.ziMoType, ziMoType, (short)OpCode.DrawTileDataReturn);
            }
            else if (!shangHuaType.Equals(ShangHuaType.bushanghua))
            {
                // �������İ�ť�Ϳ����岢���ؿ�����
                GameObject goEmpty = CreateGuoAndEmpty(ParameterCode.shangHuaType, ShangHuaType.bushanghua,
                                                                               (short)OpCode.DrawTileDataReturn);
                // �������Ͽ����İ�ť�����ص�goEmpty
                CreateButton(6, goEmpty.transform, ParameterCode.shangHuaType, shangHuaType,
                                                                                    (short)OpCode.DrawTileDataReturn);
            }
            else if (!anGangJiaGangType.Equals(PengGangType.none))
            {
                // �������İ�ť�Ϳ����岢���ؿ�����
                GameObject goEmpty = CreateGuoAndEmpty(ParameterCode.PengGangType, PengGangType.none,
                                                                                  (short)OpCode.DrawTileDataReturn);
                // �����ܵİ�ť�����ص�goEmpty
                CreateButton(2, goEmpty.transform, ParameterCode.PengGangType, anGangJiaGangType,
                                                                                    (short)OpCode.DrawTileDataReturn);
            }
        }
        #endregion
    }

    /// <summary>
    /// �����������ݸ����������������Ϣ
    /// </summary>
    /// <param name="remainTile">ʣ������</param>
    /// <param name="drawTileId">�������Id</param>
    public void OtherDrawTileData(int remainTile, int drawTileId)
    {
        // 1.���·���ʣ������
        room.txtRemainTile.text = remainTile.ToString();
        // 2. ��ʾ����������ҵ�����
        for(int i = 1; i < room.playerProperties.Count; ++i)
        {
            if(room.playerProperties[i].actorId == drawTileId)
            {
                room.goDrawTile[i].SetActive(true);
                break;
            }
        }
    }

    /// <summary>
    /// �����������ݸ���������ҵ�����
    /// </summary>
    /// <param name="ziMoType">��������</param>
    /// <param name="ziMoId">�������Id</param>
    /// <param name="ziMoNum">��������</param>
    /// <param name="handTileList">������ҵ�����</param>
    /// <param name="scoreList">�����б�</param>
    /// <param name="coinList">����б�</param>
    /// <param name="RoomInfo">������Ϣ</param>
    public void ZiMoData(ZiMoType ziMoType, int ziMoId, int ziMoNum,
                     List<int> ziMoHandTile, List<int> scoreList)
    {
        StartCoroutine(IEZiMoData(ziMoType, ziMoId, ziMoNum, ziMoHandTile, scoreList));
    }
    IEnumerator IEZiMoData(ZiMoType ziMoType, int ziMoId, int ziMoNum,
                    List<int> ziMoHandTile, List<int> scoreList)
    {
        // 1.����������������������������Ч, ɾ��������ҵ����ƣ� ���ƣ��������ƣ� ����������ʽ��ʾ�ڳ���������
        for (int i = 0; i < room.playerProperties.Count; ++i)
        {
            if (ziMoId == room.playerProperties[i].actorId)
            {
                LogPanel.Log(room.playerProperties[i].username + "��������: " + ziMoNum);
                GameObject prefabZiMo = GameResources.Instance.prefabEffectList[4];
                GameObject goZiMo = ObjectPool.Instance.GetGameObject(prefabZiMo.name, prefabZiMo);
                goZiMo.transform.SetParent(room.goTipArea[i].transform, false);
                goZiMo.transform.localPosition = Vector3.zero;
                yield return new WaitForSeconds(1f);

                List<int> huTile = new List<int>(ziMoHandTile);
                CreateOverTile(huTile, i);
                break;
            }
        }
        // 2. �����ĸ���ҵĻ���

        // 3. �����Ϊ�ɳ���״̬��������Ϸ״̬
        RoomPanel.isCanPlayTile = true;
    }


    /// <summary>
    /// ���ݸ��Ͽ������ݸ���������ҵ�����
    /// </summary>
    /// <param name="shangHuaType"></param>
    /// <param name="shangHuaId"></param>
    /// <param name="shangHuaNum"></param>
    /// <param name="shangHuaHandTile"></param>
    /// <param name="scoreList"></param>
    public void ShangHuaData(ShangHuaType shangHuaType, int shangHuaId, int shangHuaNum,
                   List<int> shangHuaHandTile, List<int> scoreList)
    {
        StartCoroutine(IEShangHuaData(shangHuaType, shangHuaId, shangHuaNum, shangHuaHandTile, scoreList));
    }
    IEnumerator IEShangHuaData(ShangHuaType shangHuaType, int shangHuaId, int shangHuaNum,
                  List<int> shangHuaHandTile, List<int> scoreList)
    {
        // 1.���Ÿ��Ͽ��������������ɸ��Ͽ�������Ч, ɾ�����Ͽ�����ҵ����ƣ����ƣ��������ƣ� ����������ʽ��ʾ�ڳ���������
        for (int i = 0; i < room.playerProperties.Count; ++i)
        {
            if (shangHuaId == room.playerProperties[i].actorId)
            {
                LogPanel.Log(room.playerProperties[i].username + "���Ͽ�������" + shangHuaNum);
                GameObject prefabShangHua = GameResources.Instance.prefabEffectList[5];
                GameObject goShangHua = ObjectPool.Instance.GetGameObject(prefabShangHua.name, prefabShangHua);
                goShangHua.transform.SetParent(room.goTipArea[i].transform, false);
                goShangHua.transform.localPosition = Vector3.zero;
                yield return new WaitForSeconds(1f);

                List<int> huTile = new List<int>(shangHuaHandTile);
                CreateOverTile(huTile, i);
                break;
            }
        }
        // 2. �����ĸ���ҵĻ���

        // 3. �����Ϊ�ɳ���״̬��������Ϸ״̬
        RoomPanel.isCanPlayTile = true;
    }

    /// <summary>
    /// ���ݰ��ܼӸ����ݸ���������ҵ�����
    /// </summary>
    /// <param name="gangTileType">��������</param>
    /// <param name="gangTileId">��������</param>
    /// <param name="gangTileNum">��������</param>
    public void AnGangJiaGangData(PengGangType anGangJiaGangType, int anGangJiaGangId, int anGangJiaGangNum)
    {
        StartCoroutine(IEAnGangJiaGangData(anGangJiaGangType, anGangJiaGangId, anGangJiaGangNum));
    }
    IEnumerator IEAnGangJiaGangData(PengGangType anGangJiaGangType, int anGangJiaGangId, int anGangJiaGangNum)
    {
        for(int i = 0; i < room.playerProperties.Count; ++i)
        {
            if(room.playerProperties[i].actorId == anGangJiaGangId)
            {
                // 1. ɾ������
                for (int k = room.goDrawTile[i].transform.childCount - 1; k >= 0; --k)
                {
                    ObjectPool.Instance.CollectGameObject(room.goDrawTile[i].transform.GetChild(k).gameObject);
                }
                // 2. ���Ÿ����������ɸ���Ч

                GameObject prefabEffectGang = GameResources.Instance.prefabEffectList[2];
                GameObject goEffectGang = ObjectPool.Instance.GetGameObject(prefabEffectGang.name, prefabEffectGang);
                goEffectGang.transform.SetParent(room.goTipArea[i].transform, false);
                goEffectGang.transform.localPosition = Vector3.zero;
                yield return new WaitForSeconds(2f);
                ObjectPool.Instance.CollectGameObject(goEffectGang);
                // 3.���ɸ���
                LogPanel.Log(room.playerProperties[i].username + "���ܼӸܵ���"  + anGangJiaGangNum);
                if (anGangJiaGangType == PengGangType.angang)
                {
                    
                    // ɾ�������е�3����
                    for(int j = 0; j < 3; ++j)
                    {
                        for(int k = 0; k < room.goHandTile[i].transform.childCount; ++k)
                        {
                            if(i == 0)
                            {
                                if(room.goHandTile[i].transform.GetChild(k).GetComponent<Tile>().tileNum == anGangJiaGangNum)
                                    ObjectPool.Instance.CollectGameObject(room.goHandTile[i].transform.GetChild(k).gameObject);
                            }
                            else
                            {
                                ObjectPool.Instance.CollectGameObject(room.goHandTile[i].transform.GetChild(k).gameObject);
                            }
                        }
                    }
                    // ��������Ƶ���������
                    GameObject prefabEmpty = GameResources.Instance.prefabEmpty;
                    GameObject goEmpty = ObjectPool.Instance.GetGameObject(prefabEmpty.name, prefabEmpty);
                    goEmpty.transform.SetParent(room.goCPGArea[i].transform, false);
                    if (i == 0)
                    {
                        goEmpty.transform.localPosition = new Vector3((room.goCPGArea[i].transform.childCount - 1) * 230, 0, 0);
                    }
                    else if (i == 1)
                    {
                        goEmpty.transform.localPosition = new Vector3(0, (room.goCPGArea[i].transform.childCount - 1) * 170, 0);
                    }
                    else if (i == 2)
                    {
                        goEmpty.transform.localPosition = new Vector3((room.goCPGArea[i].transform.childCount - 1) * (-230), 0, 0);
                    }
                    else if (i == 3)
                    {
                        goEmpty.transform.localPosition = new Vector3(0, (room.goCPGArea[i].transform.childCount - 1) * (-170), 0);
                    }
                    // ��ӳ���������
                    goEmpty.TryGetComponent<CPGProperties>(out CPGProperties cpg);
                    if (cpg == null) cpg = goEmpty.AddComponent<CPGProperties>();
                    cpg.cpgType = PengGangType.angang;
                    cpg.cpgList = new List<int>() 
                    { anGangJiaGangNum, anGangJiaGangNum, anGangJiaGangNum, anGangJiaGangNum};
                    
                    // ������Ÿ���
                    for (int j = 0; j < 4; ++j)
                    {
                        GameObject prefabGang = GetTile(anGangJiaGangNum, GameResources.Instance.prefabThrowTileList[i]);
                        GameObject goGang = ObjectPool.Instance.GetGameObject(prefabGang.name, prefabGang);
                        goGang.transform.SetParent(goEmpty.transform, false);
                        if (i == 0)
                        {
                            if (j == 3) goGang.transform.localPosition = new Vector3(70, 30, 0);
                            else goGang.transform.localPosition = new Vector3((goEmpty.transform.childCount - 1) * 70, 0, 0);
                        }
                        else if (i == 1)
                        {
                            if (j == 3) goGang.transform.localPosition = new Vector3(-30, 50, 0);
                            else goGang.transform.localPosition = new Vector3(0, (3 - goEmpty.transform.childCount) * 50, 0);
                        }
                        else if (i == 2)
                        {
                            if (j == 3) goGang.transform.localPosition = new Vector3(-70, -30, 0);
                            else goGang.transform.localPosition = new Vector3((goEmpty.transform.childCount - 1) * (-70), 0, 0);
                        }
                        else if (i == 3)
                        {
                            if (j == 3) goGang.transform.localPosition = new Vector3(30, -50, 0);
                            else goGang.transform.localPosition = new Vector3(0, (goEmpty.transform.childCount - 1) * (-50), 0);
                        }
                    }
                }
                else if(anGangJiaGangType == PengGangType.jiagang)
                {
                    // ���һ���Ƶ���Ӧ�����ƣ���Ϊ����
                    for(int j = 0; j < room.goCPGArea[i].transform.childCount; ++j)
                    {
                        GameObject goEmpty = room.goCPGArea[i].transform.GetChild(j).gameObject;
                        if(goEmpty.GetComponent<CPGProperties>().cpgType == PengGangType.peng &&
                            goEmpty.GetComponent<CPGProperties>().cpgList[0] == anGangJiaGangNum)
                        {
                            goEmpty.GetComponent<CPGProperties>().cpgType = PengGangType.jiagang;
                            goEmpty.GetComponent<CPGProperties>().cpgList.Add(anGangJiaGangNum);

                            GameObject prefabGang = GetTile(anGangJiaGangNum, GameResources.Instance.prefabThrowTileList[i]);
                            GameObject goGang = ObjectPool.Instance.GetGameObject(prefabGang.name, prefabGang);
                            goGang.transform.SetParent(goEmpty.transform, false);
                            if (i == 0)
                            {
                                goGang.transform.localPosition = new Vector3(70, 30, 0);
                            }
                            else if(i == 1)
                            {
                                goGang.transform.localPosition = new Vector3(-30, 50, 0);
                            }
                            else if(i == 2)
                            {
                                goGang.transform.localPosition = new Vector3(-70, -30, 0);
                            }
                            else if(i == 3)
                            {
                                goGang.transform.localPosition = new Vector3(30, -50, 0);
                            }
                        }
                    }
                }
                // 4.������������
                if(i == 0)
                {
                    List<int> handTile = new List<int>();
                    for(int j = 0; j < room.goHandTile[i].transform.childCount; ++i)
                    {
                        handTile.Add(room.goHandTile[i].transform.GetChild(j).GetComponent<Tile>().tileNum);
                    }
                    CreateHandTile(handTile);
                }
             
                // 5.�����Ϊ�ɳ���״̬
                if(anGangJiaGangType == PengGangType.angang ||
                    anGangJiaGangType == PengGangType.jiagang) RoomPanel.isCanPlayTile = true;
                break;
            }
        }
    }

    public void QiangGangData(OtherPlayTileType otherPlayTileType)
    {
        if(!otherPlayTileType.Equals(OtherPlayTileType.buhu))
        {
            // �������İ�ť�Ϳ����岢���ؿ�����
            GameObject goEmpty = CreateGuoAndEmpty(ParameterCode.otherPlayTileType, OtherPlayTileType.buhu, 
                                                                                         (short)OpCode.QiangGangDataReturn);
            // �������ܵİ�ť�����ص�goEmpty
            CreateButton(7, goEmpty.transform, ParameterCode.otherPlayTileType, otherPlayTileType,
                                                                                  (short)OpCode.QiangGangDataReturn);
        }
        else
        {
            Dictionary<short, object> dictQiangGang = new Dictionary<short, object>();
            dictQiangGang.Add(ParameterCode.otherPlayTileType, OtherPlayTileType.buhu);
            peerClient.SendRequest((short)OpCode.QiangGangDataReturn, dictQiangGang);
        }
    }

    public void QiangGangDataReturn(List<QiangGangReturnData> qiangGangHuDataList, 
                                    List<List<int>> huHandTileList, List<int>scoreList)
    {
        StartCoroutine(IEQiangGangDataReturn(qiangGangHuDataList, huHandTileList, scoreList));
    }
    IEnumerator IEQiangGangDataReturn(List<QiangGangReturnData> qiangGangHuDataList,
                                    List<List<int>> huHandTileList, List<int> scoreList)
    {
        // 1.�������ܺ����������������ܺ�����Ч
        //   ɾ�����ܺ�����ҵ����ƣ��������ƣ� ����������ʽ��ʾ�ڳ���������
        for (int i = 0; i < qiangGangHuDataList.Count; ++i)
        {
            for (int j = 0; j < room.playerProperties.Count; ++j)
            {
                if (qiangGangHuDataList[i].qiangGangActorId == room.playerProperties[j].actorId)
                {
                    GameObject prefabQiangGang = GameResources.Instance.prefabEffectList[6];
                    GameObject goQiangGang = ObjectPool.Instance.GetGameObject(prefabQiangGang.name, prefabQiangGang);
                    goQiangGang.transform.SetParent(room.goTipArea[j].transform, false);
                    goQiangGang.transform.localPosition = Vector3.zero;
                    room.goTipArea[j].SetActive(true);
                    yield return new WaitForSeconds(0.2f);

                    List<int> huTile = new List<int>(huHandTileList[i]);
                    CreateOverTile(huTile, j);
                }
            }
        }
        // 2. �����ĸ���ҵĻ���
        #region
        /*
        for (int i = 0; i < room.playerProperties.Count; ++i)
        {
            for (int j = 0; j < playerPositions.Count; ++j)
            {
                if (room.playerProperties[i].playerPos == playerPositions[j])
                {
                    room.playerProperties[i].Score = "����:" + scoreList[j];
                }
            }
        }
        */
        #endregion
        // 3. �����Ϊ�ɳ���״̬��������Ϸ״̬
        RoomPanel.isCanPlayTile = true;
    }


    public void GameOver(List<List<int>> noHuHandTileList, List<int> noHuPlayerIdList, List<int>scoreList)
    {
       StartCoroutine(IEGameOver(noHuHandTileList, noHuPlayerIdList, scoreList));
    }
    IEnumerator IEGameOver(List<List<int>> noHuHandTileList, List<int>noHuPlayerIdList, List<int> scoreList)
    {
        // 1. ɾ��δ������ҵ����ƣ��������ƣ� ����������ʽ��ʾ�ڳ���������

        for (int i = 0; i < noHuHandTileList.Count; ++i)
        {
            for (int j = 0; j < room.playerProperties.Count; ++j)
            {
                if (noHuPlayerIdList[i] == room.playerProperties[j].actorId)
                {
                    List<int> huTile = new List<int>(noHuHandTileList[i]);
                    CreateOverTile(huTile, j);
                }
            }
        }
        yield return new WaitForSeconds(1f);
        // 2. ��ʾ�������
        room.SettlePanel.SetActive(true);
        // 3. ��ʾ������Һ�������ҵķ���
        // 4. ��ʾ�����뿪��ť
        room.btnLeave.gameObject.SetActive(true);

    }

    /// <summary>
    /// ��ʼ������
    /// </summary>
    public void InitRoom()
    {
        int n;
        if(!room.isFirstJoinRoom)
        {
            for (int i = 0; i < room.playerProperties.Count; ++i)
            {
                room.playerProperties[i].Score = "0";
                // ɾ������
                n = room.goThrowTileLine1[i].transform.childCount;
                for (int k = n - 1; k >= 0; --k)
                {
                    ObjectPool.Instance.CollectGameObject(room.goThrowTileLine1[i].transform.GetChild(k).gameObject);
                }
                n = room.goThrowTileLine2[i].transform.childCount;
                for (int k = n - 1; k >= 0; --k)
                {
                    ObjectPool.Instance.CollectGameObject(room.goThrowTileLine2[i].transform.GetChild(k).gameObject);
                }
                // ɾ����������
                for (int k = room.goCPGArea[i].transform.childCount - 1; k >= 0; --k)
                {
                    n = room.goCPGArea[i].transform.GetChild(k).childCount;
                    for (int m = n - 1; m >= 0; --m)
                    {
                        ObjectPool.Instance.CollectGameObject
                            (room.goCPGArea[i].transform.GetChild(k).GetChild(m).gameObject);
                    }
                    ObjectPool.Instance.CollectGameObject(room.goCPGArea[i].transform.GetChild(k).gameObject);
                }
                // ɾ����ʾ���������
                n = room.goTipArea[i].transform.childCount;
                for (int k = n - 1; k >= 0; --k)
                {
                    ObjectPool.Instance.CollectGameObject(room.goTipArea[i].transform.GetChild(k).gameObject);
                }
            }
            room.playerProperties[0].IsReady = false;

        }  
        else room.isFirstJoinRoom = false;
        room.btnReady.gameObject.SetActive(true);
        room.txtRemainTile.text = "108";
        RoomPanel.isCanSelectTile = false;
        RoomPanel.selectTileList = new List<int>();
        RoomPanel.isCanPlayTile = false;
        RoomPanel.isGangHouPlayTile = false;
        n = room.goChiPengGangHu.transform.childCount;
        for (int i = n - 1; i >= 0; --i)
        {
            int m = room.goChiPengGangHu.transform.GetChild(i).childCount;
            for (int j = m - 1; j >= 0; --j)
            {
                ObjectPool.Instance.CollectGameObject(room.goChiPengGangHu.transform.GetChild(i).GetChild(j).gameObject);
            }
            ObjectPool.Instance.CollectGameObject(room.goChiPengGangHu.transform.GetChild(i).gameObject);
        }
        n = room.goGuo.transform.childCount;
        for (int i = n - 1; i >= 0; --i)
        {
            int m = room.goGuo.transform.GetChild(i).childCount;
            for (int j = m - 1; j >= 0; --j)
            {
                ObjectPool.Instance.CollectGameObject(room.goGuo.transform.GetChild(i).GetChild(j).gameObject);
            }
            ObjectPool.Instance.CollectGameObject(room.goGuo.transform.GetChild(i).gameObject);
        }
        room.goArrow.transform.localPosition = new Vector3(1200, 0, 0);

    }

    /// <summary>
    /// �����Ƶ����ֻ�ȡ�Ƶ�Ԥ����
    /// </summary>
    /// <param name="tileNum">�Ƶ�����</param>
    /// <param name="prefab">�Ƶ�Ԥ���壬�������ƣ�����</param>
    /// <returns></returns>
    public GameObject GetTile(int tileNum, GameObject prefab)
    {
        int start = (int)TileTypeTileNum.bamboo1, id = 0;
        for (int i = 1; i <= 10; ++i)
        {
            int end;
            if (i <= 3) end = start + 9;
            else end = start + 1;
            for (; start < end; ++start)
            {
                if (start == tileNum)
                {
                    return prefab.transform.GetChild(id).gameObject;
                }
                ++id;
            }
            start = end + 11;
        }
        return prefab.transform.GetChild(0).gameObject;
    }

    IEnumerator IESelectTileMove(List<int>selectTileList)
    {
        // 1.��ȡ������������
        List<int> tileList = new List<int>(selectTileList);
        for (int i = 0; i < room.goHandTile[0].transform.childCount; ++i)
        {
            tileList.Add(room.goHandTile[0].transform.GetChild(i).GetComponent<Tile>().tileNum);
        }
        // 2. ������������
        CreateHandTile(tileList);
        LogPanel.Log("ѡ�ƺ������");
        for (int i = 0; i < room.goHandTile[0].transform.childCount; ++i)
        {
            LogPanel.Log(room.goHandTile[0].transform.GetChild(i).GetComponent<Tile>().tileNum.ToString());
        }
        yield return new WaitForSeconds(0.5f);
        // 3.����ѡ��ȱ���沢��������ʱ
        room.DingQuePanel.SetActive(true);
    }

    /// <summary>
    /// ���ò�ֵ����50��trans��λ�õ�secondPos��λ�ã�ÿ0.01f�ƶ�����һ�Σ��ƶ�50�κ󵽴�secondPos,���ŵ�lastPos
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="secondPos"></param>
    /// <param name="lastPos"></param>
    /// <returns></returns>
    IEnumerator IEDrawTileMove(Transform trans, Vector3 secondPos, Vector3 lastPos)
    {
        // 1.����50����ֵ��λ�ã� ÿ0.01f�ƶ�һ��
        for (int i = 0; i < 50; ++i)
        {
            trans.localPosition = Vector2.Lerp(trans.localPosition, secondPos, tileMoveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.2f);
        // 2. 0.2f�������ƶ�������λ��
        trans.localPosition = lastPos;
    }
    /// <summary>
    /// �������������������
    /// </summary>
    /// <returns></returns>
    IEnumerator IEHandTileMove()
    {
        // 1.��ȡ���ƺ���������
        Transform drawTileTrans = null;
        int drawTileNum = 0;
        if (room.goDrawTile[0].transform.childCount > 0)
        {
            drawTileTrans = room.goDrawTile[0].transform.GetChild(0);
            drawTileNum = drawTileTrans.GetComponent<Tile>().tileNum;
        }
        yield return new WaitForSeconds(0.2f);
        // 2. ��ȡ���Ʋ��������
        List<int> handtile = new List<int>();
        for (int i = 0; i < room.goHandTile[0].transform.childCount; ++i)
        {
            handtile.Add(room.goHandTile[0].transform.GetChild(i).GetComponent<Tile>().tileNum);
        }
        if (drawTileNum > 0) handtile.Add(drawTileNum);
        // �Ӵ�С����
        handtile.Sort();
        handtile.Reverse();
        // 3.��ȡÿ���Ƶ�λ�ã��������Ƴ����Ƶ�λ��
        List<int> tileOrder = new List<int>();
        for (int i = 0; i < handtile.Count; ++i)
        {
            tileOrder.Add(i);
        }
        int drawTileIndex = handtile.IndexOf(drawTileNum);
        tileOrder.Remove(drawTileIndex);
        // 4.ָ������λ��
        for (int i = 0; i < room.goHandTile[0].transform.childCount; ++i)
        {
            room.goHandTile[0].transform.GetChild(i).transform.localPosition = new Vector3(-115 * tileOrder[i], 0, 0);
        }
        // 5.ָ������λ��
        if (drawTileTrans != null)
        {
            drawTileTrans.SetParent(room.goHandTile[0].transform, false);
            drawTileTrans.GetComponent<Tile>().isDrawTile = false;
            drawTileTrans.SetSiblingIndex(drawTileIndex);
            Vector3 lastPos = new Vector3(-115 * drawTileIndex, 0, 0);
            drawTileTrans.localPosition += new Vector3(160, 0, 0);
            Vector3 secondPos = lastPos + new Vector3(160, 0, 0);
            StartCoroutine(IEDrawTileMove(drawTileTrans, secondPos, lastPos));
        }
    }

    /// <summary>
    /// ������ť
    /// </summary>
    /// <param name="buttonIndex">��ť��prefab_buttons�е��±�</param>
    /// <param name="parent">��ť�ĸ�����</param>
    /// <param name="otherPlayTileResponse">�����ť�Ժ󣬸��������Ļظ�����</param>
    private void CreateButton(int buttonIndex, Transform parent, 
                                        short param,  object o, short opcode=(short)OpCode.PlayTileDataReturn)
    {
        GameObject prefab = GameResources.Instance.prefabButtonList[buttonIndex];
        GameObject go = ObjectPool.Instance.GetGameObject(prefab.name, prefab);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(0, (parent.childCount - 1) * 120, 0);
        go.GetComponent<Button>().onClick.AddListener(() =>
        {
            // ��������

            // �����������������ݻظ�
            Dictionary<short, object> dict = new Dictionary<short, object>();
            if(o != null) dict.Add(param, o);
            peerClient.SendRequest(opcode, dict);
            // ɾ�����а�ť
            int n = room.goChiPengGangHu.transform.childCount;
            for (int i = n - 1; i >= 0; --i)
            {
                int m = room.goChiPengGangHu.transform.GetChild(i).childCount;
                for(int j = m - 1;  j >= 0; --j)
                {
                    ObjectPool.Instance.CollectGameObject(room.goChiPengGangHu.transform.GetChild(i).GetChild(j).gameObject);
                }
                ObjectPool.Instance.CollectGameObject(room.goChiPengGangHu.transform.GetChild(i).gameObject);
            }
            n = room.goGuo.transform.childCount;
            for (int i = n - 1; i >= 0; --i)
            {
                int m = room.goGuo.transform.GetChild(i).childCount;
                for (int j = m - 1; j >= 0; --j)
                {
                    ObjectPool.Instance.CollectGameObject(room.goGuo.transform.GetChild(i).GetChild(j).gameObject);
                }
                ObjectPool.Instance.CollectGameObject(room.goGuo.transform.GetChild(i).gameObject);
            }
        });
    }

    /// <summary>
    /// �������İ�ť�Ϳ�����
    /// </summary>
    /// <returns>������</returns>
    private GameObject CreateGuoAndEmpty(short param, object o, short opcode=(short)OpCode.PlayTileDataReturn)
    {
        // �������İ�ť
        CreateButton(4, room.goGuo.transform,  param, o, opcode);
        // ����������
        GameObject prefabEmpty = GameResources.Instance.prefabEmpty;
        GameObject goEmpty = ObjectPool.Instance.GetGameObject(prefabEmpty.name, prefabEmpty);
        goEmpty.transform.SetParent(room.goChiPengGangHu.transform, false);
        goEmpty.transform.localPosition = new Vector3(0, (room.goChiPengGangHu.transform.childCount - 1) * 120, 0);
        return goEmpty;
    }

    /// <summary>
    /// ������������
    /// </summary>
    /// <param name="tileList"></param>
    private void CreateHandTile(List<int> tileList)
    {
        int n = room.goHandTile[0].transform.childCount;
        for(int i = n - 1; i >= 0; --i)
        {
            GameObject goTile = room.goHandTile[0].transform.GetChild(i).gameObject;
            ObjectPool.Instance.CollectGameObject(goTile);
        }
        //  �Ӵ�С���ɲ�����
        tileList.Sort();
        tileList.Reverse();
        
        for (int i = 0; i < tileList.Count; ++i)
        {
            int tileNum = tileList[i];
            GameObject prefabTile = GetTile(tileNum, GameResources.Instance.prefabHandTileList[0]);
            GameObject tileOwn = ObjectPool.Instance.GetGameObject(prefabTile.name, prefabTile);
            tileOwn.transform.SetParent(room.goHandTile[0].transform, false);
            tileOwn.transform.localPosition = new Vector3(-115f * i, 0, 0);
            // ��ӿ����ƵĽű�
            tileOwn.TryGetComponent<Tile>(out Tile tile);
            if (tile == null) tile = tileOwn.AddComponent<Tile>();
            tile.tileNum = tileNum;
            tile.isDrawTile = false;
            if (tileOwn.GetComponent<ClickTileController>() == null)
                tileOwn.AddComponent<ClickTileController>();
        }
    }

    /// <summary>
    /// ������Ϸ����ʱ����
    /// </summary>
    /// <param name="huTile"></param>
    /// <param name="playerIndex"></param>
    private void CreateOverTile(List<int>huTile, int playerIndex)
    {
        int n;
        if(playerIndex == 0)
        {
            n = room.goHandTile[playerIndex].transform.childCount;
            // ɾ������
            for (int k = n - 1; k >= 0; --k)
            {
                ObjectPool.Instance.CollectGameObject(room.goHandTile[playerIndex].transform.GetChild(k).gameObject);
            }
            // ɾ������
            n = room.goDrawTile[playerIndex].transform.childCount;
            if(n > 0)
                ObjectPool.Instance.CollectGameObject(room.goDrawTile[playerIndex].transform.GetChild(0).gameObject);
        }
        else
        {
            if(room.goHandTile[playerIndex].transform.childCount > 0)
            {
                n = room.goHandTile[playerIndex].transform.GetChild(0).childCount;
                // ɾ������
                for (int k = n - 1; k >= 0; --k)
                {
                    ObjectPool.Instance.CollectGameObject
                        (room.goHandTile[playerIndex].transform.GetChild(0).GetChild(k).gameObject);
                }
                ObjectPool.Instance.CollectGameObject(room.goHandTile[playerIndex].transform.GetChild(0).gameObject);
            }

            // ��������
            room.goDrawTile[playerIndex].gameObject.SetActive(false);
        }
      
       
        
        // ɾ����������
        for (int k = room.goCPGArea[playerIndex].transform.childCount - 1; k >= 0; --k)
        {
            n = room.goCPGArea[playerIndex].transform.GetChild(k).childCount;
            for(int i = n - 1; i >= 0; --i)
            {
                ObjectPool.Instance.CollectGameObject
                    (room.goCPGArea[playerIndex].transform.GetChild(k).GetChild(i).gameObject);
            }
            ObjectPool.Instance.CollectGameObject(room.goCPGArea[playerIndex].transform.GetChild(k).gameObject);
        }
        // �������ɺ���
        huTile.Sort();
        huTile.Reverse();
        for(int i = 0; i < huTile.Count; ++i)
        {
            int tileNum = huTile[i];
            GameObject prefabTile = GetTile(tileNum, GameResources.Instance.prefabThrowTileList[playerIndex]);
            GameObject goTile = ObjectPool.Instance.GetGameObject(prefabTile.name, prefabTile);
            goTile.transform.SetParent(room.goCPGArea[playerIndex].transform, false);
            if (playerIndex == 0)
            {
                goTile.transform.localPosition = 
                    new Vector3((huTile.Count - 1 - room.goCPGArea[playerIndex].transform.childCount) * 70, 0, 0);
            }
            else if (playerIndex == 1)
            {
                goTile.transform.localPosition = new Vector3(0, 
                    (huTile.Count - 1 - room.goCPGArea[playerIndex].transform.childCount) * 50, 0);
            }
            else if (playerIndex == 2)
            {
                goTile.transform.localPosition = new Vector3((room.goCPGArea[playerIndex].transform.childCount - 1) * (-70), 0, 0);
            }
            else if (playerIndex == 3)
            {
                goTile.transform.localPosition = new Vector3(0, (room.goCPGArea[playerIndex].transform.childCount - 1) * (-50), 0);
            }
        }
    }

    /// <summary>
    /// �����Ƶ����ֻ�ȡ�Ƶ�����
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public TileType GetTileType(int num)
    {
        if (num >= (int)TileTypeTileNum.bamboo1 && num <= (int)TileTypeTileNum.bamboo9) return TileType.bamboo;
        else if (num >= (int)TileTypeTileNum.circle1 && num <= (int)TileTypeTileNum.circle9) return TileType.circle;
        else if (num >= (int)TileTypeTileNum.character1 && num <= (int)TileTypeTileNum.character9) return TileType.character;
        else if (num == (int)TileTypeTileNum.wind1 || num == (int)TileTypeTileNum.wind2
            || num == (int)TileTypeTileNum.wind3 || num == (int)TileTypeTileNum.wind4
            || num == (int)TileTypeTileNum.RedDragon || num == (int)TileTypeTileNum.GreenDragon
            || num == (int)TileTypeTileNum.WhiteDragon) return TileType.honor;
        else return TileType.honor;
    }
}
 
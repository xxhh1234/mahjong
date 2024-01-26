using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    // 游戏信息
    #region
    public Peer peerClient = new Peer();
    /// <summary>
    /// 主相机实例
    /// </summary>
    public GameObject goMainCamera;
    /// <summary>
    /// 登录界面实例
    /// </summary>
    public GameObject goLoginPanel;
    /// <summary>
    /// 日志界面
    /// </summary>
    public GameObject goLogPanel;
    /// <summary>
    /// 大厅界面实例
    /// </summary>
    public GameObject goLobbyPanel;
    /// <summary>
    /// 房间界面实例
    /// </summary>
    public GameObject goRoomPanel;
    /// <summary>
    /// room实例
    /// </summary>
    public RoomPanel room;
    /// <summary>
    /// 四个玩家的位置
    /// </summary>
    public List<PlayerPosition> playerPositions = new List<PlayerPosition>
    { PlayerPosition.east, PlayerPosition.south, PlayerPosition.west, PlayerPosition.north };
    /// <summary>
    /// 手牌的移动速度
    /// </summary>
    public float tileMoveSpeed = 20.0f;
    #endregion

    /// <summary>
    /// 加载所有资源，并实例化主相机和登录界面(日志界面)
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
    /// 本家登录
    /// </summary>
    /// <param name="userInfo">用户信息</param>
    public void Login(UserInfo userInfo)
    {
       StartCoroutine(IELogin(userInfo));
    }
    IEnumerator IELogin(UserInfo userInfo)
    {
        // 1.关闭登录界面并初始化游戏资源
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
        // 2. 打开大厅界面并初始化
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
    /// 本家创建房间界面
    /// </summary>
    /// <param name="player">创建房间玩家</param>
    /// <param name="RoomInfo">房间信息</param>
    public void CreateRoomPanel(Player player, RoomInfo roomInfo)
    {
        // 1.关闭大厅界面
        ObjectPool.Instance.CollectGameObject(goLobbyPanel);
        goLobbyPanel.GetComponent<LobbyPanel>().goCreateRoomPanel.SetActive(false);
        // 2.播放创建房间的声音
        
        // 3.打开房间界面并且初始化
        goRoomPanel = ObjectPool.Instance.GetGameObject("RoomPanel", GameResources.Instance.prefabRoomPanel);
        // 4.设置房间信息： 1. 房间名称 2. 房间当前局数/房间总局数 3. 房间中控图片
        room = goRoomPanel.GetComponent<RoomPanel>();
        InitRoom();
        room.txtRoomName.text = roomInfo.roomName.ToString();
        room.imgController.sprite = GameResources.Instance.spriteController[0];
        // 5.设置本家属性： 1.玩家位置 2.玩家Id 3.玩家名称 4.玩家分数 5.玩家性别 6.玩家是否准备 
        room.playerProperties[0].playerPos = PlayerPosition.east;
        room.playerProperties[0].actorId = player.actorId;
        room.playerProperties[0].Username = "昵称:" + player.username;
        room.playerProperties[0].Score = "分数:" + player.score;
        room.playerProperties[0].Sex = player.sex;
        room.playerProperties[0].IsReady = player.isReady;
        room.playerProperties[0].IsMaster = player.isMaster;
        // 6.更新游戏状态
        
    }

    /// <summary>
    /// 本家加入房间
    /// </summary>
    /// <param name="player">加入玩家</param>
    /// <param name="RoomInfo">房间信息</param>
    /// <param name="playerList">房间里的其他玩家</param>
    public void JoinRoomPanel(Player player, RoomInfo roomInfo, List<Player> playerList)
    {
        // 1.关闭大厅界面
        goLobbyPanel.SetActive(false);
        goLobbyPanel.GetComponent<LobbyPanel>().goJoinRoomPanel.SetActive(false);
        // 2.播放加入房间声音

        // 3.显示房间界面并且初始化
        goRoomPanel = ObjectPool.Instance.GetGameObject("RoomPanel", GameResources.Instance.prefabRoomPanel);
        // 4.设置房间信息： 1.房间名称 2.房间中控图片 
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
        // m代表本家位置的值
        int m = (int)player.playerPos; 
        for (int i = 0; i < playerList.Count; ++i)
        {
            int n = (int)playerList[i].playerPos;
            int j = n - m +  (n - m < 0 ? 4 : 0);
            room.playerProperties[j].playerPos = playerList[i].playerPos;
            room.playerProperties[j].actorId = playerList[i].actorId;
            if(j == 0 || j == 2)
            {
                room.playerProperties[j].Username = "昵称:" + playerList[i].username;
                room.playerProperties[j].Score = "分数:" + playerList[i].score;
            }
            else
            {
                room.playerProperties[j].Username = "昵称:" +  "\n" + playerList[i].username;
                room.playerProperties[j].Score = "分数:" + "\n" + playerList[i].score;
            }
            room.playerProperties[j].Sex = playerList[i].sex;
            room.playerProperties[j].IsReady = playerList[i].isReady;   
            room.playerProperties[j].IsMaster = playerList[i].isMaster;
        }
        // 6.更新游戏状态
    }

    /// <summary>
    /// 其他玩家加入房间
    /// </summary>
    /// <param name="otherPlayer">其他玩家</param>
    /// <param name="RoomInfo">房间信息</param>
    public void OtherJoinRoomPanel(Player otherPlayer, RoomInfo roomInfo)
    {
        // 1.播放其他玩家加入房间的声音

        // 2.设置其他加入玩家的属性：1.玩家位置 2.玩家Id 3.玩家名称 4.玩家分数或金币 5.玩家性别 6.玩家是否准备 
        // m代表本家位置的值
        int m = (int)room.playerProperties[0].playerPos; 
        // n代表其他玩家位置的值
        int n = (int)otherPlayer.playerPos;
        int j = n - m + (n - m < 0 ? 4 : 0);
        room.playerProperties[j].playerPos = otherPlayer.playerPos;
        room.playerProperties[j].actorId = otherPlayer.actorId;
        if (j == 0 || j == 2)
        {
            room.playerProperties[j].Username = "昵称:" + otherPlayer.username;
            room.playerProperties[j].Score = "分数:" + otherPlayer.score;
        }
        else
        {
            room.playerProperties[j].Username = "昵称:" + "\n" + otherPlayer.username;
            room.playerProperties[j].Score = "分数:" + "\n" + otherPlayer.score;
        }
        room.playerProperties[j].Sex = otherPlayer.sex;
        room.playerProperties[j].IsReady = otherPlayer.isReady;
        room.playerProperties[j].IsMaster = otherPlayer.isMaster;
    }

    /// <summary>
    /// 本家离开房间
    /// </summary>
    public void LeaveRoomPanel()
    {
        // 1.隐藏RoomPanel界面并且显示LobbyPanel界面
        ObjectPool.Instance.CollectGameObject(goRoomPanel);
        goLobbyPanel = ObjectPool.Instance.GetGameObject("LobbyPanel", GameResources.Instance.prefabLobbyPanel);
    }

    /// <summary>
    /// 其他玩家离开房间
    /// </summary>
    /// <param name="leaveId">其他离开玩家的Id</param>
    public void OtherLeaveRoomPanel(int leaveId)
    {
        // 1.更新其他离开玩家属性: 1.玩家位置 2.玩家Id 3.玩家名称 4.玩家分数或金币 5.玩家性别 6.玩家是否准备 
        for (int i = 0; i < 4; ++i)
        {

            if (room.playerProperties[i].actorId == leaveId)
            {
                room.playerProperties[i].playerPos = playerPositions[0];
                room.playerProperties[i].actorId = 0;
                room.playerProperties[i].Username = "";
                room.playerProperties[i].Sex = 0;
                room.playerProperties[i].IsReady = false;
                // 是否是庄家设为false
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
    /// 本家点击准备
    /// </summary>
    public void ClickReady()
    {
        // 1.播放准备声音，并显示准备标志
        room.btnReady.gameObject.SetActive(false);
        room.playerProperties[0].IsReady = true;
        // 2.更新游戏状态
    }

    /// <summary>
    /// 其他玩家点击准备
    /// </summary>
    /// <param name="readyId">其他准备玩家的Id</param>
    public void OtherClickReady(int readyId)
    {
        // 1.显示其他玩家的准备标志
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
    /// 生成四个玩家的手牌
    /// </summary>
    /// <param name="currentNumber">当前局数</param>
    /// <param name="allNumber">总局数</param>
    /// <param name="remainTile">剩余牌数</param>
    /// <param name="tileList"></param>
    /// <param name="drawTile">摸牌数字</param>
    /// <param name="drawTileId">摸牌玩家Id</param>
    public void SendTile(int remainTile, List<int> tileList)
    {
        StartCoroutine(IESendTile(remainTile, tileList));
    }
    IEnumerator IESendTile(int remainTile, List<int> tileList)
    {
        // 1. 把离开房间按钮关闭
        room.btnLeave.gameObject.SetActive(false);
        // 2. 播放色子动画

        // 3. 等待1.5s后生成本家的手牌
        yield return new WaitForSeconds(1.5f);
        tileList.Sort();
        tileList.Reverse();
        //  从大到小生成并排列
        for (int i = 0; i < tileList.Count; ++i)
        {
            int tileNum = tileList[i];
            GameObject prefabTile = GetTile(tileNum, GameResources.Instance.prefabHandTileList[0]);
            GameObject tileOwn = ObjectPool.Instance.GetGameObject(prefabTile.name, prefabTile);
            tileOwn.transform.SetParent(room.goHandTile[0].transform, false);
            tileOwn.transform.localPosition = new Vector3(-115f * i, 0, 0);
            // 添加控制牌的脚本
            tileOwn.TryGetComponent<Tile>(out Tile tile);
            if(tile == null) tile = tileOwn.AddComponent<Tile>();
            tile.tileNum = tileNum;
            tile.isDrawTile = false;
            if(tileOwn.GetComponent<ClickTileController>() == null)
                tileOwn.AddComponent<ClickTileController>();
        }
        // 4. 生成下家，对家，上家的手牌
        for (int i = 1; i < 4; ++i)
        {
            GameObject prefabHandTile = GameResources.Instance.prefabHandTileList[i];
            GameObject goHandTile =  ObjectPool.Instance.GetGameObject(prefabHandTile.name, prefabHandTile);
            goHandTile.transform.SetParent(room.goHandTile[i].transform, false);
            goHandTile.transform.localPosition = Vector3.zero;
        }
        // 5. 把各自的勾取消
        for (int i = 0; i < room.playerProperties.Count; ++i)
        {
            room.playerProperties[i].IsReady = false;
        }
        // 6. 更新房间剩余牌数与游戏状态
        room.txtRemainTile.text = remainTile.ToString();
        // 7. 开启选择3张同花色手牌界面并开启倒计时
        RoomPanel.isCanSelectTile = true;
        room.SelectTilePanel.SetActive(true);
    }

    public void SelectTile(List<int>selectTileList)
    {
        // 1.向服务器请求其他玩家选中的3张牌
        Dictionary<short, object> dictSelectTile = new Dictionary<short, object>();
        dictSelectTile.Add(ParameterCode.selectTileList, RoomPanel.selectTileList);
        dictSelectTile.Add(ParameterCode.actorId, room.playerProperties[0].actorId);
        peerClient.SendRequest((short)OpCode.SelectTile, dictSelectTile);
        // 2. 获取非选中手牌的数字
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
        // 3. 重新生成手牌
        CreateHandTile(tileList);
    }

    public void AddThreeTile(List<int>selectTileList)
    {
        StartCoroutine(IESelectTileMove(selectTileList));
    }

    public void  OtherDingQue(TileType queTileType, int queId)
    {
        // 1.显示其他玩家的定缺花色
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
    /// 生成四个玩家的抛牌
    /// </summary>
    /// <param name="isDrawTile">是否摸牌</param>
    /// <param name="tileNum">抛牌数字</param>
    public void ThrowTile(bool isDrawTile, int tileNum)
    {
        // 1.生成本家抛牌
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
        // 2.显示提示箭头
        room.goArrow.transform.SetParent(goThrowTile.transform, false);
        room.goArrow.transform.localPosition = new Vector3(0, 67, 0);
        // 3.如果此时吃碰杠胡上面有按钮就删除
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
        // 4.如果过上面有按钮就删除
        for (int i = n - 1; i >= 0; --i)
        {
            int m = room.goGuo.transform.GetChild(i).childCount;
            for (int j = m - 1; j >= 0; --j)
            {
                ObjectPool.Instance.CollectGameObject(room.goGuo.transform.GetChild(i).GetChild(j).gameObject);
            }
            ObjectPool.Instance.CollectGameObject(room.goGuo.transform.GetChild(i).gameObject);
        }

        // 5.如果isDrawTile为true就删除摸牌，否则将摸牌整理进手牌 
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
    /// 根据出牌数据更新出牌玩家的信息
    /// </summary>
    /// <param name="playTileNum">出牌数字</param>
    /// <param name="playTileId">出牌玩家Id</param>
    /// <param name="otherPlayTileType">其他玩家出牌以后胡牌的类型</param>
    /// <param name="PengGangType">吃碰杠类型</param>
    /// <param name="chiTileList">吃牌列表</param>
    public void PlayTileData(int playTileNum, int playTileId,
                                OtherPlayTileType otherPlayTileType, PengGangType PengGangType)
    {
        StartCoroutine(IEPlayTileData(playTileNum, playTileId, otherPlayTileType, PengGangType));
    }
     IEnumerator IEPlayTileData(int playTileNum, int playTileId,
                                OtherPlayTileType otherPlayTileType, PengGangType pengGangType)
    {
        // 1.生成过的按钮和胡的按钮
        if (!otherPlayTileType.Equals(OtherPlayTileType.buhu))
        {
            // 创建过的按钮和空物体并返回空物体
            GameObject goEmpty = CreateGuoAndEmpty(ParameterCode.otherPlayTileType, OtherPlayTileType.buhu);
            // 创建胡的按钮并挂载到goEmpty
            CreateButton(3, goEmpty.transform, ParameterCode.otherPlayTileType, otherPlayTileType);
        }
        // 2. 生成过的按钮和碰杠的按钮
        if (pengGangType.Equals(PengGangType.peng) || pengGangType.Equals(PengGangType.gang))
        {
            // 创建过的按钮和空物体并返回空物体
            GameObject goEmpty = CreateGuoAndEmpty(ParameterCode.PengGangType, PengGangType.none);
            // 创建碰的按钮并挂载到go_emptyObject
            CreateButton(1, goEmpty.transform, ParameterCode.PengGangType, PengGangType.peng);
            if (pengGangType.Equals(PengGangType.gang))
            {
                // 创建过的按钮和空物体并返回空物体
                GameObject goEmpty2 = CreateGuoAndEmpty(ParameterCode.PengGangType, PengGangType.none);
                // 创建杠的按钮并挂载到go_emptyObject
                CreateButton(2, goEmpty2.transform, ParameterCode.PengGangType, PengGangType.gang);
            }
        }
       
        // 4. 显示出牌玩家出牌
        for (int i = 0; i < room.playerProperties.Count; ++i)
        {
            if (playTileId == room.playerProperties[i].actorId)
            {
                // 播放出牌声音

                // 生成出牌提示区域并以手牌形式显示出牌
                
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
                // 摆放出牌位置，以抛牌形式显示出牌
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
                // 使箭头指向出牌
                room.goArrow.transform.SetParent(goPlayTile.transform, false);
                room.goArrow.transform.localPosition = new Vector3(0, 67, 0);
                // 取消出牌玩家的摸牌显示
                int n;
                if (i == 0)
                {
                    // 删除摸牌
                    n = room.goDrawTile[i].transform.childCount;
                    if (n > 0)
                        ObjectPool.Instance.CollectGameObject(room.goDrawTile[i].transform.GetChild(0).gameObject);
                }
                else
                {
                    // 隐藏摸牌
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
        // 1.播放胡的声音，生成胡的特效, 删除胡牌玩家的手牌，吃碰杠牌， 并以抛牌形式显示在吃碰杠区域
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
        // 2. 更新四个玩家的积分
        #region
        /*
        for (int i = 0; i < room.playerProperties.Count; ++i)
        {
            for (int j = 0; j < playerPositions.Count; ++j)
            {
                if (room.playerProperties[i].playerPos == playerPositions[j])
                {
                    room.playerProperties[i].Score = "分数:" + scoreList[j];
                }
            }
        }
        */
        #endregion
        // 3. 房间变为可出牌状态，更新游戏状态
        RoomPanel.isCanPlayTile = true;
    }

    /// <summary>
    /// 根据碰杠数据更新非出牌玩家碰杠牌的信息
    /// </summary>
    /// <param name="playTileNum">出牌数字</param>
    /// <param name="pengGangId">碰杠玩家Id</param>
    /// <param name="pengGangType">碰杠类型</param>
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
                    // 1.播放碰的声音

                    // 2.生成碰的特效并且在玩家提示区域显示
                    GameObject prefabEffectPeng = GameResources.Instance.prefabEffectList[1];
                    GameObject goEffectPeng = ObjectPool.Instance.GetGameObject(prefabEffectPeng.name, prefabEffectPeng);
                    goEffectPeng.transform.SetParent(room.goTipArea[i].transform, false);
                    goEffectPeng.transform.localPosition = Vector3.zero;
                    yield return new WaitForSeconds(2f);
                    ObjectPool.Instance.CollectGameObject(goEffectPeng);
                    // 3. 删除手牌中的两张碰牌，重新排序 
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
                    // 4. 玩家吃碰杠区域生成三张碰牌，房间变为可出牌状态
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
                    // 添加吃碰杠属性
                    goEmpty.TryGetComponent<CPGProperties>(out CPGProperties cpg);
                    if(cpg == null) cpg = goEmpty.AddComponent<CPGProperties>();
                    cpg.cpgType = PengGangType.peng;
                    cpg.cpgList = new List<int>() { playTileNum, playTileNum, playTileNum };
                    // 添加三张碰牌
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
                    // 1.播放杠的声音

                    // 2.生成杠的特效并且在玩家提示区域显示
                    GameObject prefabEffectGang = GameResources.Instance.prefabEffectList[2];
                    GameObject goEffectGang = ObjectPool.Instance.GetGameObject(prefabEffectGang.name, prefabEffectGang);
                    goEffectGang.transform.SetParent(room.goTipArea[i].transform, false);
                    goEffectGang.transform.localPosition = Vector3.zero;
                    yield return new WaitForSeconds(2f);
                    ObjectPool.Instance.CollectGameObject(goEffectGang);

                    // 3. 删除手牌中的三张杠牌，重新排序 
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
                    // 4. 玩家吃碰杠区域生成四张杠牌，房间变为可出牌状态
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
                    // 添加吃碰杠属性
                    goEmpty.TryGetComponent<CPGProperties>(out CPGProperties cpg);
                    if(cpg == null) cpg = goEmpty.AddComponent<CPGProperties>();
                    cpg.cpgType = PengGangType.gang;
                    cpg.cpgList = new List<int>() { playTileNum, playTileNum, playTileNum, playTileNum };
                    // 添加四张杠牌
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
    /// 根据摸牌数据更新摸牌玩家的信息
    /// </summary>
    /// <param name="remainTile">房间剩余牌数</param>
    /// <param name="drawTileNum">摸牌数字</param>
    /// <param name="isGangHouDrawTile">是否杠后摸牌</param>
    /// <param name="ziMoType">自摸类型</param>
    /// <param name="ShangHuaType">杠上开花类型</param>
    /// <param name="anGangJiaGangType">暗杠加杠类型</param>
    public void DrawTileData(int remainTile, int drawTileNum, bool isGangHouDrawTile,
                                ZiMoType ziMoType, ShangHuaType ShangHuaType, PengGangType anGangJiaGangType)
    {
        StartCoroutine(IEDrawTileData(remainTile, drawTileNum, isGangHouDrawTile,
                                    ziMoType, ShangHuaType, anGangJiaGangType));
    }
    IEnumerator IEDrawTileData(int remainTile, int drawTileNum, bool isGangHouDrawTile,
                                 ZiMoType ziMoType, ShangHuaType shangHuaType, PengGangType anGangJiaGangType)
    {
        // 1.更新剩余牌数
        room.txtRemainTile.text = remainTile.ToString();
        // 2.生成摸牌
        GameObject prefabDrawTile = GetTile(drawTileNum, GameResources.Instance.prefabHandTileList[0]);
        GameObject goDrawTile = ObjectPool.Instance.GetGameObject(prefabDrawTile.name, prefabDrawTile);
        goDrawTile.transform.SetParent(room.goDrawTile[0].transform, false);
        goDrawTile.transform.localPosition = Vector3.zero;
        // 添加牌的控制脚本
        goDrawTile.TryGetComponent<Tile>(out Tile tile);
        if(tile == null) tile = goDrawTile.AddComponent<Tile>();
        tile.tileNum = drawTileNum;
        tile.isDrawTile = true;
        if(goDrawTile.GetComponent<ClickTileController>() == null)
            goDrawTile.AddComponent<ClickTileController>();
        room.goDrawTile[0].SetActive(true);
        // 3. 隐藏其他玩家的摸牌
        for (int i = 1; i < 4; ++i)
        {
            room.goDrawTile[i].SetActive(false);
        }
        yield return new WaitForSeconds(0.2f);
        // 4.更新房间是否杠后出牌， 是否能出牌, 并生成自摸上花按钮， 如果暗杠抢杠，直接向服务器发送摸牌返回数据
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
                // 创建过的按钮和空物体并返回空物体
                GameObject goEmpty = CreateGuoAndEmpty(ParameterCode.ziMoType, ZiMoType.buzimo,
                                                                            (short)OpCode.DrawTileDataReturn);
                // 创建自摸的按钮并挂载到goEmpty
                CreateButton(5, goEmpty.transform, ParameterCode.ziMoType, ziMoType, (short)OpCode.DrawTileDataReturn);
            }
            else if (!shangHuaType.Equals(ShangHuaType.bushanghua))
            {
                // 创建过的按钮和空物体并返回空物体
                GameObject goEmpty = CreateGuoAndEmpty(ParameterCode.shangHuaType, ShangHuaType.bushanghua,
                                                                               (short)OpCode.DrawTileDataReturn);
                // 创建杠上开花的按钮并挂载到goEmpty
                CreateButton(6, goEmpty.transform, ParameterCode.shangHuaType, shangHuaType,
                                                                                    (short)OpCode.DrawTileDataReturn);
            }
            else if (!anGangJiaGangType.Equals(PengGangType.none))
            {
                // 创建过的按钮和空物体并返回空物体
                GameObject goEmpty = CreateGuoAndEmpty(ParameterCode.PengGangType, PengGangType.none,
                                                                                  (short)OpCode.DrawTileDataReturn);
                // 创建杠的按钮并挂载到goEmpty
                CreateButton(2, goEmpty.transform, ParameterCode.PengGangType, anGangJiaGangType,
                                                                                    (short)OpCode.DrawTileDataReturn);
            }
        }
        #endregion
    }

    /// <summary>
    /// 根据摸牌数据更新其他摸牌玩家信息
    /// </summary>
    /// <param name="remainTile">剩余牌数</param>
    /// <param name="drawTileId">摸牌玩家Id</param>
    public void OtherDrawTileData(int remainTile, int drawTileId)
    {
        // 1.更新房间剩余牌数
        room.txtRemainTile.text = remainTile.ToString();
        // 2. 显示其他摸牌玩家的摸牌
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
    /// 根据自摸数据更新所有玩家的数据
    /// </summary>
    /// <param name="ziMoType">自摸类型</param>
    /// <param name="ziMoId">自摸玩家Id</param>
    /// <param name="ziMoNum">自摸数字</param>
    /// <param name="handTileList">所有玩家的手牌</param>
    /// <param name="scoreList">分数列表</param>
    /// <param name="coinList">金币列表</param>
    /// <param name="RoomInfo">房间信息</param>
    public void ZiMoData(ZiMoType ziMoType, int ziMoId, int ziMoNum,
                     List<int> ziMoHandTile, List<int> scoreList)
    {
        StartCoroutine(IEZiMoData(ziMoType, ziMoId, ziMoNum, ziMoHandTile, scoreList));
    }
    IEnumerator IEZiMoData(ZiMoType ziMoType, int ziMoId, int ziMoNum,
                    List<int> ziMoHandTile, List<int> scoreList)
    {
        // 1.播放自摸的声音，生成自摸的特效, 删除自摸玩家的手牌， 摸牌，吃碰杠牌， 并以抛牌形式显示在吃碰杠区域
        for (int i = 0; i < room.playerProperties.Count; ++i)
        {
            if (ziMoId == room.playerProperties[i].actorId)
            {
                LogPanel.Log(room.playerProperties[i].username + "自摸的牌: " + ziMoNum);
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
        // 2. 更新四个玩家的积分

        // 3. 房间变为可出牌状态，更新游戏状态
        RoomPanel.isCanPlayTile = true;
    }


    /// <summary>
    /// 根据杠上开花数据更新所有玩家的数据
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
        // 1.播放杠上开花的声音，生成杠上开花的特效, 删除杠上开花玩家的手牌，摸牌，吃碰杠牌， 并以抛牌形式显示在吃碰杠区域
        for (int i = 0; i < room.playerProperties.Count; ++i)
        {
            if (shangHuaId == room.playerProperties[i].actorId)
            {
                LogPanel.Log(room.playerProperties[i].username + "杠上开花的牌" + shangHuaNum);
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
        // 2. 更新四个玩家的积分

        // 3. 房间变为可出牌状态，更新游戏状态
        RoomPanel.isCanPlayTile = true;
    }

    /// <summary>
    /// 根据暗杠加杠数据更新所有玩家的数据
    /// </summary>
    /// <param name="gangTileType">杠牌类型</param>
    /// <param name="gangTileId">杠牌类型</param>
    /// <param name="gangTileNum">杠牌数字</param>
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
                // 1. 删除摸牌
                for (int k = room.goDrawTile[i].transform.childCount - 1; k >= 0; --k)
                {
                    ObjectPool.Instance.CollectGameObject(room.goDrawTile[i].transform.GetChild(k).gameObject);
                }
                // 2. 播放杠声音，生成杠特效

                GameObject prefabEffectGang = GameResources.Instance.prefabEffectList[2];
                GameObject goEffectGang = ObjectPool.Instance.GetGameObject(prefabEffectGang.name, prefabEffectGang);
                goEffectGang.transform.SetParent(room.goTipArea[i].transform, false);
                goEffectGang.transform.localPosition = Vector3.zero;
                yield return new WaitForSeconds(2f);
                ObjectPool.Instance.CollectGameObject(goEffectGang);
                // 3.生成杠牌
                LogPanel.Log(room.playerProperties[i].username + "暗杠加杠的牌"  + anGangJiaGangNum);
                if (anGangJiaGangType == PengGangType.angang)
                {
                    
                    // 删除手牌中的3张牌
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
                    // 添加四张牌到吃碰杠牌
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
                    // 添加吃碰杠属性
                    goEmpty.TryGetComponent<CPGProperties>(out CPGProperties cpg);
                    if (cpg == null) cpg = goEmpty.AddComponent<CPGProperties>();
                    cpg.cpgType = PengGangType.angang;
                    cpg.cpgList = new List<int>() 
                    { anGangJiaGangNum, anGangJiaGangNum, anGangJiaGangNum, anGangJiaGangNum};
                    
                    // 添加四张杠牌
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
                    // 添加一张牌到对应的碰牌，变为杠牌
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
                // 4.重新生成手牌
                if(i == 0)
                {
                    List<int> handTile = new List<int>();
                    for(int j = 0; j < room.goHandTile[i].transform.childCount; ++i)
                    {
                        handTile.Add(room.goHandTile[i].transform.GetChild(j).GetComponent<Tile>().tileNum);
                    }
                    CreateHandTile(handTile);
                }
             
                // 5.房间变为可出牌状态
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
            // 创建过的按钮和空物体并返回空物体
            GameObject goEmpty = CreateGuoAndEmpty(ParameterCode.otherPlayTileType, OtherPlayTileType.buhu, 
                                                                                         (short)OpCode.QiangGangDataReturn);
            // 创建抢杠的按钮并挂载到goEmpty
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
        // 1.播放抢杠胡的声音，生成抢杠胡的特效
        //   删除抢杠胡牌玩家的手牌，吃碰杠牌， 并以抛牌形式显示在吃碰杠区域
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
        // 2. 更新四个玩家的积分
        #region
        /*
        for (int i = 0; i < room.playerProperties.Count; ++i)
        {
            for (int j = 0; j < playerPositions.Count; ++j)
            {
                if (room.playerProperties[i].playerPos == playerPositions[j])
                {
                    room.playerProperties[i].Score = "分数:" + scoreList[j];
                }
            }
        }
        */
        #endregion
        // 3. 房间变为可出牌状态，更新游戏状态
        RoomPanel.isCanPlayTile = true;
    }


    public void GameOver(List<List<int>> noHuHandTileList, List<int> noHuPlayerIdList, List<int>scoreList)
    {
       StartCoroutine(IEGameOver(noHuHandTileList, noHuPlayerIdList, scoreList));
    }
    IEnumerator IEGameOver(List<List<int>> noHuHandTileList, List<int>noHuPlayerIdList, List<int> scoreList)
    {
        // 1. 删除未胡牌玩家的手牌，吃碰杠牌， 并以抛牌形式显示在吃碰杠区域

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
        // 2. 显示结算界面
        room.SettlePanel.SetActive(true);
        // 3. 显示胡牌玩家和输牌玩家的分数
        // 4. 显示房间离开按钮
        room.btnLeave.gameObject.SetActive(true);

    }

    /// <summary>
    /// 初始化房间
    /// </summary>
    public void InitRoom()
    {
        int n;
        if(!room.isFirstJoinRoom)
        {
            for (int i = 0; i < room.playerProperties.Count; ++i)
            {
                room.playerProperties[i].Score = "0";
                // 删除抛牌
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
                // 删除吃碰杠牌
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
                // 删除提示区域的物体
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
    /// 根据牌的数字获取牌的预制体
    /// </summary>
    /// <param name="tileNum">牌的数字</param>
    /// <param name="prefab">牌的预制体，例如手牌，抛牌</param>
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
        // 1.获取所有手牌数字
        List<int> tileList = new List<int>(selectTileList);
        for (int i = 0; i < room.goHandTile[0].transform.childCount; ++i)
        {
            tileList.Add(room.goHandTile[0].transform.GetChild(i).GetComponent<Tile>().tileNum);
        }
        // 2. 重新生成手牌
        CreateHandTile(tileList);
        LogPanel.Log("选牌后的手牌");
        for (int i = 0; i < room.goHandTile[0].transform.childCount; ++i)
        {
            LogPanel.Log(room.goHandTile[0].transform.GetChild(i).GetComponent<Tile>().tileNum.ToString());
        }
        yield return new WaitForSeconds(0.5f);
        // 3.开启选择定缺界面并开启倒计时
        room.DingQuePanel.SetActive(true);
    }

    /// <summary>
    /// 利用插值计算50个trans的位置到secondPos的位置，每0.01f移动摸牌一次，移动50次后到达secondPos,最后放到lastPos
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="secondPos"></param>
    /// <param name="lastPos"></param>
    /// <returns></returns>
    IEnumerator IEDrawTileMove(Transform trans, Vector3 secondPos, Vector3 lastPos)
    {
        // 1.计算50个差值的位置， 每0.01f移动一次
        for (int i = 0; i < 50; ++i)
        {
            trans.localPosition = Vector2.Lerp(trans.localPosition, secondPos, tileMoveSpeed * Time.deltaTime);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.2f);
        // 2. 0.2f后将摸牌移动到最后的位置
        trans.localPosition = lastPos;
    }
    /// <summary>
    /// 将摸到的牌整理进手牌
    /// </summary>
    /// <returns></returns>
    IEnumerator IEHandTileMove()
    {
        // 1.获取摸牌和摸牌数字
        Transform drawTileTrans = null;
        int drawTileNum = 0;
        if (room.goDrawTile[0].transform.childCount > 0)
        {
            drawTileTrans = room.goDrawTile[0].transform.GetChild(0);
            drawTileNum = drawTileTrans.GetComponent<Tile>().tileNum;
        }
        yield return new WaitForSeconds(0.2f);
        // 2. 获取手牌并添加摸牌
        List<int> handtile = new List<int>();
        for (int i = 0; i < room.goHandTile[0].transform.childCount; ++i)
        {
            handtile.Add(room.goHandTile[0].transform.GetChild(i).GetComponent<Tile>().tileNum);
        }
        if (drawTileNum > 0) handtile.Add(drawTileNum);
        // 从大到小排序
        handtile.Sort();
        handtile.Reverse();
        // 3.获取每张牌的位置，并从中移除摸牌的位置
        List<int> tileOrder = new List<int>();
        for (int i = 0; i < handtile.Count; ++i)
        {
            tileOrder.Add(i);
        }
        int drawTileIndex = handtile.IndexOf(drawTileNum);
        tileOrder.Remove(drawTileIndex);
        // 4.指定手牌位置
        for (int i = 0; i < room.goHandTile[0].transform.childCount; ++i)
        {
            room.goHandTile[0].transform.GetChild(i).transform.localPosition = new Vector3(-115 * tileOrder[i], 0, 0);
        }
        // 5.指定摸牌位置
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
    /// 创建按钮
    /// </summary>
    /// <param name="buttonIndex">按钮在prefab_buttons中的下标</param>
    /// <param name="parent">按钮的父物体</param>
    /// <param name="otherPlayTileResponse">点击按钮以后，给服务器的回复类型</param>
    private void CreateButton(int buttonIndex, Transform parent, 
                                        short param,  object o, short opcode=(short)OpCode.PlayTileDataReturn)
    {
        GameObject prefab = GameResources.Instance.prefabButtonList[buttonIndex];
        GameObject go = ObjectPool.Instance.GetGameObject(prefab.name, prefab);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(0, (parent.childCount - 1) * 120, 0);
        go.GetComponent<Button>().onClick.AddListener(() =>
        {
            // 播放声音

            // 向服务器请求出牌数据回复
            Dictionary<short, object> dict = new Dictionary<short, object>();
            if(o != null) dict.Add(param, o);
            peerClient.SendRequest(opcode, dict);
            // 删除所有按钮
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
    /// 创建过的按钮和空物体
    /// </summary>
    /// <returns>空物体</returns>
    private GameObject CreateGuoAndEmpty(short param, object o, short opcode=(short)OpCode.PlayTileDataReturn)
    {
        // 创建过的按钮
        CreateButton(4, room.goGuo.transform,  param, o, opcode);
        // 创建空物体
        GameObject prefabEmpty = GameResources.Instance.prefabEmpty;
        GameObject goEmpty = ObjectPool.Instance.GetGameObject(prefabEmpty.name, prefabEmpty);
        goEmpty.transform.SetParent(room.goChiPengGangHu.transform, false);
        goEmpty.transform.localPosition = new Vector3(0, (room.goChiPengGangHu.transform.childCount - 1) * 120, 0);
        return goEmpty;
    }

    /// <summary>
    /// 创建本家手牌
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
        //  从大到小生成并排列
        tileList.Sort();
        tileList.Reverse();
        
        for (int i = 0; i < tileList.Count; ++i)
        {
            int tileNum = tileList[i];
            GameObject prefabTile = GetTile(tileNum, GameResources.Instance.prefabHandTileList[0]);
            GameObject tileOwn = ObjectPool.Instance.GetGameObject(prefabTile.name, prefabTile);
            tileOwn.transform.SetParent(room.goHandTile[0].transform, false);
            tileOwn.transform.localPosition = new Vector3(-115f * i, 0, 0);
            // 添加控制牌的脚本
            tileOwn.TryGetComponent<Tile>(out Tile tile);
            if (tile == null) tile = tileOwn.AddComponent<Tile>();
            tile.tileNum = tileNum;
            tile.isDrawTile = false;
            if (tileOwn.GetComponent<ClickTileController>() == null)
                tileOwn.AddComponent<ClickTileController>();
        }
    }

    /// <summary>
    /// 创建游戏结束时的牌
    /// </summary>
    /// <param name="huTile"></param>
    /// <param name="playerIndex"></param>
    private void CreateOverTile(List<int>huTile, int playerIndex)
    {
        int n;
        if(playerIndex == 0)
        {
            n = room.goHandTile[playerIndex].transform.childCount;
            // 删除手牌
            for (int k = n - 1; k >= 0; --k)
            {
                ObjectPool.Instance.CollectGameObject(room.goHandTile[playerIndex].transform.GetChild(k).gameObject);
            }
            // 删除摸牌
            n = room.goDrawTile[playerIndex].transform.childCount;
            if(n > 0)
                ObjectPool.Instance.CollectGameObject(room.goDrawTile[playerIndex].transform.GetChild(0).gameObject);
        }
        else
        {
            if(room.goHandTile[playerIndex].transform.childCount > 0)
            {
                n = room.goHandTile[playerIndex].transform.GetChild(0).childCount;
                // 删除手牌
                for (int k = n - 1; k >= 0; --k)
                {
                    ObjectPool.Instance.CollectGameObject
                        (room.goHandTile[playerIndex].transform.GetChild(0).GetChild(k).gameObject);
                }
                ObjectPool.Instance.CollectGameObject(room.goHandTile[playerIndex].transform.GetChild(0).gameObject);
            }

            // 隐藏摸牌
            room.goDrawTile[playerIndex].gameObject.SetActive(false);
        }
      
       
        
        // 删除吃碰杠牌
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
        // 重新生成胡牌
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
    /// 根据牌的数字获取牌的类型
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
 
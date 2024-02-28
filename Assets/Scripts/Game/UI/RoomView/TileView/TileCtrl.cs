using XH;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace mahjong
{
    class TileCtrl : Ctrl
    {
        public override void Init(string name)
        {
            base.Init(name);

            EventManager.Instance.AddListener(ServerToClient.SendTileToAll, (Dictionary<short, object> eventDict) =>
            {
                ThreadManager.ExecuteUpdate(() => { GameManager.Instance.StartCoroutine(IESendTile(eventDict)); });
            });
            EventManager.Instance.AddListener(ServerToClient.SendTimerToAll, (Dictionary<short, object> eventDict) =>
            {
                PlayTimerAnim(eventDict);
            });
            EventManager.Instance.AddListener(ServerToClient.SendPlayTileDataToOther, (Dictionary<short, object> eventDict) =>
            {
                OnPlayTileDataReturn(eventDict);
            });
            EventManager.Instance.AddListener(ServerToClient.SendToNextDrawTile, (Dictionary<short, object> eventDict) =>
            {
                OnDrawTileData(eventDict);
            });
            EventManager.Instance.AddListener(ServerToClient.SendDrawTileDataToOther, (Dictionary<short, object> eventDict) =>
            {
                OnOtherDrawTileData(eventDict);
            });
        }

        public override void UnInit()
        {
            base.UnInit();

            EventManager.Instance.RemoveListener(ServerToClient.SendTileToAll, (Dictionary<short, object> eventDict) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    GameManager.Instance.StartCoroutine(IESendTile(eventDict));
                });
            });
            EventManager.Instance.RemoveListener(ServerToClient.SendTimerToAll, (Dictionary<short, object> eventDict) =>
            {
                PlayTimerAnim(eventDict);
            });
            EventManager.Instance.RemoveListener(ServerToClient.SendPlayTileDataToOther, (Dictionary<short, object> eventDict) =>
            {
                OnPlayTileDataReturn(eventDict);
            });
            EventManager.Instance.RemoveListener(ServerToClient.SendToNextDrawTile, (Dictionary<short, object> eventDict) =>
            {
                OnDrawTileData(eventDict);
            });
            EventManager.Instance.RemoveListener(ServerToClient.SendDrawTileDataToOther, (Dictionary<short, object> eventDict) =>
            {
                OnOtherDrawTileData(eventDict);
            });
        }

        public IEnumerator IESendTile(Dictionary<short, object> eventDict)
        {
            // 1. 解析数据
            RoomData roomData = Util.Convert<RoomData>(eventDict[ParameterCode.RoomData]);
            HandTileData handTileData = Util.Convert<HandTileData>(eventDict[ParameterCode.HandTileData]);
            // 2. 更新房间剩余牌数,房间状态, 是否能选中牌并通知RoomModel刷新RoomView
            ClientDataManager.Instance.RefreshData("RoomData", roomData, DataEvent.ChangeRoomData);
            // 3. 播放色子动画并播放声音
            EventManager.Instance.Broadcast(UIEvent.ShowDiceAnim, true, false);
            AudioManager.Instance.PlayAudio("audio_shaizi");
            // 4. 等待1.5s，关闭色子动画，播放游戏开始的声音
            yield return new WaitForSeconds(1.5f);
            EventManager.Instance.Broadcast(UIEvent.ShowDiceAnim, false, false);
            AudioManager.Instance.PlayAudio("game_p_start");
            // 5. 取消对钩并通知PlayerModel刷新PlayerView, 通知TileModel刷新TileView
            PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
            for (int i = 0; i < playerData.playerCnt; ++i)
            {
                playerData.playerDatas[i].isReady = false;
                handTileData.handTileId = (ushort)i;
                ClientDataManager.Instance.RefreshData("HandTileData", handTileData, DataEvent.ChangeHandTileData);
                if(i == 0)
                {
                    string str = "";
                    for(int j = 0; j < handTileData.tileCnt; ++j) 
                        str += ((TileTypeTileNum)handTileData.handTile[j]).ToString() + " ";
                    XH.Logger.XH_LOG("13张手牌为" + str);
                }
            }
            ClientDataManager.Instance.RefreshData("PlayerData", playerData, DataEvent.ChangePlayerData);
            // 7. 开启SelectTileView并开启倒计时
            SelectTileData selectTileData = (SelectTileData)ClientDataManager.Instance.GetData("SelectTileData");
            selectTileData.isOnSelectTile = true;
            ClientDataManager.Instance.RefreshData("SelectTileData", selectTileData);
            View roomView = UIManager.Instance.GetView("RoomView");
            UIManager.Instance.ShowView("SelectTileView", roomView.gameObject.transform);
        }
        public void PlayTimerAnim(Dictionary<short, object> eventDict)
        {
            TimerData timerData = Util.Convert<TimerData>(eventDict[ParameterCode.TimerData]);
            PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
            for(int i = 0; i < playerData.playerCnt; ++i)
            {
                if (playerData.playerDatas[i].playerId == timerData.timerId)
                {
                    timerData.timerId = (ushort)i;
                    EventManager.Instance.Broadcast(UIEvent.ShowTimerAnim, timerData as ValueType);

                    break;
                }
            }
        }
        public void OnPlayTileDataReturn(Dictionary<short, object> eventDict)
        {
            // 1. 解析出牌返回数据
            PlayTileReturnData playTileReturnData = Util.Convert<PlayTileReturnData>(eventDict[ParameterCode.PlayTileReturnData]);
            HuTileType huTileType = playTileReturnData.huTileType;
            PengGangType pengGangType = playTileReturnData.pengGangType;
            ushort playTileId = playTileReturnData.playTileId;
            ushort playTileNum = playTileReturnData.playTileNum;
            if (huTileType.Equals(HuTileType.buhu) && pengGangType.Equals(PengGangType.none))
            {
                Dictionary<short, object> dictPlayTileDataReturn = new Dictionary<short, object>();
                dictPlayTileDataReturn.Add(ParameterCode.none, null);
                NetManager.peerClient.SendRequestAsync(ClientToServer.PlayTileDataReturn, dictPlayTileDataReturn);
            }
            // 2. 刷新按钮数据
            ButtonData buttonData;
            if (!huTileType.Equals(HuTileType.buhu))
            {
                buttonData = new ButtonData(3, ParameterCode.HuTileType, huTileType, ClientToServer.PlayTileDataReturn);
                ClientDataManager.Instance.RefreshData("ButtonData", buttonData, DataEvent.ChangeButtonData);
            }
            if (pengGangType.Equals(PengGangType.peng) || pengGangType.Equals(PengGangType.gang))
            {
                buttonData = new ButtonData(1, ParameterCode.PengGangType, pengGangType, ClientToServer.PlayTileDataReturn);
                ClientDataManager.Instance.RefreshData("ButtonData", buttonData, DataEvent.ChangeButtonData);
                if (pengGangType.Equals(PengGangType.gang))
                {
                    buttonData = new ButtonData(2, ParameterCode.PengGangType, pengGangType, 
                        ClientToServer.PlayTileDataReturn);
                    ClientDataManager.Instance.RefreshData("ButtonData", buttonData, DataEvent.ChangeButtonData);
                }
            }
            SinglePlayerData[] playerDatas = ((PlayerData)ClientDataManager.Instance.GetData("PlayerData")).playerDatas;
            PlayTileData playTileData = (PlayTileData)ClientDataManager.Instance.GetData("PlayTileData");
            for (int i = 1; i < playerDatas.Length; ++i)
            {
                if (playTileId == playerDatas[i].playerId)
                {
                    // 3. 播放出牌声音

                    // 4. 更新出牌数据, 刷新提示数据，以手牌形式显示出牌
                    playTileData.playTileIndex = (ushort)i;
                    ClientDataManager.Instance.RefreshData("PlayTileData", playTileData);

                    TipData tipData = new TipData();
                    tipData.isTile = true;
                    tipData.tipId = (ushort)i;
                    tipData.tipNum = playTileNum;
                    tipData.isOnce = true;
                    ClientDataManager.Instance.RefreshData("TipData", tipData, DataEvent.ChangeTipData);
                    // 5. 删除摸牌, 刷新抛牌数据，以抛牌形式显示出牌
                    ClientDataManager.Instance.RefreshData("DrawTileData", new DrawTileData((ushort)i, 0),
                        DataEvent.ChangeDrawTileData);
                    ClientDataManager.Instance.RefreshData("ThrowTileData", new ThrowTileData((ushort)i, playTileNum), 
                            DataEvent.ChangeThrowTileData);
                    break;
                }
            }
        }
        public void OnDrawTileData(Dictionary<short, object> eventDict)
        {
            NetManager.peerClient.SendRequestAsync(ClientToServer.DrawTile, eventDict, (Dictionary<short, object> respDict) =>
            {
                // 1. 解析摸牌返回数据
                DrawTileReturnData data = Util.Convert<DrawTileReturnData>(respDict[ParameterCode.DrawTileReturnData]);
                uint remainTile = data.remainTile;
                ushort drawTileNum = data.drawTileNum;
                ushort drawTileId = data.drawTileId;
                bool isGangHouDrawTile = data.isGangHouDrawTile;
                ZiMoType ziMoType = data.ziMoType;
                ShangHuaType shangHuaType = data.shangHuaType;
                PengGangType anGangJiaGangType = data.anGangJiaGangType;

                // 2. 更新房间数据
                RoomData roomData = (RoomData)ClientDataManager.Instance.GetData("RoomData");
                roomData.remainTile = remainTile;
                ClientDataManager.Instance.RefreshData("RoomData", roomData, DataEvent.ChangeRoomData);
                // 3. 更新摸牌数据
                PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
                for (int i = 1; i < playerData.playerCnt; ++i)
                {
                    ClientDataManager.Instance.RefreshData("DrawTileData", new DrawTileData((ushort)i, 0), 
                        DataEvent.ChangeDrawTileData);
                }
                ClientDataManager.Instance.RefreshData("DrawTileData", new DrawTileData(0, drawTileNum),
                        DataEvent.ChangeDrawTileData);
                // 4. 更新出牌数据
                PlayTileData playTileData = (PlayTileData)ClientDataManager.Instance.GetData("PlayTileData");
                playTileData.isGangHouPlayTile = isGangHouDrawTile;
                if (ziMoType.Equals(ZiMoType.buzimo) && shangHuaType.Equals(ShangHuaType.bushanghua)
                                                && anGangJiaGangType.Equals(PengGangType.none))
                {
                    playTileData.isCanPlayTile = true;
                    ClientDataManager.Instance.RefreshData("PlayTileData", playTileData);
                    return;
                }
                playTileData.isCanPlayTile = false;
                ClientDataManager.Instance.RefreshData("PlayTileData", playTileData);

                // 5. 刷新按钮数据
                ButtonData buttonData;
                if (!ziMoType.Equals(ZiMoType.buzimo))
                {
                    buttonData = new ButtonData(5, ParameterCode.ZiMoType, ziMoType, ClientToServer.DrawTileDataReturn);
                    ClientDataManager.Instance.RefreshData("ButtonData", buttonData, DataEvent.ChangeButtonData);

                    return;
                }
                if (!shangHuaType.Equals(ShangHuaType.bushanghua))
                {
                    buttonData = new ButtonData(6, ParameterCode.ShangHuaType, shangHuaType, ClientToServer.DrawTileDataReturn);
                    ClientDataManager.Instance.RefreshData("ButtonData", buttonData, DataEvent.ChangeButtonData);

                    return;
                }
                if (!anGangJiaGangType.Equals(PengGangType.none))
                {
                    buttonData = new ButtonData(2, ParameterCode.PengGangType, anGangJiaGangType, ClientToServer.DrawTileDataReturn);
                    ClientDataManager.Instance.RefreshData("ButtonData", buttonData, DataEvent.ChangeButtonData);

                    return;
                }
            });
        }
        public void OnOtherDrawTileData(Dictionary<short, object> eventDict)
        {
            DrawTileData drawTileData = Util.Convert<DrawTileData>(eventDict[ParameterCode.DrawTileData]);
            // 1. 通知RoomModel刷新RoomView
            RoomData roomData = (RoomData)ClientDataManager.Instance.GetData("RoomData");
            roomData.remainTile = drawTileData.drawTileNum;
            ClientDataManager.Instance.RefreshData("RoomData", roomData, DataEvent.ChangeRoomData);
            // 2. 通知TileModel刷新TileView
            PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
            ushort drawTileId = drawTileData.drawTileId;
            for (int i = 1; i < playerData.playerCnt; ++i)
            {
                ClientDataManager.Instance.RefreshData("DrawTileData", new DrawTileData((ushort)i,
                    (ushort)(playerData.playerDatas[i].playerId == drawTileId ? 1 : 0)),
                    DataEvent.ChangeDrawTileData);
            }
        }

        private void InitTile(GameObject tile, ushort tileNum, bool isADrawTile, TileType queTileType)
        {
            tile.GetComponent<SpriteRenderer>().color = GetTileType(tileNum).Equals(queTileType) 
                ? new Color(127 / 255f, 127 / 255f, 127 / 255f) : new Color(1f, 1f, 1f);
            if(!tile.TryGetComponent(out EventTrigger eventTrigger))
            { 
                eventTrigger = tile.AddComponent<EventTrigger>();
                Util.AddEventTriggerListener(ref eventTrigger, EventTriggerType.Select, (BaseEventData data) =>
                {
                    AudioManager.Instance.PlayAudio("selectpai");
                    SelectTileData selectTileData = (SelectTileData)ClientDataManager.Instance.GetData("SelectTileData");
                    bool isOnSelectTile = selectTileData.isOnSelectTile;
                    List<ushort> selectTiles = selectTileData.selectTiles;
                    if (!isOnSelectTile)
                    {
                        tile.transform.localPosition += new Vector3(0, 50, 0);
                    }
                    else
                    {
                        if (tile.transform.localPosition.y == 0)
                        {
                            tile.transform.localPosition += new Vector3(0, 50, 0);
                            selectTiles.Add(tileNum);
                        }
                        else
                        {
                            tile.transform.localPosition = new Vector3
                            (tile.transform.localPosition.x, 0, tile.transform.localPosition.z);
                            selectTiles.Remove(tileNum);
                        }
                        selectTileData.selectTiles = selectTiles;
                        ClientDataManager.Instance.RefreshData("SelectTileData", selectTileData);
                    }
                });
                Util.AddEventTriggerListener(ref eventTrigger, EventTriggerType.Deselect, (BaseEventData data) =>
                {
                    bool isOnSelectTile = ((SelectTileData)ClientDataManager.Instance.GetData("SelectTileData")).isOnSelectTile;
                    if (!isOnSelectTile)
                        tile.transform.localPosition = new Vector3
                        (tile.transform.localPosition.x, 0, tile.transform.localPosition.z);
                });
            }
            Util.RemoveEventTriggerListener(ref eventTrigger, EventTriggerType.PointerClick);
            Util.AddEventTriggerListener(ref eventTrigger, EventTriggerType.PointerClick, (BaseEventData data) =>
            {
                bool isOnSelectTile = ((SelectTileData)ClientDataManager.Instance.GetData("SelectTileData")).isOnSelectTile;
                if (isOnSelectTile)
                    return;
                if (tile.transform.localPosition.y == 0)
                    return;
                PlayTileData playTileData = (PlayTileData)ClientDataManager.Instance.GetData("PlayTileData");
                if (!playTileData.isCanPlayTile)
                    return;
                UserData userData = (UserData)ClientDataManager.Instance.GetData("UserData");
                // 1. 播放牌的声音, 刷新PlayTileData
                string audioClipName = (userData.sex == 1 ? "b_" : "g_") + ((TileTypeTileNum)tileNum).ToString();
                AudioManager.Instance.PlayAudio(audioClipName);
                playTileData.isCanPlayTile = false;
                playTileData.playTileNum = tileNum;
                playTileData.isADrawTile = isADrawTile;
                playTileData.playTileIndex = 0;
                ClientDataManager.Instance.RefreshData("PlayTileData", playTileData);
                Dictionary<short, object> playTileDict = new Dictionary<short, object>();
                playTileDict.Add(ParameterCode.PlayTileData, playTileData);
                NetManager.peerClient.SendRequestAsync(ClientToServer.PlayTile, playTileDict);
                // 2. 通知TileModel刷新ThrowTileData
                ClientDataManager.Instance.RefreshData("ThrowTileData", new ThrowTileData(0, tileNum),
                    DataEvent.ChangeThrowTileData);
                // 3. 通知ButtonModel刷新ButtonView
                ClientDataManager.Instance.RefreshData("ButtonData", new ButtonData(), DataEvent.ChangeButtonData);
                // 4.如果出牌是一张摸牌就回收，否则将摸牌整理进手牌 
                if (isADrawTile)
                {
                    ClientDataManager.Instance.RefreshData("DrawTileData", new DrawTileData(0, 0),
                        DataEvent.ChangeDrawTileData);
                    return;
                }
                HandTileData handTileData = (HandTileData)ClientDataManager.Instance.GetData("HandTileData");
                DrawTileData drawTileData = (DrawTileData)ClientDataManager.Instance.GetData("DrawTileData");
                List<ushort> tiles = handTileData.handTile.ToList();
                tiles.RemoveAll(x => x == 0);
                tiles.Remove(tileNum);
                handTileData.handTileId = 0;
                handTileData.handTile = tiles.ToArray();
                handTileData.tileCnt = (ushort)tiles.Count();
                ClientDataManager.Instance.RefreshData("HandTileData", handTileData, DataEvent.ChangeHandTileData);
                tiles.Add(drawTileData.drawTileNum);
                handTileData.handTile = tiles.ToArray();
                handTileData.tileCnt = (ushort)tiles.Count();
                ClientDataManager.Instance.RefreshData("HandTileData", handTileData);
                EventManager.Instance.Broadcast(UIEvent.MoveHandTile, (ValueType)drawTileData);
            });
        }
        public TileType GetTileType(int tileNum)
        {
            int wan1Num = TileTypeTileNum.wan_1.GetHashCode();
            int wan9Num = TileTypeTileNum.wan_9.GetHashCode();
            int tiao1Num = TileTypeTileNum.tiao_1.GetHashCode();
            int tiao9Num = TileTypeTileNum.tiao_9.GetHashCode();
            int tong1Num = TileTypeTileNum.tong_1.GetHashCode();
            int tong9Num = TileTypeTileNum.tong_9.GetHashCode();
            if (tileNum >= wan1Num && tileNum <= wan9Num)
                return TileType.wan;
            if (tileNum >= tiao1Num && tileNum <= tiao9Num)
                return TileType.tiao;
            if (tileNum >= tong1Num && tileNum <= tong9Num)
                return TileType.tong;
            return TileType.none;
        }
    }
}
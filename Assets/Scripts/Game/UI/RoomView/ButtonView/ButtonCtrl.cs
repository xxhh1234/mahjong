using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XH;

namespace mahjong
{
    class ButtonCtrl : Ctrl
    {
        public override void Init(string name)
        {
            base.Init(name);

            EventManager.Instance.AddListener(ServerToClient.SendHuTileDataToAll, (Dictionary<short, object> eventDict) =>
            {
                OnHuTileReturnData(eventDict);
            });
            EventManager.Instance.AddListener(ServerToClient.SendPengGangTileDataToAll, (Dictionary<short, object> eventDict) =>
            {
                OnPengGangReturnData(eventDict);
            });
            EventManager.Instance.AddListener(ServerToClient.SendAnGangJiaGangDataToAll, (Dictionary<short, object> eventDict) =>
            {
                OnAnGangJiaGangReturnData(eventDict);
            });
            EventManager.Instance.AddListener(ServerToClient.SendZiMoDataToAll, (Dictionary<short, object> eventDict) =>
            {
                OnZiMoReturnData(eventDict);
            });
            EventManager.Instance.AddListener(ServerToClient.SendShangHuaDataToAll, (Dictionary<short, object> eventDict) =>
            {
                OnShangHuaReturnData(eventDict);
            });
            EventManager.Instance.AddListener(ServerToClient.SendQiangGangDataToOther, (Dictionary<short, object> eventDict) =>
            {
                OnQiangGangData(eventDict);
            });
            EventManager.Instance.AddListener(ServerToClient.SendQiangGangHuDataToAll, (Dictionary<short, object> eventDict) =>
            {
                OnQiangGangReturnData(eventDict);
            });
            EventManager.Instance.AddListener(ServerToClient.SendGameOverToAll, (Dictionary<short, object> eventDict) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                { 
                    GameManager.Instance.StartCoroutine(IEGameOverData(eventDict));
                });
            });
        }

        public override void UnInit()
        {
            base.UnInit();

            EventManager.Instance.RemoveListener(ServerToClient.SendHuTileDataToAll, (Dictionary<short, object> eventDict) =>
            {
                OnHuTileReturnData(eventDict);
            });
            EventManager.Instance.RemoveListener(ServerToClient.SendPengGangTileDataToAll, (Dictionary<short, object> eventDict) =>
            {
                OnPengGangReturnData(eventDict);
            });
            EventManager.Instance.RemoveListener(ServerToClient.SendAnGangJiaGangDataToAll, (Dictionary<short, object> eventDict) =>
            {
                OnAnGangJiaGangReturnData(eventDict);
            });
            EventManager.Instance.RemoveListener(ServerToClient.SendZiMoDataToAll, (Dictionary<short, object> eventDict) =>
            {
                OnZiMoReturnData(eventDict);
            });
            EventManager.Instance.RemoveListener(ServerToClient.SendShangHuaDataToAll, (Dictionary<short, object> eventDict) =>
            {
                OnShangHuaReturnData(eventDict);
            });
            EventManager.Instance.RemoveListener(ServerToClient.SendQiangGangDataToOther, (Dictionary<short, object> eventDict) =>
            {
                OnQiangGangData(eventDict);
            });
            EventManager.Instance.RemoveListener(ServerToClient.SendQiangGangHuDataToAll, (Dictionary<short, object> eventDict) =>
            {
                OnQiangGangReturnData(eventDict);
            });
            EventManager.Instance.RemoveListener(ServerToClient.SendGameOverToAll, (Dictionary<short, object> eventDict) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    GameManager.Instance.StartCoroutine(IEGameOverData(eventDict));
                });
            });
        }

        public void OnHuTileReturnData(Dictionary<short, object> eventDict)
        {
            List<HuTileReturnData> datas = Util.Convert<List<HuTileReturnData>>(eventDict[ParameterCode.HuTileReturnData]);
            PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
            bool isOwnHu = false;
            for (int i = 0; i < datas.Count; ++i)
            {
                for (int j = 0; j < playerData.playerDatas.Length; ++j)
                {
                    if (datas[i].huTileId == playerData.playerDatas[j].playerId)
                    {
                        // 1. 播放胡的声音
                        UserData userData = (UserData)ClientDataManager.Instance.GetData("UserData");
                        string audioClipName = userData.sex == 1 ? "b_hu" : "g_hu";
                        AudioManager.Instance.PlayAudio(audioClipName);
                        // 2. 胡牌
                        HuTileReturnData data = datas[i];
                        data.huTileId = (ushort)j;
                        ThreadManager.ExecuteUpdate(() =>
                        {
                            GameManager.Instance.StartCoroutine(IEHuTile(3, data));
                        });
                        // 3. 更新积分
                        playerData.playerDatas[j].score = datas[i].score;
                        // 4. 设置是否能出牌
                        if(j == 0)
                            isOwnHu = true;
                    }
                }
            }
            ClientDataManager.Instance.RefreshData("PlayerData", playerData, DataEvent.ChangePlayerData);

            PlayTileData playTileData = (PlayTileData)ClientDataManager.Instance.GetData("PlayTileData");
            playTileData.isCanPlayTile = !isOwnHu;
            ClientDataManager.Instance.RefreshData("PlayTileData", playTileData);
        }
        public void OnPengGangReturnData(Dictionary<short, object> eventDict)
        {
            PengGangReturnData data = Util.Convert<PengGangReturnData>(eventDict[ParameterCode.PengGangReturnData]);
            PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData"); 
            for (int i = 0; i < playerData.playerCnt; ++i)
            {
                if (playerData.playerDatas[i].playerId == data.pengGangId)
                {
                    if(data.pengGangType.Equals(PengGangType.none))
                        continue;
                    // 1. 设置碰杠玩家分数, 位置
                    playerData.playerDatas[i].score = data.score;
                    data.pengGangId = (ushort)i;
                    // 2. 碰杠牌
                    if (data.pengGangType.Equals(PengGangType.peng))
                    {
                        ThreadManager.ExecuteUpdate(() =>
                        {
                            GameManager.Instance.StartCoroutine(IEPengGang(1, data, 2));
                        });
                    }
                    if(data.pengGangType.Equals(PengGangType.gang))
                    {
                        ThreadManager.ExecuteUpdate(() =>
                        {
                            GameManager.Instance.StartCoroutine(IEPengGang(2, data, 3));
                        });
                    }
                    break;
                }
            }
            ClientDataManager.Instance.RefreshData("PlayerData", playerData, DataEvent.ChangePlayerData);
        }
        public void OnAnGangJiaGangReturnData(Dictionary<short, object> eventDict)
        {
            PengGangReturnData data = Util.Convert<PengGangReturnData>(eventDict[ParameterCode.PengGangReturnData]);
            PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
            for (int i = 0; i < playerData.playerCnt; ++i)
            {
                if (playerData.playerDatas[i].playerId == data.pengGangId)
                {
                    if (data.pengGangType == PengGangType.none)
                        continue;
                    // 1. 设置碰杠玩家分数, 位置
                    playerData.playerDatas[i].score = data.score;
                    data.pengGangId = (ushort)i;
                    // 2. 判断杠牌是否是一张摸牌
                    ushort removeTileNum = 0;
                    DrawTileData drawTileData = (DrawTileData)ClientDataManager.Instance.GetData("DrawTileData");
                    bool isADrawTile = true;
                    if(i == 0 && drawTileData.drawTileNum != data.pengGangTileNum)
                    { 
                        isADrawTile = false;
                        HandTileData handTileData = (HandTileData)ClientDataManager.Instance.GetData("HandTileData");
                        List<ushort> tiles = handTileData.handTile.ToList();
                        tiles.RemoveAll(x => x == 0);
                        handTileData.handTileId = 0;
                        tiles.Add(drawTileData.drawTileNum);
                        handTileData.handTile = tiles.ToArray();
                        handTileData.tileCnt = (ushort)tiles.Count();
                        ClientDataManager.Instance.RefreshData("HandTileData", handTileData);
                    }
                    ClientDataManager.Instance.RefreshData("DrawTileData", new DrawTileData((ushort)i, 0), 
                        DataEvent.ChangeDrawTileData);
                    // 3. 碰杠牌
                    if (data.pengGangType.Equals(PengGangType.angang))
                    {
                        removeTileNum = (ushort)(isADrawTile ? 3 : 4);
                        ThreadManager.ExecuteUpdate(() =>
                        {
                            GameManager.Instance.StartCoroutine(IEPengGang(2, data, removeTileNum));
                        });
                    }
                    if (data.pengGangType.Equals(PengGangType.jiagang))
                    {
                        removeTileNum = (ushort)(isADrawTile ? 0 : 1);
                        ThreadManager.ExecuteUpdate(() =>
                        {
                            GameManager.Instance.StartCoroutine(IEPengGang(2, data, removeTileNum));
                        });
                    }
                    break;
                }
            }
            ClientDataManager.Instance.RefreshData("PlayerData", playerData, DataEvent.ChangePlayerData);
        }
        public void OnZiMoReturnData(Dictionary<short, object> eventDict)
        {
            HuTileReturnData data = Util.Convert<HuTileReturnData>(eventDict[ParameterCode.HuTileReturnData]);
            PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
            bool isOwnHu = false;
            for (int i = 0; i < playerData.playerDatas.Length; ++i)
            {
                if (data.huTileId == playerData.playerDatas[i].playerId)
                {
                    // 1. 播放自摸的声音
                    UserData userData = (UserData)ClientDataManager.Instance.GetData("UserData");
                    string audioClipName = userData.sex == 1 ? "b_hu_zimo" : "g_hu_zimo";
                    AudioManager.Instance.PlayAudio(audioClipName);
                    // 2. 更新积分, 胡牌玩家位置
                    playerData.playerDatas[i].score = data.score;
                    data.huTileId = (ushort)i;
                    // 3. 胡牌
                    ThreadManager.ExecuteUpdate(() =>
                    {
                        GameManager.Instance.StartCoroutine(IEHuTile(4, data));
                    });
                    // 4. 设置是否能出牌
                    if (i == 0)
                        isOwnHu = true;
                    break;
                }
            }
            ClientDataManager.Instance.RefreshData("PlayerData", playerData, DataEvent.ChangePlayerData);

            PlayTileData playTileData = (PlayTileData)ClientDataManager.Instance.GetData("PlayTileData");
            playTileData.isCanPlayTile = !isOwnHu;
            ClientDataManager.Instance.RefreshData("PlayTileData", playTileData);
        }
        public void OnShangHuaReturnData(Dictionary<short, object> eventDict)
        {
            HuTileReturnData data = Util.Convert<HuTileReturnData>(eventDict[ParameterCode.HuTileReturnData]);
            PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
            bool isOwnHu = false;
            for (int i = 0; i < playerData.playerDatas.Length; ++i)
            {
                if (data.huTileId == playerData.playerDatas[i].playerId)
                {
                    // 1. 播放杠上开花的声音
                    UserData userData = (UserData)ClientDataManager.Instance.GetData("UserData");
                    string audioClipName = userData.sex == 1 ? "b_gangshangkaihua" : "g_gangshangkaihua";
                    AudioManager.Instance.PlayAudio(audioClipName);
                    // 2. 更新积分, 胡牌位置
                    playerData.playerDatas[i].score = data.score;
                    data.huTileId = (ushort)i;
                    // 3. 杠上开花
                    ThreadManager.ExecuteUpdate(() => { GameManager.Instance.StartCoroutine(IEHuTile(5, data)); });
               
                    // 4. 设置是否能出牌
                    if (i == 0)
                        isOwnHu = true;
                    break;
                }
            }
            ClientDataManager.Instance.RefreshData("PlayerData", playerData, DataEvent.ChangePlayerData);

            PlayTileData playTileData = (PlayTileData)ClientDataManager.Instance.GetData("PlayTileData");
            playTileData.isCanPlayTile = !isOwnHu;
            ClientDataManager.Instance.RefreshData("PlayTileData", playTileData);
        }
        public void OnQiangGangData(Dictionary<short, object> eventDict)
        {
            QiangGangData qiangGangData = Util.Convert<QiangGangData>(eventDict[ParameterCode.QiangGangData]);
            HuTileType qiangGangType = qiangGangData.huTileType;
            
            if (!qiangGangType.Equals(HuTileType.buhu))
            {
                ButtonData buttonData = new ButtonData(7, ParameterCode.HuTileType, qiangGangType, 
                    ClientToServer.QiangGangDataReturn);
                ClientDataManager.Instance.RefreshData("ButtonData", buttonData, DataEvent.ChangeButtonData);
            }
            else
            {
                Dictionary<short, object> dictQiangGang = new Dictionary<short, object>();
                dictQiangGang.Add(ParameterCode.HuTileReturnData, HuTileType.buhu);
                NetManager.peerClient.SendRequestAsync(ClientToServer.QiangGangDataReturn, dictQiangGang);
            }
        }
        public void OnQiangGangReturnData(Dictionary<short, object> eventDict)
        {
            HuTileReturnData data = Util.Convert<HuTileReturnData>(eventDict[ParameterCode.HuTileReturnData]);
            PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
            bool isOwnHu = false;
            for (int i = 0; i < playerData.playerDatas.Length; ++i)
            {
                if (data.huTileId == playerData.playerDatas[i].playerId)
                {
                    // 1. 更新积分，胡牌位置
                    playerData.playerDatas[i].score = data.score;
                    data.huTileId = (ushort)i;
                    // 2. 胡牌
                    ThreadManager.ExecuteUpdate(() => { GameManager.Instance.StartCoroutine(IEHuTile(6, data)); });
                    // 3. 设置是否能出牌
                    if (i == 0)
                        isOwnHu = true;
                    break;
                }
            }
            ClientDataManager.Instance.RefreshData("PlayerData", playerData, DataEvent.ChangePlayerData);

            PlayTileData playTileData = (PlayTileData)ClientDataManager.Instance.GetData("PlayTileData");
            playTileData.isCanPlayTile = !isOwnHu;
            ClientDataManager.Instance.RefreshData("PlayTileData", playTileData);
        }
        public IEnumerator IEGameOverData(Dictionary<short, object> eventDict)
        {
            List<HuTileReturnData> datas = Util.Convert<List<HuTileReturnData>>(eventDict[ParameterCode.HuTileReturnData]);
            PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
            for (int i = 0; i < datas.Count; ++i)
            {
                for (int j = 0; j < playerData.playerDatas.Length; ++j)
                {
                    if (datas[i].huTileId == playerData.playerDatas[j].playerId)
                    {
                        // 1. 删除手牌
                        HandTileData handTileData = new HandTileData();
                        handTileData.handTileId = (ushort)i;
                        ClientDataManager.Instance.RefreshData("HandTileData", handTileData, DataEvent.ChangeHandTileData);
                        // 2. 删除摸牌
                        DrawTileData drawTileData = new DrawTileData();
                        drawTileData.drawTileId = (ushort)i;
                        ClientDataManager.Instance.RefreshData("DrawTileData", drawTileData, DataEvent.ChangeDrawTileData);
                        // 3. 删除碰杠牌
                        PengGangReturnData pengGangReturnData = new PengGangReturnData();
                        pengGangReturnData.pengGangId = (ushort)i;
                        ClientDataManager.Instance.RefreshData("PengGangReturnData", pengGangReturnData, DataEvent.ChangePengGangReturnData);

                        yield return new WaitForSeconds(0.2f);

                        // 4. 生成结束的牌
                        Array.Sort(datas[i].huTile);
                        Array.Reverse(datas[i].huTile);
                        ClientDataManager.Instance.RefreshData("HuTileReturnData", datas[i], DataEvent.ChangeHuTileReturnData);
                    }
                }
            }
            
            // 5. 设置是否能出牌
            PlayTileData playTileData = (PlayTileData)ClientDataManager.Instance.GetData("PlayTileData");
            playTileData.isCanPlayTile = false;
            ClientDataManager.Instance.RefreshData("PlayTileData", playTileData);
            
            yield return new WaitForSeconds(1f);

            // 6. 显示SettleView
            UIManager.Instance.ShowView("SettleView");
            // 7. 通知SettleModel刷新SettleView
            // TODO:(分数，胡牌类型）


        }

        private void AddButtonCtrl(short param, object obj, short opCode)
        {
            // 播放按钮点击的声音
            AudioManager.Instance.PlayAudio("click1");
            Dictionary<short, object> dict = new Dictionary<short, object>();
            if (obj != null) dict.Add(param, obj);
            NetManager.peerClient.SendRequestAsync(opCode, dict);
        }
        public IEnumerator IEHuTile(ushort tipNum, HuTileReturnData data)
        {
            // 1. 生成胡的特效
            TipData tipData = new TipData();
            tipData.isTile = false;
            tipData.tipNum = tipNum;
            tipData.tipId = data.huTileId;
            tipData.isOnce = false;
            ClientDataManager.Instance.RefreshData("TipData", tipData, DataEvent.ChangeTipData);
            // 2. 删除抛牌中的出牌
            EventManager.Instance.Broadcast(UIEvent.RemoveThrowTile_1);
            // 3. 删除手牌
            HandTileData handTileData = new HandTileData();
            handTileData.handTileId = data.huTileId;
            EventManager.Instance.Broadcast(UIEvent.RefreshHandTileView, (ValueType)handTileData);
            // 4. 删除摸牌
            DrawTileData drawTileData = new DrawTileData();
            drawTileData.drawTileId = data.huTileId;
            ClientDataManager.Instance.RefreshData("DrawTileData", drawTileData, DataEvent.ChangeDrawTileData);
            // 5. 删除碰杠牌
            PengGangReturnData pengGangReturnData = new PengGangReturnData();
            pengGangReturnData.pengGangId = data.huTileId;
            ClientDataManager.Instance.RefreshData("PengGangReturnData", pengGangReturnData, DataEvent.ChangePengGangReturnData);

            yield return new WaitForSeconds(0.2f);

            // 6. 生成胡牌
            Array.Sort(data.huTile);
            Array.Reverse(data.huTile);
            ClientDataManager.Instance.RefreshData("HuTileReturnData", data, DataEvent.ChangeHuTileReturnData);
        }
        public IEnumerator IEPengGang(ushort tipNum, PengGangReturnData data, ushort removeTileNum)
        {
            // 1. 播放声音
            UserData userData = (UserData)ClientDataManager.Instance.GetData("UserData");
            string audioClipName = userData.sex == 1 ? "b_" : "g_";
            audioClipName += data.pengGangType.Equals(PengGangType.peng) ? "peng" : "gang";
            AudioManager.Instance.PlayAudio(audioClipName);
            // 2. 生成特效
            TipData tipData = new TipData();
            tipData.isTile = false;
            tipData.tipId = data.pengGangId;
            tipData.tipNum = tipNum;
            tipData.isOnce =  true;
            ClientDataManager.Instance.RefreshData("TipData", tipData, DataEvent.ChangeTipData);

            yield return new WaitForSeconds(0.2f);
            // 3. 删除抛牌中的出牌
            EventManager.Instance.Broadcast(UIEvent.RemoveThrowTile_1);
            // 4. 删除手牌中的碰杠牌并重新生成
            HandTileData handTileData = (HandTileData)ClientDataManager.Instance.GetData("HandTileData");
            handTileData.handTileId = data.pengGangId;
            
            if(data.pengGangId == 0)
            { 
                List<ushort> tiles = handTileData.handTile.ToList();
                tiles.RemoveAll(x => x==0);
                for(int i = 0; i < removeTileNum; ++i)
                    tiles.Remove(data.pengGangTileNum);
                handTileData.handTile = tiles.ToArray();
                handTileData.tileCnt = (ushort)tiles.Count();
                ClientDataManager.Instance.RefreshData("HandTileData", handTileData, DataEvent.ChangeHandTileData);
            }
            else
            {
                handTileData.tileCnt = removeTileNum;
                EventManager.Instance.Broadcast(UIEvent.RefreshHandTileView, handTileData as ValueType);
            }
            // 5. 生成碰杠牌
            ClientDataManager.Instance.RefreshData("PengGangReturnData", data, DataEvent.ChangePengGangReturnData);
            // 6. 设置房间是否出牌
            PlayTileData playTileData = (PlayTileData)ClientDataManager.Instance.GetData("PlayTileData");
            playTileData.isCanPlayTile = (data.pengGangId==0);
            ClientDataManager.Instance.RefreshData("PlayTileData", playTileData);
        }
    }
}

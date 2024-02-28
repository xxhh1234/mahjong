using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using XH;

namespace mahjong
{
    class PlayerCtrl : Ctrl
    {
        public override void Init(string name)
        {
            base.Init(name);

            EventManager.Instance.AddListener(ServerToClient.SendPlayerToOther, (Dictionary<short, object> eventDict) =>
            { OnOtherJoinRoom(eventDict); });
            EventManager.Instance.AddListener(ServerToClient.SendLeaveRoomIdToOther, (Dictionary<short, object> eventDict) =>
            { OnOtherLeaveRoom(eventDict); });
            EventManager.Instance.AddListener(ServerToClient.SendReadyPlayerIdToOther, (Dictionary<short, object> eventDict) =>
            { OnOtherReady(eventDict); });
        }

        public override void UnInit()
        {
            base.UnInit();

            EventManager.Instance.RemoveListener(ServerToClient.SendPlayerToOther, (Dictionary<short, object> eventDict) =>
            { OnOtherJoinRoom(eventDict); });
            EventManager.Instance.RemoveListener(ServerToClient.SendLeaveRoomIdToOther, (Dictionary<short, object> eventDict) =>
            { OnOtherLeaveRoom(eventDict); });
            EventManager.Instance.RemoveListener(ServerToClient.SendReadyPlayerIdToOther, (Dictionary<short, object> eventDict) =>
            { OnOtherReady(eventDict); });
        }

        private void OnButtonReady()
        {
            AudioManager.Instance.PlayAudio("ready");
            NetManager.peerClient.SendRequestAsync(ClientToServer.ClickReady, null, (Dictionary<short, object> respDict) =>
            {
                ushort readyPlayerId = Util.Convert<ushort>(respDict[ParameterCode.playerId]);
                PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
                for(int i = 0; i< playerData.playerCnt; ++i)
                {
                    if(readyPlayerId == playerData.playerDatas[i].playerId)
                    {
                        playerData.playerDatas[i].isReady = true;
                        break;
                    }
                }
                ClientDataManager.Instance.RefreshData("PlayerData", playerData, DataEvent.ChangePlayerData);
            });
        }

        public void OnOtherJoinRoom(Dictionary<short, object> eventDict)
        {
            // 1. 播放其他玩家加入房间的声音
            AudioManager.Instance.PlayAudio("playerEnter");
            // 2. 解析数据
            SinglePlayerData playerData1 = Util.Convert<SinglePlayerData>(eventDict[ParameterCode.SinglePlayerData]);
            PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
            // 3. 设置加入玩家数据的位置
            for(int i = 0; i < playerData.playerDatas.Length; ++i)
            {
                if (playerData.playerDatas[i].playerName == null)
                { 
                    playerData.playerDatas[i] = playerData1;
                    ++playerData.playerCnt;
                    break;
                }
            }
            // 4. 通知PlayerModel刷新PlayerView
            ClientDataManager.Instance.RefreshData("PlayerData", playerData, DataEvent.ChangePlayerData);
        }
        public void OnOtherLeaveRoom(Dictionary<short, object> eventDict)
        {
            // 1. 播放其他玩家离开房间的声音
            AudioManager.Instance.PlayAudio("playerLeave");
            // 2. 解析数据
            int leaveId = Util.Convert<int>(eventDict[ParameterCode.playerId]);
            PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
            // 3. 通知PlayerModel刷新PlayerView
            for (int i = 1; i < 4; ++i)
            {
                if (playerData.playerDatas[i].playerId == leaveId)
                {
                    playerData.playerDatas[i] = default;
                    break;
                }
            }
            // 4. 通知PlayerModel刷新PlayerView
            ClientDataManager.Instance.RefreshData("PlayerData", playerData, DataEvent.ChangePlayerData);
        }
        public void OnOtherReady(Dictionary<short, object> eventDict)
        {
            AudioManager.Instance.PlayAudio("ready");
            // 1. 解析数据
            int readyId = Util.Convert<int>(eventDict[ParameterCode.playerId]);
            PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
            for (int i = 1; i < 4; ++i)
            {
                if (playerData.playerDatas[i].playerId == readyId)
                {
                    playerData.playerDatas[i].isReady = true;
                    break;
                }
            }
            // 2. 通知PlayerModel刷新PlayerView
            ClientDataManager.Instance.RefreshData("PlayerData", playerData, DataEvent.ChangePlayerData);

        }
    }
}

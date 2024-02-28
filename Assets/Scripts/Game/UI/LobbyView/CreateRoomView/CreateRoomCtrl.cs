using System.Collections.Generic;
using XH;

namespace mahjong
{
    class CreateRoomCtrl : Ctrl
    {
        public override void Init(string name)
        {
            base.Init(name);
        }

        public override void UnInit()
        {
            base.UnInit();
        }

        private void OnButtonCreate()
        {
            // 1. 播放创建房间按钮的声音
            AudioManager.Instance.PlayAudio("click");
            NetManager.peerClient.SendRequestAsync(ClientToServer.CreateRoom, null, 
                (Dictionary<short, object> respDict) =>
            {
                // 2.关闭大厅界面并创建房间
                UIManager.Instance.CloseView("CreateRoomView");
                UIManager.Instance.CloseView("LobbyView");
                UIManager.Instance.ShowView("RoomView");
                View roomView = UIManager.Instance.GetView("RoomView");
                UIManager.Instance.ShowView("PlayerView", roomView.gameObject.transform);
                UIManager.Instance.ShowView("TileView", roomView.gameObject.transform);
                UIManager.Instance.ShowView("ButtonView", roomView.gameObject.transform);
                // 3. 解析数据
                RoomData roomData = Util.Convert<RoomData>(respDict[ParameterCode.RoomData]);
                SinglePlayerData playerData1 = Util.Convert<SinglePlayerData>(respDict[ParameterCode.SinglePlayerData]);
                PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
                playerData.playerDatas[playerData.playerCnt++] = playerData1;
                // 4.通知RoomModel刷新RoomView
                ClientDataManager.Instance.RefreshData("RoomData", roomData, DataEvent.ChangeRoomData);
                // 5.通知PlayerModel刷新PlayerView
                ClientDataManager.Instance.RefreshData("PlayerData", playerData,DataEvent.ChangePlayerData);
            });
        }
    }
}

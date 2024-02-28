using System.Collections.Generic;
using Unity.VisualScripting;
using XH;

namespace mahjong
{
    class JoinRoomCtrl : Ctrl
    {
        private void OnButtonJoin(string roomName)
        {
            // 1. 播放加入房间按钮声音
            AudioManager.Instance.PlayAudio("click1");
            Dictionary<short, object> dictJoinRoom = new Dictionary<short, object>();
            JoinRoomData joinRoomData = new JoinRoomData();
            joinRoomData.roomName = roomName;
            dictJoinRoom.Add(ParameterCode.JoinRoomData, joinRoomData);
            NetManager.peerClient.SendRequestAsync(ClientToServer.JoinRoom, dictJoinRoom,
                (Dictionary<short, object> respDict) =>
                {
                    // 2.关闭大厅界面并创建房间
                    UIManager.Instance.CloseView("JoinRoomView");
                    UIManager.Instance.CloseView("LobbyView");
                    UIManager.Instance.ShowView("RoomView");
                    View roomView = UIManager.Instance.GetView("RoomView");
                    UIManager.Instance.ShowView("PlayerView", roomView.gameObject.transform);
                    UIManager.Instance.ShowView("TileView", roomView.gameObject.transform);
                    UIManager.Instance.ShowView("ButtonView", roomView.gameObject.transform);
                    // 3. 解析数据
                    RoomData roomData = Util.Convert<RoomData>(respDict[ParameterCode.RoomData]);
                    PlayerData playerData = Util.Convert<PlayerData>(respDict[ParameterCode.PlayerData]);
                    // 4. 通知RoomModel刷新RoomView
                    ClientDataManager.Instance.RefreshData("RoomData", roomData, DataEvent.ChangeRoomData);
                    // 5.通知PlayerModel刷新PlayerView
                    ClientDataManager.Instance.RefreshData("PlayerData", playerData, DataEvent.ChangePlayerData);
                });
        }
    }
}

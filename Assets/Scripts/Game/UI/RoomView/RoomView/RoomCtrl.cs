using System.Collections.Generic;
using XH;

namespace mahjong
{
    class RoomCtrl : Ctrl
    {
        public override void Init(string name)
        {
            base.Init(name);

         
        }

        public override void UnInit()
        {
            base.UnInit();

            
        }


        private void OnButtonLeave()
        {
            AudioManager.Instance.PlayAudio("click3");
            NetManager.peerClient.SendRequestAsync(ClientToServer.LeaveRoom, null, (Dictionary<short, object> respDict) =>
            {
                UIManager.Instance.CloseView("RoomView");
                UIManager.Instance.ShowView("LobbyView");
            });
        }
    }
}

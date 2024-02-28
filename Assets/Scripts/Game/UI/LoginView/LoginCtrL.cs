using System.Collections.Generic;
using XH;

namespace mahjong
{
    class LoginCtrl : Ctrl
    {
        public override void Init(string name)
        {
            base.Init(name);
        }

        public override void UnInit()
        {
            base.UnInit();
        }

        private void OnButtonLogin(string id)
        {
            // 1. 播放登录按钮声音
            AudioManager.Instance.PlayAudio("click3");
            Dictionary<short, object> requDict = new Dictionary<short, object>();
            requDict.Add(ParameterCode.LoginData, int.Parse(id));
            NetManager.peerClient.SendRequestAsync(ClientToServer.Login, requDict, 
                (Dictionary<short, object> respDict) =>
            {
                // 2. 关闭LoginView，打开LobbyView
                UIManager.Instance.CloseView("LoginView");
                UIManager.Instance.ShowView("LobbyView");
                // 3.刷新UserData
                UserData userData = Util.Convert<UserData>(respDict[ParameterCode.UserData]);
                ClientDataManager.Instance.RefreshData("UserData", userData, DataEvent.ChangeUserData);
            });
        }
    }
}

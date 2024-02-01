using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace XH
{
    class LoginController : Controller
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
            Dictionary<short, object> requDict = new Dictionary<short, object>();
            requDict.Add(ParameterCode.id, int.Parse(id));
            NetManager.peerClient.SendRequestAsync((short)EventType.LOGIC_Login, requDict, 
                (Dictionary<short, object> respDict) =>
            {
                ObjectPool.Instance.CollectGameObject(GameManager.Instance.goLoginView);
                // 2. 打开大厅界面并初始化
                GameManager.Instance.goLobbyView = ObjectPool.Instance.GetGameObject("LobbyView", GameResources.Instance.prefabLobbyView);
                UIManager.Instance.SwitchView("LoginView", "LobbyView");

                DataManager.Instance.RefreshData("UserData", (UserData)respDict[ParameterCode.userInfo],
                    EventType.UI_ChangeUserData);
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace XH
{
    struct UserData
    {
        public string Id;
        public string Username;
        public int Coin;
        public int Diamond;
        public int Sex;
    }

    class LobbyModel : Model
    {
        public override void Init(string name)
        {
            base.Init(name);
            DataManager.Instance.AddData("UserData", new UserData());
            EventManager.Instance.AddListener<UserData>(EventType.UI_ChangeUserData, SetUserData);
        }

        public override void UnInit()
        {
            base.UnInit();
            EventManager.Instance.RemoveListener<UserData>(EventType.UI_ChangeUserData, SetUserData);
        }

        private void SetUserData(UserData userData)
        {
            EventManager.Instance.Broadcast(EventType.UI_RefreshLobbyView, userData);
        }
       
       
    }
}

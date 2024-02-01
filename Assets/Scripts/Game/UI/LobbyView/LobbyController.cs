using System.Collections.Generic;
using UnityEngine;

namespace XH
{
    class LobbyController : Controller
    {
        public override void Init(string name)
        {
            base.Init(name);
        }

        public override void UnInit()
        {
            base.UnInit();
        }


        private void OnCreateRoomView(GameObject goCreateRoomView)
        {
            goCreateRoomView.SetActive(true);
        }

        private void OnJoinRoomView(GameObject goJoinRoomView)
        {
            goJoinRoomView.SetActive(true);
        }


    }
}

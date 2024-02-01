using System;
using UnityEngine;
using UnityEngine.UI;

namespace XH
{
    class LobbyView : View
    {
        private Image imgHead;
        private Text txtUsername, txtId, txtCoin, txtDiamond;
        private Button btnAddCoin, btnAddDiamond, btnAddPrice;
        // mid
        private Button btnCreateRoom, btnJoinRoom;
        // bottom
        private Button btnActivity, btnTask, btnShop, btnIntraCity, btnSetting;
        // panel
        private GameObject goCreateRoomView, goJoinRoomView;

        public override void Init(string name)
        {
            base.Init(name);

            imgHead = transform.Find("imgTop/imgHead").GetComponent<Image>();
            txtUsername = transform.Find("imgTop/imgHead/txtUsername").GetComponent<Text>();
            txtId = transform.Find("imgTop/imgHead/txtId").GetComponent<Text>();
            txtCoin = transform.Find("imgTop/imgCoin/txtCoin").GetComponent<Text>();
            txtDiamond = transform.Find("imgTop/imgDiamond/txtDiamond").GetComponent<Text>();
            btnCreateRoom = transform.Find("imgMid/btnCreateRoom").GetComponent<Button>();
            btnJoinRoom = transform.Find("imgMid/btnJoinRoom").GetComponent<Button>();
            goCreateRoomView = transform.Find("CreateRoomPanel").gameObject;
            goJoinRoomView = transform.Find("JoinRoomPanel").gameObject;

            btnCreateRoom.onClick.AddListener(() => 
            { 
                Action<GameObject> action = controller.GetMethod("OnCreateRoomView") as Action<GameObject>; 
                action(goCreateRoomView); 
            });
            btnJoinRoom.onClick.AddListener(() => 
            { 
                Action<GameObject> action = controller.GetMethod("OnJoinRoomView") as Action<GameObject>;
                action(goJoinRoomView); 
            });

            EventManager.Instance.AddListener(EventType.UI_RefreshLobbyView, (UserData userData) => { RefreshLobbyView(userData); });
        }

        public override void UnInit()
        {
            base.UnInit();
            EventManager.Instance.RemoveListener(EventType.UI_RefreshLobbyView, (UserData userData) => { RefreshLobbyView(userData); });

        }

        public void RefreshLobbyView(UserData userData)
        {
            if(userData.Sex == 1)
                imgHead.sprite =  GameResources.Instance.spriteBoy;
            else if(userData.Sex == 2)
                imgHead.sprite =  GameResources.Instance.spriteGirl;
            else
                imgHead.sprite =  GameResources.Instance.spriteDefault;
            txtUsername.text = userData.Username;
            txtId.text = userData.Id;
            txtUsername.text = userData.Username;
            txtCoin.text = userData.Coin.ToString();
            txtDiamond.text = userData.Diamond.ToString();
        }
    }
}

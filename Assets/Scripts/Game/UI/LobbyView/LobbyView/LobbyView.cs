using System;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;
using XH;

namespace mahjong
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

            btnCreateRoom.onClick.AddListener(OnCreateRoomView);
            btnJoinRoom.onClick.AddListener(OnJoinRoomView);

            EventManager.Instance.AddListener(UIEvent.RefreshLobbyView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    RefreshLobbyView(value);
                });
            });
        }

        public override void UnInit()
        {
            base.UnInit();
            EventManager.Instance.RemoveListener(UIEvent.RefreshLobbyView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    RefreshLobbyView(value);
                });
            });
        }

        public void RefreshLobbyView(ValueType value)
        {
            UserData userData = (UserData)value;
            Texture2D texture; 
            if(userData.sex == 0)
                ResourceManager.Instance.Load(out texture, "DefaultHead");
            else if(userData.sex == 1)
                ResourceManager.Instance.Load(out texture, "Boy");
            else
                ResourceManager.Instance.Load(out texture, "Girl");
            imgHead.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            txtUsername.text = userData.username;
            txtId.text = userData.id;

            txtCoin.text = userData.coin.ToString();
            txtDiamond.text = userData.diamond.ToString();
        }

        private void OnCreateRoomView()
        {
            UIManager.Instance.ShowView("CreateRoomView", transform);
        }

        private void OnJoinRoomView()
        {
            UIManager.Instance.ShowView("JoinRoomView", transform);
        }
    }
}

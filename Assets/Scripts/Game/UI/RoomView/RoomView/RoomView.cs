using System;
using UnityEngine;
using UnityEngine.UI;
using XH;

namespace mahjong
{
    class RoomView : View
    {
        private Text txtRemainTile;
        private Text txtRoomName;
        private Button btnLeave;
        

        public override void Init(string name)
        {
            base.Init(name);

            txtRemainTile = transform.Find("imgRemainTile/txtRemainTile").GetComponent<Text>();
            txtRoomName = transform.Find("imgRoomName/txtRoomName").GetComponent<Text>();
            btnLeave = transform.Find("btnLeave").GetComponent<Button>();
            btnLeave.onClick.AddListener(() =>
            {
               Action action = controller.GetMethod("OnButtonLeave") as Action;
                action();
            });

            EventManager.Instance.AddListener(UIEvent.RefreshRoomView, (ValueType value)=>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    RefreshRoomView(value);

                });
            });
        }

        public override void UnInit()
        {
            base.UnInit();

            EventManager.Instance.RemoveListener(UIEvent.RefreshRoomView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    RefreshRoomView(value);
                });
            });
        }

        private void RefreshRoomView(ValueType value)
        {
            RoomData roomData = (RoomData)value;
            txtRemainTile.text = roomData.remainTile.ToString();
            txtRoomName.text = roomData.roomName.ToString();
        }
    }
}

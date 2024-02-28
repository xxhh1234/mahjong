using System;
using UnityEngine.UI;
using XH;

namespace mahjong
{
    class CreateRoomView : View
    {
        private Text txtConsumeDiamond;
        private Button btnCreate;
        private Button btnClose;

        public override void Init(string name)
        {
            base.Init(name);
            txtConsumeDiamond = transform.Find("imgCreateRoom/SiChuanPanel/imgConsume/txtConsume").GetComponent<Text>();
            btnCreate = transform.Find("imgCreateRoom/SiChuanPanel/btnCreate").GetComponent<Button>();
            btnClose = transform.Find("imgCreateRoom/btnClose").GetComponent<Button>();
            btnCreate.onClick.AddListener(() =>
            { 
                Action action = controller.GetMethod("OnButtonCreate") as Action;
                action();
            });
            btnClose.onClick.AddListener(OnButtonClose);

        }

        public override void UnInit()
        {
            base.UnInit();
        }

        private void OnButtonClose()
        {
            UIManager.Instance.CloseView("CreateRoomView");
        }
    }
}

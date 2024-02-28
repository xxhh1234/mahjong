using System;
using UnityEngine.UI;
using XH;

namespace mahjong
{
    class DingQueView : View
    {
        private Button btnWan, btnTiao, btnTong;

        public override void Init(string name)
        {
            base.Init(name);

            btnWan = transform.Find("btnCharacter").GetComponent<Button>();
            btnTiao = transform.Find("btnBamboo").GetComponent<Button>();
            btnTong = transform.Find("btnCircle").GetComponent<Button>();

            btnWan.onClick.AddListener(() =>
            {
                Action action = controller.GetMethod("OnButtonWan") as Action;
                action();
            });
            btnTiao.onClick.AddListener(() => 
            { 
                Action action = controller.GetMethod("OnButtonTiao") as Action;
                action();
            });
            btnTong.onClick.AddListener(() => 
            {
                Action action = controller.GetMethod("OnButtonTong") as Action;
                action();
            });
        }

        public override void UnInit()
        {
            base.UnInit();
        }
    }
}

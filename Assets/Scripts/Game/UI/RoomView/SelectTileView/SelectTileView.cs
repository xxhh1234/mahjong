using System;
using UnityEngine.UI;
using XH;   

namespace mahjong
{
    class SelectTileView : View
    {
        private Button btnSure;

        public override void Init(string name)
        {
            base.Init(name);

            btnSure = transform.Find("btnSure").GetComponent<Button>();
            btnSure.onClick.AddListener(() =>
            {
                Action action = controller.GetMethod("OnButtonSure") as Action;
                action();
            });
        }

        public override void UnInit()
        {
            base.UnInit();
        }


    }
}

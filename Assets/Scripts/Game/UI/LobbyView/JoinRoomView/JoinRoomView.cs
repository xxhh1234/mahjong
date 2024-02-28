using System;
using System.Reflection;
using UnityEngine.UI;
using XH;

namespace mahjong
{
    class JoinRoomView : View
    {
        private Text[] txtInputs = new Text[5];
        public Button[] btnDigits = new Button[10];
        public Button btnClear, btnDelete, btnClose, btnJoin;

        public override void Init(string name)
        {
            base.Init(name);

            for(int i = 0; i < txtInputs.Length; ++i)
            { 
                string index = (i + 1).ToString();
                txtInputs[i] = transform.Find("imgJoinRoom/imgInput" + index + "/txtInput" + index).GetComponent<Text>();
            }

            for(int i = 0; i < btnDigits.Length; ++i)
            {
                string index = i.ToString();
                btnDigits[i] = transform.Find("imgJoinRoom/btn" + index).GetComponent<Button>();
            }
            btnDigits[0].onClick.AddListener(OnButton0);
            btnDigits[1].onClick.AddListener(OnButton1);
            btnDigits[2].onClick.AddListener(OnButton2);
            btnDigits[3].onClick.AddListener(OnButton3);
            btnDigits[4].onClick.AddListener(OnButton4);
            btnDigits[5].onClick.AddListener(OnButton5);
            btnDigits[6].onClick.AddListener(OnButton6);
            btnDigits[7].onClick.AddListener(OnButton7);
            btnDigits[8].onClick.AddListener(OnButton8);
            btnDigits[9].onClick.AddListener(OnButton9);


            btnClear = transform.Find("imgJoinRoom/btnClear").GetComponent<Button>();
            btnDelete = transform.Find("imgJoinRoom/btnDelete").GetComponent<Button>();
            btnClose = transform.Find("imgJoinRoom/btnClose").GetComponent<Button>();
            btnJoin = transform.Find("imgJoinRoom/btnJoin").GetComponent<Button>();
            btnClear.onClick.AddListener(OnButtonClear);
            btnDelete.onClick.AddListener(OnButtonDelete);
            btnClose.onClick.AddListener(OnButtonClose);
            btnJoin.onClick.AddListener(() =>
            {
                string roomName="";
                for(int i = 0; i < txtInputs.Length; ++i)
                { 
                    if (string.IsNullOrEmpty(txtInputs[i].text))
                        return;
                    roomName += txtInputs[i].text;
                }
                Action<string> action = controller.GetMethod("OnButtonJoin") as Action<string>;
                action(roomName);
            });
        }

        public override void UnInit()
        {
            base.UnInit();
        }

        private void RefreshJoinRoomView(ValueType value)
        {
            JoinRoomData joinRoomData = (JoinRoomData)value;
            string roomName = joinRoomData.roomName;
            if(string.IsNullOrEmpty(roomName))
            { 
                for(int i = 0; i < txtInputs.Length; ++i)
                    txtInputs[i].text = "";
            }
            else
            {
                for (int i = 0; i < txtInputs.Length && i < roomName.Length; ++i)
                    txtInputs[i].text = roomName[i].ToString();
            }
        }

        private void OnButton(int digit)
        {
            for(int i = 0; i < txtInputs.Length; ++i)
            {
                if (string.IsNullOrEmpty(txtInputs[i].text))
                { 
                    txtInputs[i].text = digit.ToString();
                    return;
                }
            }
        }
        private void OnButton0()
        {
            OnButton(0);
        }
        private void OnButton1()
        {
            OnButton(1);
        }
        private void OnButton2()
        {
            OnButton(2);
        }
        private void OnButton3()
        {
            OnButton(3);
        }
        private void OnButton4()
        {
            OnButton(4);
        }
        private void OnButton5()
        {
            OnButton(5);
        }
        private void OnButton6()
        {
            OnButton(6);
        }
        private void OnButton7()
        {
            OnButton(7);
        }
        private void OnButton8()
        {
            OnButton(8);
        }
        private void OnButton9()
        {
            OnButton(9);
        }
        private void OnButtonClear()
        {
           RefreshJoinRoomView(new JoinRoomData());
        }
        private void OnButtonDelete()
        {
            for(int i = txtInputs.Length - 1; i >= 0; --i)
            {
                if (!string.IsNullOrEmpty(txtInputs[i].text))
                {
                    txtInputs[i].text = "";
                    break;
                }
            }
        }
        private void OnButtonClose()
        {
            UIManager.Instance.CloseView("JoinRoomView");
        }
    }
}

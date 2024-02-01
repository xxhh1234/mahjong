using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XH
{
    public class CreateRoomPanel : MonoBehaviour
    {
        public Text txtConsumeDiamond;
        public Button btnCreate;
        public Button btnClose;


        private void Start()
        {
            txtConsumeDiamond = transform.Find("imgCreateRoom/SiChuanPanel/imgConsume/txtConsume").GetComponent<Text>();
            btnCreate = transform.Find("imgCreateRoom/SiChuanPanel/btnCreate").GetComponent<Button>();
            btnClose = transform.Find("imgCreateRoom/btnClose").GetComponent<Button>();
            btnCreate.onClick.AddListener(OnButtonCreate);
            btnClose.onClick.AddListener(OnButtonClose);

        }

        public void OnButtonCreate()
        {
            NetManager.peerClient.SendRequest((short)OpCode.CreateRoomPanel, null);
        }

        public void OnButtonClose()
        {
            gameObject.SetActive(false);
        }
    }
}


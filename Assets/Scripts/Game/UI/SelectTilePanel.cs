using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XH
{
    public class SelectTilePanel : MonoBehaviour
    {
        public Button btnSure;

        private void Start()
        {
            btnSure = transform.Find("btnSure").GetComponent<Button>();
            btnSure.onClick.AddListener(OnButtonSure);
        }

        public void OnButtonSure()
        {
            if (!RoomPanel.isCanSelectTile) return;
            if (RoomPanel.selectTileList.Count != 3) return;
            if (!GameManager.Instance.GetTileType(RoomPanel.selectTileList[0]).Equals
                (GameManager.Instance.GetTileType(RoomPanel.selectTileList[1]))) return;
            if (!GameManager.Instance.GetTileType(RoomPanel.selectTileList[1]).Equals
                (GameManager.Instance.GetTileType(RoomPanel.selectTileList[2]))) return;
            // 1. ѡ�ƽ���
            RoomPanel.isCanSelectTile = false;
            // 2.�ر�ѡ��3�����ƽ���
            gameObject.SetActive(false);
            // 3.ɾ�������е�3���Ʋ������������3����
            // GameManager.Instance.SelectTile(RoomPanel.selectTileList);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SettlePanel : MonoBehaviour
{
    public Button btnGoOn, btnClose;

    private void Start()
    {
        btnGoOn = transform.Find("btnGoOn").GetComponent<Button>();
        btnClose = transform.Find("btnClose").GetComponent<Button>();

        btnGoOn.onClick.AddListener(OnGoOn);
        btnClose.onClick.AddListener(OnClose);
    }

    private void OnClose()
    {
        gameObject.SetActive(false);
    }

    private void OnGoOn()
    {
        gameObject.SetActive(false);
        GameManager.Instance.peerClient.SendRequest((short)OpCode.GoOn, null);
    }
}

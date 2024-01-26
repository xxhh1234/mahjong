using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    // top
    public Image imgHead;
    public Text txtUsername, txtId, txtCoin,txtDiamond;
    public Button btnAddCoin, btnAddDiamond, btnAddPrice;
    // mid
    public Button btnCreateRoom, btnJoinRoom;
    // bottom
    public Button btnActivity, btnTask, btnShop, btnIntraCity, btnSetting;
    // panel
    public GameObject goCreateRoomPanel, goJoinRoomPanel;

    private void Awake()
    {
        imgHead = transform.Find("imgTop/imgHead").GetComponent<Image>();
        txtUsername = transform.Find("imgTop/imgHead/txtUsername").GetComponent<Text>();
        txtId = transform.Find("imgTop/imgHead/txtId").GetComponent<Text>();
        txtCoin = transform.Find("imgTop/imgCoin/txtCoin").GetComponent<Text>();
        txtDiamond = transform.Find("imgTop/imgDiamond/txtDiamond").GetComponent<Text>();
        btnCreateRoom = transform.Find("imgMid/btnCreateRoom").GetComponent<Button>();
        btnJoinRoom = transform.Find("imgMid/btnJoinRoom").GetComponent<Button>();
        goCreateRoomPanel = transform.Find("CreateRoomPanel").gameObject;
        goJoinRoomPanel = transform.Find("JoinRoomPanel").gameObject;
       
        btnCreateRoom.onClick.AddListener(OnCreateRoomPanel);
        btnJoinRoom.onClick.AddListener(OnJoinRoomPanel);
    }

    private void OnCreateRoomPanel()
    {
        goCreateRoomPanel.SetActive(true);
    }

    private void OnJoinRoomPanel()
    {
        goJoinRoomPanel.SetActive(true);
    }
}

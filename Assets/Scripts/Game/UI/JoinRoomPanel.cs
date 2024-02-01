using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XH
{
    public class JoinRoomPanel : MonoBehaviour
{
    // Input
    public Text txtInput1, txtInput2, txtInput3, txtInput4, txtInput5;
    public Button btn0, btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8, btn9;
    /// <summary>
    /// 清空输入按钮，删除输入按钮，关闭加入房间界面按钮，加入房间界面按钮
    /// </summary>
    public Button btnClear, btnDelete, btnClose, btnJoin;

    private void Start()
    {
        txtInput1 = transform.Find("imgJoinRoom/imgInput1/txtInput1").GetComponent<Text>();
        txtInput2 = transform.Find("imgJoinRoom/imgInput2/txtInput2").GetComponent<Text>();
        txtInput3 = transform.Find("imgJoinRoom/imgInput3/txtInput3").GetComponent<Text>();
        txtInput4 = transform.Find("imgJoinRoom/imgInput4/txtInput4").GetComponent<Text>();
        txtInput5 = transform.Find("imgJoinRoom/imgInput5/txtInput5").GetComponent<Text>();

        btn0 = transform.Find("imgJoinRoom/btn0").GetComponent<Button>();
        btn1 = transform.Find("imgJoinRoom/btn1").GetComponent<Button>();
        btn2 = transform.Find("imgJoinRoom/btn2").GetComponent<Button>();
        btn3 = transform.Find("imgJoinRoom/btn3").GetComponent<Button>();
        btn4 = transform.Find("imgJoinRoom/btn4").GetComponent<Button>();
        btn5 = transform.Find("imgJoinRoom/btn5").GetComponent<Button>();
        btn6 = transform.Find("imgJoinRoom/btn6").GetComponent<Button>();
        btn7 = transform.Find("imgJoinRoom/btn7").GetComponent<Button>();
        btn8 = transform.Find("imgJoinRoom/btn8").GetComponent<Button>();
        btn9 = transform.Find("imgJoinRoom/btn9").GetComponent<Button>();
        btnClear = transform.Find("imgJoinRoom/btnClear").GetComponent<Button>();
        btnDelete = transform.Find("imgJoinRoom/btnDelete").GetComponent<Button>();
        btnClose = transform.Find("imgJoinRoom/btnClose").GetComponent<Button>();
        btnJoin = transform.Find("imgJoinRoom/btnJoin").GetComponent<Button>();


        btn0.onClick.AddListener(OnButton0);
        btn1.onClick.AddListener(OnButton1);
        btn2.onClick.AddListener(OnButton2);
        btn3.onClick.AddListener(OnButton3);
        btn4.onClick.AddListener(OnButton4);
        btn5.onClick.AddListener(OnButton5);
        btn6.onClick.AddListener(OnButton6);
        btn7.onClick.AddListener(OnButton7);
        btn8.onClick.AddListener(OnButton8);
        btn9.onClick.AddListener(OnButton9);
        btnClear.onClick.AddListener(OnButtonClear);
        btnDelete.onClick.AddListener(OnButtonDelete);
        btnClose.onClick.AddListener(OnButtonClose);
        btnJoin.onClick.AddListener(OnButtonJoin);

    }

    private void OnButton0()
    {
        if (txtInput1.text == "" || txtInput1.text == null)
        {
            txtInput1.text = 0.ToString();
        }
        else if (txtInput2.text == "" || txtInput2.text == null)
        {
            txtInput2.text = 0.ToString();
        }
        else if (txtInput3.text == "" || txtInput3.text == null)
        {
            txtInput3.text = 0.ToString();
        }
        else if (txtInput4.text == "" || txtInput4.text == null)
        {
            txtInput4.text = 0.ToString();
        }
        else if (txtInput5.text == "" || txtInput5.text == null)
        {
            txtInput5.text = 0.ToString();
        }
    }
    private void OnButton1()
    {
        if(txtInput1.text == "" || txtInput1.text == null)
        {
            txtInput1.text = 1.ToString();
        }
        else if(txtInput2.text == "" || txtInput2.text == null)
        {
            txtInput2.text = 1.ToString();
        }
        else if (txtInput3.text == "" || txtInput3.text == null)
        {
            txtInput3.text = 1.ToString();
        }
        else if (txtInput4.text == "" || txtInput4.text == null)
        {
            txtInput4.text = 1.ToString();
        }
        else if (txtInput5.text == "" || txtInput5.text == null)
        {
            txtInput5.text = 1.ToString();
        }
    }
    private void OnButton2()
    {
        if (txtInput1.text == "" || txtInput1.text == null)
        {
            txtInput1.text = 2.ToString();
        }
        else if (txtInput2.text == "" || txtInput2.text == null)
        {
            txtInput2.text = 2.ToString();
        }
        else if (txtInput3.text == "" || txtInput3.text == null)
        {
            txtInput3.text = 2.ToString();
        }
        else if (txtInput4.text == ""  ||  txtInput4.text == null)
        {
            txtInput4.text = 2.ToString();
        }
        else if (txtInput5.text == "" || txtInput5.text == null)
        {
            txtInput5.text = 2.ToString();
        }
    }
    private void OnButton3()
    {
        if (txtInput1.text == "" || txtInput1.text == null)
        {
            txtInput1.text = 3.ToString();
        }
        else if (txtInput2.text == "" || txtInput2.text == null)
        {
            txtInput2.text = 3.ToString();
        }
        else if (txtInput3.text == "" || txtInput3.text == null)
        {
            txtInput3.text = 3.ToString();
        }
        else if (txtInput4.text == "" || txtInput4.text == null)
        {
            txtInput4.text = 3.ToString();
        }
        else if (txtInput5.text == "" || txtInput5.text == null)
        {
            txtInput5.text = 3.ToString();
        }
    }
    private void OnButton4()
    {
        if (txtInput1.text == "" || txtInput1.text == null)
        {
            txtInput1.text = 4.ToString();
        }
        else if (txtInput2.text == "" || txtInput2.text == null)
        {
            txtInput2.text = 4.ToString();
        }
        else if (txtInput3.text == "" || txtInput3.text == null)
        {
            txtInput3.text = 4.ToString();
        }
        else if (txtInput4.text == "" || txtInput4.text == null)
        {
            txtInput4.text = 4.ToString();
        }
        else if (txtInput5.text == "" || txtInput5.text == null)
        {
            txtInput5.text = 4.ToString();
        }
    }
    private void OnButton5()
    {
        if (txtInput1.text == "" || txtInput1.text == null)
        {
            txtInput1.text = 5.ToString();
        }
        else if (txtInput2.text == "" || txtInput2.text == null)
        {
            txtInput2.text = 5.ToString();
        }
        else if (txtInput3.text == "" || txtInput3.text == null)
        {
            txtInput3.text = 5.ToString();
        }
        else if (txtInput4.text == "" || txtInput4.text == null)
        {
            txtInput4.text = 5.ToString();
        }
        else if (txtInput5.text == "" || txtInput5.text == null)
        {
            txtInput5.text = 5.ToString();
        }
    }
    private void OnButton6()
    {
        if (txtInput1.text == "" || txtInput1.text == null)
        {
            txtInput1.text = 6.ToString();
        }
        else if (txtInput2.text == "" || txtInput2.text == null)
        {
            txtInput2.text = 6.ToString();
        }
        else if (txtInput3.text == "" || txtInput3.text == null)
        {
            txtInput3.text = 6.ToString();
        }
        else if (txtInput4.text == "" || txtInput4.text == null)
        {
            txtInput4.text = 6.ToString();
        }
        else if (txtInput5.text == "" || txtInput5.text == null)
        {
            txtInput5.text = 6.ToString();
        }
    }
    private void OnButton7()
    {
        if (txtInput1.text == "" || txtInput1.text == null)
        {
            txtInput1.text = 7.ToString();
        }
        else if (txtInput2.text == "" || txtInput2.text == null)
        {
            txtInput2.text = 7.ToString();
        }
        else if (txtInput3.text == "" || txtInput3.text == null)
        {
            txtInput3.text = 7.ToString();
        }
        else if (txtInput4.text == "" || txtInput4.text == null)
        {
            txtInput4.text = 7.ToString();
        }
        else if (txtInput5.text == "" || txtInput5.text == null)
        {
            txtInput5.text = 7.ToString();
        }
    }
    private void OnButton8()
    {
        if (txtInput1.text == "" || txtInput1.text == null)
        {
            txtInput1.text = 8.ToString();
        }
        else if (txtInput2.text == "" || txtInput2.text == null)
        {
            txtInput2.text = 8.ToString();
        }
        else if (txtInput3.text == "" || txtInput3.text == null)
        {
            txtInput3.text = 8.ToString();
        }
        else if (txtInput4.text == "" || txtInput4.text == null)
        {
            txtInput4.text = 8.ToString();
        }
        else if (txtInput5.text == "" || txtInput5.text == null)
        {
            txtInput5.text = 8.ToString();
        }
    }
    private void OnButton9()
    {
        if (txtInput1.text == "" || txtInput1.text == null)
        {
            txtInput1.text = 9.ToString();
        }
        else if (txtInput2.text == "" || txtInput2.text == null)
        {
            txtInput2.text = 9.ToString();
        }
        else if (txtInput3.text == "" || txtInput3.text == null)
        {
            txtInput3.text = 9.ToString();
        }
        else if (txtInput4.text == "" || txtInput4.text == null)
        {
            txtInput4.text = 9.ToString();
        }
        else if (txtInput5.text == "" || txtInput5.text == null)
        {
            txtInput5.text = 9.ToString();
        }
    }

    private void OnButtonClear()
    {
        txtInput1.text = "";
        txtInput2.text = "";
        txtInput3.text = "";
        txtInput4.text = "";
        txtInput5.text = "";
    }

    private void OnButtonDelete()
    {
        if(txtInput5.text != "" && txtInput5.text != null)
        {
            txtInput5.text = "";
        }
        else if(txtInput4.text != "" && txtInput4.text != null)
        {
            txtInput4.text = "";
        }
        else if (txtInput3.text != "" && txtInput3.text != null)
        {
            txtInput3.text = "";
        }
        else if (txtInput2.text != "" && txtInput2.text != null)
        {
            txtInput2.text = "";
        }
        else if (txtInput1.text != "" && txtInput1.text != null)
        {
            txtInput1.text = "";
        }
    }

    private void OnButtonClose()
    {
        gameObject.SetActive(false);
    }

    private void OnButtonJoin()
    {
        if(txtInput1.text != "" && txtInput1.text != null
            && txtInput2.text != "" && txtInput2.text != null
            && txtInput2.text != "" && txtInput2.text != null
            && txtInput3.text != "" && txtInput2.text != null
            && txtInput4.text != "" && txtInput2.text != null
            && txtInput5.text != "" && txtInput2.text != null)
        {
            string roomName = txtInput1.text + txtInput2.text + txtInput3.text + txtInput4.text + txtInput5.text;
            Dictionary<short, object> dict = new Dictionary<short, object>();
            dict.Add(ParameterCode.roomName, roomName);
            NetManager.peerClient.SendRequest((short)OpCode.JoinRoomPanel, dict);
        }

    }
}
}


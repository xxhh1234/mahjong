using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public InputField inputId;
    public Button btnLogin;
    private readonly string ip = "127.0.0.1";
    private readonly int port = 4999;

    private void Start()
    {
        inputId = transform.Find("inputId").GetComponent<InputField>();
        btnLogin = transform.Find("btnLogin").GetComponent<Button>();
        btnLogin.onClick.AddListener(OnButtonLogin);
    }

    private void OnEnable()
    {
        StartCoroutine(Connect());
    }

    IEnumerator Connect()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.peerClient.Connect(ip, port);
    }

    /// <summary>
    /// btnLogin的回调函数
    /// </summary>
    public void OnButtonLogin()
    {
        int id = 0;
        if(inputId.text != null)
        {
            id = int.Parse(inputId.text);
        }
        Dictionary<short, object> dict = new Dictionary<short, object>();
        dict.Add(ParameterCode.id, id);
        GameManager.Instance.peerClient.SendRequest((short)OpCode.Login, dict);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogPanel : MonoBehaviour
{
    public static Text txtLog;

    private void Start()
    {
        txtLog = transform.Find("txtLog").GetComponent<Text>();
    }

    public static void Log(string str)
    {
        // txtLog.text += "\n" + str;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DingQuePanel : MonoBehaviour
{
    public Button btnCharacter,  btnBamboo, btnCircle;
    
    private void Start()
    {
        btnCharacter = transform.Find("btnCharacter").GetComponent<Button>();
        btnBamboo = transform.Find("btnBamboo").GetComponent<Button>();
        btnCircle = transform.Find("btnCircle").GetComponent<Button>();

        btnCharacter.onClick.AddListener(OnButtonCharacter);
        btnBamboo.onClick.AddListener(OnButtonBamboo);
        btnCircle.onClick.AddListener(OnButtonCircle);
    }



    public void OnButtonCharacter()
    {
        GameManager.Instance.room.playerProperties[0].QueTileType = TileType.character.ToString();
        Dictionary<short, object> dictDingQue = new Dictionary<short, object>();
        dictDingQue.Add(ParameterCode.queTileType, TileType.character);
        GameManager.Instance.peerClient.SendRequest((short)OpCode.DingQue, dictDingQue);
        gameObject.SetActive(false);
    }

    public void OnButtonBamboo()
    {
        GameManager.Instance.room.playerProperties[0].QueTileType = TileType.bamboo.ToString();
        Dictionary<short, object> dictDingQue = new Dictionary<short, object>();
        dictDingQue.Add(ParameterCode.queTileType, TileType.bamboo);
        GameManager.Instance.peerClient.SendRequest((short)OpCode.DingQue, dictDingQue);
        gameObject.SetActive(false);
    }

    public void OnButtonCircle()
    {
        GameManager.Instance.room.playerProperties[0].QueTileType = TileType.circle.ToString();
        Dictionary<short, object> dictDingQue = new Dictionary<short, object>();
        dictDingQue.Add(ParameterCode.queTileType, TileType.circle);
        GameManager.Instance.peerClient.SendRequest((short)OpCode.DingQue, dictDingQue);
        gameObject.SetActive(false);
    }
}

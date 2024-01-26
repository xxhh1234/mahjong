using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickTileController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Tile tile;
    public bool  isSelect = false;

    public void OnDeselect(BaseEventData eventData)
    {
        if(!RoomPanel.isCanSelectTile)
            transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if(!RoomPanel.isCanSelectTile)
        {
            transform.localPosition += new Vector3(0, 75, 0);
        }
        else
        {
            if (!isSelect)
            {
                isSelect = true;
                RoomPanel.selectTileList.Add(tile.tileNum);
                transform.localPosition += new Vector3(0, 75, 0);
            }
            else
            {
                isSelect = false;
                transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
                RoomPanel.selectTileList.Remove(tile.tileNum);
            }
        }
    }

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(OnButtonClick);
        tile = gameObject.GetComponent<Tile>();
    }
    private void OnButtonClick()
    {
        if(RoomPanel.isCanPlayTile)
        {
            // 出牌
            Dictionary<short, object>  dict = new Dictionary<short, object>();
            dict[ParameterCode.playTileNum] = tile.tileNum;
            dict[ParameterCode.isDrawTile] = tile.isDrawTile;
            dict[ParameterCode.isGangHouPlayTile] = RoomPanel.isGangHouPlayTile;
            GameManager.Instance.peerClient.SendRequest((short)OpCode.PlayTile, dict);

            // 整理手牌
            GameManager.Instance.ThrowTile(tile.isDrawTile, tile.tileNum);
            RoomPanel.isCanPlayTile = false;
            ObjectPool.Instance.CollectGameObject(gameObject);
        }
    }
}

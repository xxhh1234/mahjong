using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XH;

namespace mahjong
{
    class SelectTileCtrl : Ctrl
    {
        public override void Init(string name)
        {
            base.Init(name);

            EventManager.Instance.AddListener(ServerToClient.SendSelectTileToAll, (Dictionary<short, object> eventDict) =>
            {
                ThreadManager.ExecuteUpdate(() => { GameManager.Instance.StartCoroutine(IESelectTileMove(eventDict)); });
            });
        }

        public override void UnInit()
        {
            base.UnInit();

            EventManager.Instance.RemoveListener(ServerToClient.SendSelectTileToAll, (Dictionary<short, object> eventDict) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    GameManager.Instance.StartCoroutine(IESelectTileMove(eventDict));
                });
            });

        }

        private void OnButtonSure()
        {
            SelectTileData data = (SelectTileData)ClientDataManager.Instance.GetData("SelectTileData");
            if (!data.isOnSelectTile) 
                return;
            if(data.selectTiles.Count != 3)
                return;
            if (!(GetTileType(data.selectTiles[0]).Equals(GetTileType(data.selectTiles[1])))) 
                return;
            if (!(GetTileType(data.selectTiles[1]).Equals(GetTileType(data.selectTiles[2])))) 
                return;
            XH.Logger.XH_LOG("选中的三张手牌为" + ((TileTypeTileNum)data.selectTiles[0]).ToString() +
               ((TileTypeTileNum)data.selectTiles[1]).ToString() +
           ((TileTypeTileNum)data.selectTiles[2]).ToString());

            AudioManager.Instance.PlayAudio("click1");
            // 1. 选牌结束
            ClientDataManager.Instance.RefreshData("SelectTileData", new SelectTileData(0));
            // 2.关闭选择3张手牌界面
            UIManager.Instance.CloseView("SelectTileView");
            // 3.删除手牌中的3张牌并向服务器请求3张牌
            Dictionary<short, object> dictSelectTile = new Dictionary<short, object>();
            dictSelectTile.Add(ParameterCode.SelectTileData, data);
            NetManager.peerClient.SendRequestAsync(ClientToServer.SelectTile, dictSelectTile);
            // 4. 删除本家手牌，获取非选中手牌的数字并生成
            ushort[] tile = new ushort[14];
            ushort cnt = 0;
            HandTileData handTileData = (HandTileData)ClientDataManager.Instance.GetData("HandTileData");
            for(ushort i = 0; i < handTileData.tileCnt; ++i)
            {
                ushort tileNum = handTileData.handTile[i];
                if(tileNum != data.selectTiles[0] &&  tileNum != data.selectTiles[1] &&  tileNum != data.selectTiles[2])
                    tile[cnt++] = tileNum;
            }
            handTileData.handTileId = 0;
            handTileData.tileCnt = cnt;
            handTileData.handTile = tile;
            ClientDataManager.Instance.RefreshData("HandTileData", handTileData, DataEvent.ChangeHandTileData);
        }


        public IEnumerator IESelectTileMove(Dictionary<short, object> eventDict)
        {
            // 1. 解析数据
            SelectTileData selectTileData = Util.Convert<SelectTileData>(eventDict[ParameterCode.SelectTileData]);
            List<ushort> selectTiles = selectTileData.selectTiles;
            XH.Logger.XH_LOG("得到的三张手牌为" + ((TileTypeTileNum)selectTiles[0]).ToString() +
                ((TileTypeTileNum)selectTiles[1]).ToString() +
            ((TileTypeTileNum)selectTiles[2]).ToString());
            // 2. 通知TileModel刷新HandTileView
            ushort[] tile = new ushort[14];
            ushort cnt = 0;
            for(int i = 0; i < selectTiles.Count; ++i)
                tile[cnt++] = selectTiles[i];
            HandTileData handTileData = (HandTileData)ClientDataManager.Instance.GetData("HandTileData");
            for(int i = 0; i < handTileData.tileCnt; ++i)
                tile[cnt++] = handTileData.handTile[i];
            handTileData.handTileId = 0;
            handTileData.tileCnt = cnt;
            handTileData.handTile = tile;
            ClientDataManager.Instance.RefreshData("HandTileData", handTileData, DataEvent.ChangeHandTileData);

            yield return new WaitForSeconds(0.5f);

            // 3.开启选择定缺界面并开启倒计时
            View roomView = UIManager.Instance.GetView("RoomView");
            UIManager.Instance.ShowView("DingQueView", roomView.gameObject.transform);
        }
        public TileType GetTileType(int tileNum)
        {
            int wan1Num = TileTypeTileNum.wan_1.GetHashCode();
            int wan9Num = TileTypeTileNum.wan_9.GetHashCode();
            int tiao1Num = TileTypeTileNum.tiao_1.GetHashCode();
            int tiao9Num = TileTypeTileNum.tiao_9.GetHashCode();
            int tong1Num = TileTypeTileNum.tong_1.GetHashCode();
            int tong9Num = TileTypeTileNum.tong_9.GetHashCode();
            if(tileNum >= wan1Num && tileNum <= wan9Num)
                return TileType.wan;
            if(tileNum >= tiao1Num && tileNum <= tiao9Num)
                return TileType.tiao;
            if(tileNum >= tong1Num && tileNum <= tong9Num)
                return TileType.tong;
            return TileType.none;
        }

    
    }
}

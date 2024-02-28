using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XH;

namespace mahjong
{
    class DingQueCtrl : Ctrl
    {
        public override void Init(string name)
        {
            base.Init(name);

            EventManager.Instance.AddListener(ServerToClient.SendQueTileTypeToOther, (Dictionary<short, object> eventDict) =>
            {
                OnOtherDingQue(eventDict);
            });
        }

        public override void UnInit()
        {
            base.UnInit();

            EventManager.Instance.RemoveListener(ServerToClient.SendQueTileTypeToOther, (Dictionary<short, object> eventDict) =>
            {
                OnOtherDingQue(eventDict);
            });
        }

        public void OnExcuteDingQueData(DingQueData dingQueData)
        {
            PlayerData playerData = (PlayerData)ClientDataManager.Instance.GetData("PlayerData");
            for (int i = 0; i < playerData.playerCnt; ++i)
            {
                if (dingQueData.dingQueId == playerData.playerDatas[i].playerId)
                {
                    dingQueData.dingQueId = (ushort)i;
                    if(i == 0)
                    { 
                        ClientDataManager.Instance.RefreshData("DingQueData", dingQueData, DataEvent.ChangeDingQueData);
                        HandTileData handTileData =  (HandTileData)ClientDataManager.Instance.GetData("HandTileData");
                        handTileData.handTileId = 0;
                        ClientDataManager.Instance.RefreshData("HandTileData", handTileData, DataEvent.ChangeHandTileData);
                    }
                    else
                        EventManager.Instance.Broadcast(UIEvent.RefreshQueTileType, dingQueData as ValueType);
                    break;
                }
            }
        }
        public void OnOtherDingQue(Dictionary<short, object> eventDict)
        {
            DingQueData dingQueData = Util.Convert<DingQueData>(eventDict[ParameterCode.DingQueData]);
            OnExcuteDingQueData(dingQueData);
        }

        private void OnButtonWan()
        {
            AudioManager.Instance.PlayAudio("click1");
            DingQueData dingQueData = new DingQueData();
            dingQueData.queTileType = TileType.wan;
            Dictionary<short, object> dictDingQue = new Dictionary<short, object>();
            dictDingQue.Add(ParameterCode.DingQueData, dingQueData);
            NetManager.peerClient.SendRequestAsync(ClientToServer.DingQue, dictDingQue, (Dictionary<short, object> respDict) =>
            {
                dingQueData = Util.Convert<DingQueData>(respDict[ParameterCode.DingQueData]);
                OnExcuteDingQueData(dingQueData);
            });
            UIManager.Instance.CloseView("DingQueView");
        }
        private void OnButtonTiao()
        {
            AudioManager.Instance.PlayAudio("click2");
            DingQueData dingQueData = new DingQueData();
            dingQueData.queTileType = TileType.tiao;
            Dictionary<short, object> dictDingQue = new Dictionary<short, object>();
            dictDingQue.Add(ParameterCode.DingQueData, dingQueData);
            NetManager.peerClient.SendRequestAsync(ClientToServer.DingQue, dictDingQue, (Dictionary<short, object> respDict) =>
            {
                dingQueData = Util.Convert<DingQueData>(respDict[ParameterCode.DingQueData]);
                OnExcuteDingQueData(dingQueData);
            });
            UIManager.Instance.CloseView("DingQueView");
        }
        private void OnButtonTong()
        {
            AudioManager.Instance.PlayAudio("click3");
            DingQueData dingQueData = new DingQueData();
            dingQueData.queTileType = TileType.tong;
            Dictionary<short, object> dictDingQue = new Dictionary<short, object>();
            dictDingQue.Add(ParameterCode.DingQueData, dingQueData);
            NetManager.peerClient.SendRequestAsync(ClientToServer.DingQue, dictDingQue, (Dictionary<short, object> respDict) =>
            {
                dingQueData = Util.Convert<DingQueData>(respDict[ParameterCode.DingQueData]);
                OnExcuteDingQueData(dingQueData);
            });
            UIManager.Instance.CloseView("DingQueView");
        }
    }
}

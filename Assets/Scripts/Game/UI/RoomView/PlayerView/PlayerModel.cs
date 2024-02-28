using System;
using XH;

namespace mahjong
{
    struct SinglePlayerData
    {
        public PlayerPosition playerPos;
        public ushort sex;
        public bool isMaster;
        public string playerName;
        public uint score;
        public bool isReady;
        public ushort playerId;
    }
    struct PlayerData
    {
        public SinglePlayerData[] playerDatas;
        public ushort playerCnt;


        public PlayerData(uint size)
        {
            playerDatas = new SinglePlayerData[size];
            playerCnt = 0;
        }
    }
    struct DingQueData
    {
        public ushort dingQueId;
        public TileType queTileType;
    }
    struct TimerData
    {
        public ushort timerId;
        public bool isPlay;
    }


    class PlayerModel : Model
    {
        public override void Init(string name)
        {
            base.Init(name);

            ClientDataManager.Instance.AddData("PlayerData", new PlayerData(4));
            ClientDataManager.Instance.AddData("DingQueData", new DingQueData());

            EventManager.Instance.AddListener(DataEvent.ChangePlayerData, (ValueType value) => 
            {
                SortPlayerData((PlayerData)value);
            });
            InitEvent(DataEvent.ChangeDingQueData, UIEvent.RefreshQueTileType);
        }

        public override void UnInit()
        {
            base.UnInit();
            EventManager.Instance.RemoveListener(DataEvent.ChangePlayerData, (ValueType value) =>
            {
                SortPlayerData((PlayerData)value);
            });
            UnInitEvent(DataEvent.ChangeDingQueData);
        }

        private void SortPlayerData(PlayerData playerData)
        {
            UserData userData = (UserData)ClientDataManager.Instance.GetData("UserData");
            int ownPos = 0;
            for (int i = 0; i < playerData.playerDatas.Length; ++i)
            {
                if (playerData.playerDatas[i].playerName == userData.username)
                {
                    ownPos = playerData.playerDatas[i].playerPos.GetHashCode();
                    break;
                }
            }
            SinglePlayerData[] newPlayerDatas = new SinglePlayerData[playerData.playerDatas.Length];
            for (int i = 0; i < playerData.playerDatas.Length; ++i)
            {
                if (playerData.playerDatas[i].playerName == null)
                    continue;
                int otherPos = (int)playerData.playerDatas[i].playerPos;
                int j = otherPos - ownPos +  (otherPos - ownPos < 0 ? 4 : 0);
                newPlayerDatas[j] = playerData.playerDatas[i];
            }
            playerData.playerDatas = newPlayerDatas;
            ClientDataManager.Instance.RefreshData("PlayerData", playerData);
            EventManager.Instance.Broadcast(UIEvent.RefreshPlayerView, playerData as ValueType);
        }
    }
}

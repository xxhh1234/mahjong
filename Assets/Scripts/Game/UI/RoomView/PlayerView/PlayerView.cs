using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using XH;

namespace mahjong
{
    class PlayerView : View
    {
        private Image  imgController;
        private string[] playerNames =
        {
            "own/",
            "down/",
            "opposite/",
            "up/"
        };
        private Image[] imgHeads = new Image[4];
        private Image[] imgMasters = new Image[4];
        private Text[] txtQues = new Text[4]; 
        private Text[] txtUsernames = new Text[4]; 
        private Text[] txtScores = new Text[4];
        private GameObject[] goRights = new GameObject[4];
        private Button btnReady;
        private GameObject timer;

        private Sprite[]  sprControllers = new Sprite[4];
        private Sprite[]  sprHeads = new Sprite[3];
        private float[,] timerPos =
        {
            {   0, -200 },
            { 200,    0 },
            {   0,  200 },
            {-200,    0 }
        };

        public override void Init(string name)
        {
            base.Init(name);

            imgController = transform.Find("imgController").GetComponent<Image>();
            for (int i = 0; i < playerNames.Length; ++i)
            {
                imgHeads[i] = transform.Find(playerNames[i] + "imgHead").GetComponent<Image>();
                imgMasters[i] = transform.Find(playerNames[i] + "imgHead/imgMaster").GetComponent<Image>();
                txtQues[i] = transform.Find(playerNames[i] + "imgHead/txtQue").GetComponent<Text>();
                txtUsernames[i] = transform.Find(playerNames[i] + "panUsername/txtUsername").GetComponent<Text>();
                txtScores[i] = transform.Find(playerNames[i] + "panScore/txtScore").GetComponent<Text>();
                goRights[i] = transform.Find(playerNames[i] + "goRight").gameObject;
       
            }
            btnReady = transform.Find("btnReady").GetComponent<Button>();
            btnReady.onClick.AddListener(() =>
            {
                Action action = controller.GetMethod("OnButtonReady") as Action;
                action();
            });
            timer = transform.Find("timer").gameObject;

            Texture2D texture;
            ResourceManager.Instance.Load(out texture, "DNXB");
            sprControllers[0] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            ResourceManager.Instance.Load(out texture, "NXBD");
            sprControllers[1] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            ResourceManager.Instance.Load(out texture, "XBDN");
            sprControllers[2] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            ResourceManager.Instance.Load(out texture, "BDNX");
            sprControllers[3] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            ResourceManager.Instance.Load(out texture, "DefaultHead");
            sprHeads[0] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            ResourceManager.Instance.Load(out texture, "Boy");
            sprHeads[1] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            ResourceManager.Instance.Load(out texture, "Girl");
            sprHeads[2] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));

            EventManager.Instance.AddListener(UIEvent.RefreshPlayerView, (ValueType value) =>
            { ThreadManager.ExecuteUpdate(() => { RefreshPlayerView(value); });});
            EventManager.Instance.AddListener(UIEvent.RefreshQueTileType, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() => { RefreshQueTileType(value); });
            });
            EventManager.Instance.AddListener(UIEvent.ShowTimerAnim, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() => { ShowTimerAnim(value); });
            });

        }

        public override void UnInit()
        {
            base.UnInit();

            EventManager.Instance.RemoveListener(UIEvent.RefreshPlayerView, (ValueType value) =>
            { ThreadManager.ExecuteUpdate(() => { RefreshPlayerView(value); }); });
            EventManager.Instance.RemoveListener(UIEvent.RefreshQueTileType, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() => { RefreshQueTileType(value); });
            });
            EventManager.Instance.RemoveListener(UIEvent.ShowTimerAnim, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() => { ShowTimerAnim(value); });
            });
        }

        private void RefreshPlayerView(ValueType value)
        {
            // 1. 解析数据
            PlayerData playerData = (PlayerData)value;
            SinglePlayerData[] playerDatas = playerData.playerDatas;
            // 2. 设置imgController
            imgController.sprite = sprControllers[playerDatas[0].playerPos.GetHashCode()];
            // 3. 设置头像，庄家，玩家名，分数，准备按钮
            for (int i = 0; i < playerDatas.Length; ++i)
            {
                if (playerDatas[i].playerName == null)
                    continue;
                imgHeads[i].sprite = sprHeads[playerDatas[i].sex];
                imgMasters[i].gameObject.SetActive(playerDatas[i].isMaster);
                // PlayerName, Score
                if (i == 0 || i == 2)
                {
                    txtUsernames[i].text = "昵称:" + playerDatas[i].playerName;
                    txtScores[i].text = "分数:" + playerDatas[i].score;
                }
                else
                {
                    txtUsernames[i].text = "昵称:" +  "\n" + playerDatas[i].playerName;
                    txtScores[i].text = "分数:" + "\n" + playerDatas[i].score;
                }
                // btnReady
                if(i == 0)
                {   
                    RoomState roomState = ((RoomData)ClientDataManager.Instance.GetData("RoomData")).roomState;
                    if(roomState == RoomState.Idle)
                        btnReady.gameObject.SetActive(!playerDatas[i].isReady);
                    else
                        btnReady.gameObject.SetActive(false);
                }
                // goRights
                goRights[i].SetActive(playerDatas[i].isReady);
            }
        }
        private void RefreshQueTileType(ValueType value)
        {
            DingQueData dingQueData = (DingQueData)value;
            ushort dingQueId = dingQueData.dingQueId;
            TileType type = dingQueData.queTileType;
            if(type == TileType.wan)
                txtQues[dingQueId].text = "万";
            else if(type == TileType.tiao)
                txtQues[dingQueId].text = "条";
            else if(type == TileType.tong)
                txtQues[dingQueId].text = "筒";
            else
                txtQues[dingQueId].text = "";
        } 
        private void ShowTimerAnim(ValueType value)
        {
            TimerData timerData = (TimerData)value;
            ushort timerId = timerData.timerId;
            bool isPlay = timerData.isPlay;
            if (!isPlay)
            {
                timer.SetActive(false);
                return;
            }
            timer.SetActive(true);
            Animator animator = timer.GetComponent<Animator>();
            timer.transform.localPosition = new Vector3(timerPos[timerId, 0], timerPos[timerId, 1], -10);
            animator.Play("timerAnim", 0, 0f);
        }
    }
}

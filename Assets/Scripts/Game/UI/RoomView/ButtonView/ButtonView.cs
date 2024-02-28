using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XH;

namespace mahjong
{
    class ButtonView : View
    {
        private GameObject goChiPengGangHu, goGuo;
        private GameObject[] goCPGArea = new GameObject[4];

        private string[] playerNames =
        {
            "own",
            "down",
            "opposite",
            "up"
        };
        private string[] buttonsName;
        private float[,] CPG_INTERVAL =
        {
            { 230,    0},
            {   0,  170},
            {-230,    0},
            {   0, -170}
        };
        private float[,] PENG_TILE_INTERVAL =
        {
            { 70,   0 },
            {  0,  50 },
            {-70,   0 },
            {  0, -50 }
        };
        private float[,] GANG_TILE_POS =
        {
            {  70,  30 },
            { -30,  50 },
            { -70, -30 },
            {  30, -50 }
        };

        public override void Init(string name)
        {
            base.Init(name);

            goChiPengGangHu = transform.Find("goChiPengGangHu").gameObject;
            goGuo = transform.Find("goGuo").gameObject;
            for (int i = 0; i < playerNames.Length; ++i)
                goCPGArea[i] = transform.Find(playerNames[i] + "/goCPGArea").gameObject;

            for (int i = 0; i < 5; ++i)
            {
                GameObject buttons = GameObjectManager.Instance.GetGO("Buttons");
                int n = buttons.transform.childCount;
                buttonsName = new string[n];
                for (int j = 0; j < n; ++j)
                {
                    GameObject go = buttons.transform.GetChild(0).gameObject;
                    buttonsName[j] = go.name;
                    GameObjectManager.Instance.AddGo(go.name, go);
                }
                GameObject.DestroyImmediate(buttons);
            }

            EventManager.Instance.AddListener(UIEvent.RefreshButtonView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                { RefreshButtonView(value);});
            });
            EventManager.Instance.AddListener(UIEvent.RefreshHuTileView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                { RefreshHuTileView(value); });
            });
            EventManager.Instance.AddListener(UIEvent.RefreshPengGangView, (ValueType value) =>
            { 
                ThreadManager.ExecuteUpdate(() => 
                { RefreshPengGangView(value); }); 
            });
        }

        public override void UnInit()
        {
            base.UnInit();

            EventManager.Instance.RemoveListener(UIEvent.RefreshButtonView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                { RefreshButtonView(value); });
            });
            EventManager.Instance.RemoveListener(UIEvent.RefreshHuTileView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                { RefreshHuTileView(value); });
            });
            EventManager.Instance.RemoveListener(UIEvent.RefreshPengGangView, (ValueType value) =>
            { ThreadManager.ExecuteUpdate(() => 
                { RefreshPengGangView(value); }); 
            });
        }

        private void RefreshButtonView(ValueType value)
        {
            ButtonData buttonData = (ButtonData)value;
            if(buttonData.param == 0)
            {
                int n = goChiPengGangHu.transform.childCount;
                for (int i = n - 1; i >= 0; --i)
                {
                    int m = goChiPengGangHu.transform.GetChild(i).childCount;
                    for (int j = m - 1; j >= 0; --j)
                        GameObjectManager.Instance.CollectGO(goChiPengGangHu.transform.GetChild(i).GetChild(j).gameObject);
                    GameObjectManager.Instance.CollectGO(goChiPengGangHu.transform.GetChild(i).gameObject);
                }
                n = goGuo.transform.childCount;
                for (int i = n - 1; i >= 0; --i)
                {
                    int m = goGuo.transform.GetChild(i).childCount;
                    for (int j = m - 1; j >= 0; --j)
                    {
                        GameObjectManager.Instance.CollectGO(goGuo.transform.GetChild(i).GetChild(j).gameObject);
                    }
                    GameObjectManager.Instance.CollectGO(goGuo.transform.GetChild(i).gameObject);
                }
                return;
            }
            // 1. 创建过按钮
            CreateButton(4, goGuo.transform, ParameterCode.none, null, buttonData.opCode);
            // 2. 创建空物体并将按钮挂到空物体上
            GameObject empty = GameObjectManager.Instance.GetGO("Empty");
            empty.transform.SetParent(goChiPengGangHu.transform, false);
            empty.transform.localPosition = new Vector3(0, (goChiPengGangHu.transform.childCount - 1) * 120, 0);
            CreateButton(buttonData.buttonIndex, empty.transform, buttonData.param, buttonData.obj, buttonData.opCode);
        }
        private void RefreshHuTileView(ValueType value)
        {
            HuTileReturnData huTileReturnData = (HuTileReturnData)value;
            ushort index = huTileReturnData.huTileId;
            int m = goCPGArea[index].transform.childCount;
            for (int i = m - 1; i >= 0; --i)
            {
                int n = goCPGArea[index].transform.GetChild(i).childCount;
                for (int j = n - 1; j >= 0; --j)
                    GameObjectManager.Instance.CollectGO(goCPGArea[index].transform.GetChild(i).GetChild(j).gameObject);
                GameObjectManager.Instance.CollectGO(goCPGArea[index].transform.GetChild(i).gameObject);
            }
            if (huTileReturnData.huTileCnt == 0)
                return;

            ushort[] handTile = huTileReturnData.huTile;
            int len = handTile.Length;
            int childCnt = goCPGArea[index].transform.childCount;
            for (int i = 0; i < handTile.Length; ++i)
            {
                ushort tileNum = handTile[i];
                GameObject tile = GetTile(tileNum, index, false);
                tile.transform.SetParent(goCPGArea[index].transform, false);
                if (index == 0)
                    tile.transform.localPosition = new Vector3((len - 1 - childCnt) * 70, 0, 0);
                else if(index == 1)
                    tile.transform.localPosition = new Vector3(0, (len - 1 - childCnt) * 50, 0);
                else if (index == 2)
                    tile.transform.localPosition = new Vector3((childCnt - 1) * (-70), 0, 0);
                else if (index == 3)
                    tile.transform.localPosition = new Vector3(0, (childCnt - 1) * (-50), 0);
            }
        }
        private void RefreshPengGangView(ValueType value)
        {
            PengGangReturnData pengGangReturnData = (PengGangReturnData)value;
            ushort index = pengGangReturnData.pengGangId;
            if(pengGangReturnData.pengGangTileNum == 0)
            {
                int m = goCPGArea[index].transform.childCount;
                for (int i = m - 1; i >= 0; --i)
                {
                    int n = goCPGArea[index].transform.GetChild(i).childCount;
                    for (int j = n - 1; j >= 0; --j)
                        GameObjectManager.Instance.CollectGO(goCPGArea[index].transform.GetChild(i).GetChild(j).gameObject);
                    GameObjectManager.Instance.CollectGO(goCPGArea[index].transform.GetChild(i).gameObject);
                }
                return;
            }
            
            // 碰，明杠，暗杠，加杠
            PengGangType type = pengGangReturnData.pengGangType;
            // 1. 设置碰杠牌的父物体，并且添加三张碰牌
            GameObject empty = null;
            if(type.Equals(PengGangType.jiagang))
            {
                for (int i = 0; i < goCPGArea[index].transform.childCount; ++i)
                {
                    GameObject goEmpty = goCPGArea[index].transform.GetChild(i).gameObject;
                    string tileName = goEmpty.transform.GetChild(0).name;
                    if (((TileTypeTileNum)pengGangReturnData.pengGangTileNum).ToString() == tileName)
                    {
                        empty = goEmpty;
                        break;
                    }
                }
            }
            else
            {
                empty = GameObjectManager.Instance.GetGO("Empty");
                Transform parent = goCPGArea[index].transform;
                empty.transform.SetParent(parent, false);
                int num = parent.childCount - 1;
                empty.transform.localPosition = new Vector3(CPG_INTERVAL[index, 0] * num, CPG_INTERVAL[index, 1] * num, 0);
                for(int i = 0; i < 3; ++i)
                {
                    GameObject tile = GetTile(pengGangReturnData.pengGangTileNum, index, false);
                    tile.transform.SetParent(empty.transform, false);
                    if(index == 1)
                    { 
                        num = 3 - empty.transform.childCount;
                        tile.transform.SetSiblingIndex(0);
                    }
                    else
                        num = empty.transform.childCount;
                    tile.transform.localPosition = new Vector3
                        (PENG_TILE_INTERVAL[index, 0] * num, PENG_TILE_INTERVAL[index, 1] * num, 0);

                }
            }
            // 2. 不是碰牌则添加一张牌
            if(!type.Equals(PengGangType.peng))
            {
                GameObject tile = GetTile(pengGangReturnData.pengGangTileNum, index, false);
                tile.transform.SetParent(empty.transform, false);
                tile.transform.localPosition = new Vector3(GANG_TILE_POS[index, 0], GANG_TILE_POS[index, 1], 0);
            }
            
        }


        private void CreateButton(ushort buttonIndex, Transform parent, short param, object obj, short opcode)
        {
            GameObject go = GameObjectManager.Instance.GetGO(buttonsName[buttonIndex]);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(0, (parent.childCount - 1) * 120, -10);
            if(!go.TryGetComponent(out EventTrigger eventTrigger))
            {
                eventTrigger = go.AddComponent<EventTrigger>();
                Util.AddEventTriggerListener(ref eventTrigger, EventTriggerType.PointerClick, (BaseEventData data) =>
                {
                    // 1. 向服务器请求出牌数据回复
                    Action<short, object, short> action = controller.GetMethod("AddButtonCtrl") as Action<short, object, short>;
                    action(param, obj, opcode);
                    // 2.删除所有按钮
                    ClientDataManager.Instance.RefreshData("ButtonData", new ButtonData(), DataEvent.ChangeButtonData);
                });
            }
        }
        private GameObject GetTile(int tileNum, int tileIndex, bool isHandTile)
        {
            GameObject go;
            string atlasName, spriteName;
            if (isHandTile)
            {
                go = GameObjectManager.Instance.GetGO("handTile" + playerNames[tileIndex]);
                atlasName = "Room1";
                if (tileIndex == 0)
                    spriteName = ((TileTypeTileNum)tileNum).ToString();
                else
                    spriteName = "handTile" + playerNames[tileIndex];
            }
            else
            {
                go = GameObjectManager.Instance.GetGO("throwTile" + playerNames[tileIndex]);
                if (tileIndex == 1 || tileIndex == 2)
                    go.transform.Rotate(new Vector3(0, 0, 180));
                if (tileIndex == 0 || tileIndex == 2)
                    atlasName = "Room3";
                else
                    atlasName = "Room2";

                spriteName = ((TileTypeTileNum)tileNum).ToString();
            }
            go.GetComponent<SpriteRenderer>().sprite = ResourceManager.Instance.LoadSprite(atlasName, spriteName);

            return go;
        }
    }
}
 
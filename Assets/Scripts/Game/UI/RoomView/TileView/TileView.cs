using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XH;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

namespace mahjong
{
    class TileView : View
    {
        private string[] playerNames =
        {
            "own",
            "down",
            "opposite",
            "up"
        };
        private GameObject[] goHandTile = new GameObject[4];
        private GameObject[] goDrawTile = new GameObject[4];
        private GameObject[] goThrowTileLine1 = new GameObject[4];
        private GameObject[] goThrowTileLine2 = new GameObject[4];      
        private GameObject[] goTipArea = new GameObject[4];
        private GameObject goArrow;
        private GameObject dice;

        private float tileMoveSpeed = 10.0f;
        private float[,] handTileInternal =
        {
            { -115,   0 },
            {    0, -40 },
            {  70,   0 },
            {    0,  40 }
        };
        private float[,] throwTileInternal = 
        {
            { 70,   0 },
            {  0,  50 },
            {-70,   0 },
            {  0, -50 }
        };
        private string[] effectsName;

        public override void Init(string name)
        {
            base.Init(name);

            for (int i = 0; i < playerNames.Length; ++i)
            {
                goHandTile[i] = transform.Find(playerNames[i] + "/goHandTile").gameObject;
                goDrawTile[i] = transform.Find(playerNames[i] + "/goDrawTile").gameObject;
                goThrowTileLine1[i] = transform.Find(playerNames[i] + "/goThrowTileLine1").gameObject;
                goThrowTileLine2[i] = transform.Find(playerNames[i] + "/goThrowTileLine2").gameObject;
                goTipArea[i] = transform.Find(playerNames[i] + "/goTipArea").gameObject;
            }
            goArrow = transform.Find("goArrow").gameObject;
            goArrow.transform.localPosition = new Vector3(1200, 0, 0);
            dice = transform.Find("dice").gameObject;

            for (int i = 0; i < 50; ++i)
            {
                GameObject handTile = GameObjectManager.Instance.GetGO("HandTile");
                GameObject throwTile = GameObjectManager.Instance.GetGO("ThrowTile");
                for (int j = 0; j < 4; ++j)
                {
                    GameObject goHandTile = handTile.transform.GetChild(0).gameObject;
                    GameObjectManager.Instance.AddGo("handTile" + goHandTile.name, goHandTile);

                    GameObject goThrowTile = throwTile.transform.GetChild(0).gameObject;
                    GameObjectManager.Instance.AddGo("throwTile" + goThrowTile.name, goThrowTile);
                }
                GameObject.DestroyImmediate(handTile);
                GameObject.DestroyImmediate(throwTile);
            }
          
            for(int i = 0; i < 5; ++i)
            {
                GameObject effects = GameObjectManager.Instance.GetGO("Effects");
                int n = effects.transform.childCount;
                effectsName = new string[n];
                for (int j = 0; j < n; ++j)
                {
                    GameObject go = effects.transform.GetChild(0).gameObject;
                    effectsName[j] = go.name;
                    GameObjectManager.Instance.AddGo(go.name, go);
                }
                GameObject.DestroyImmediate(effects);
            }

            EventManager.Instance.AddListener(UIEvent.ShowDiceAnim, (bool isPlay) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    ShowDiceView(isPlay);
                });
            });
            EventManager.Instance.AddListener(UIEvent.RefreshHandTileView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    RefreshHandTileView(value);
                });
            });
            EventManager.Instance.AddListener(UIEvent.MoveHandTile, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    GameManager.Instance.StartCoroutine(IEHandTileMove(value));
                });
            });
            EventManager.Instance.AddListener(UIEvent.RefreshDrawTileView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    RefreshDrawTileView(value);
                });
            });
            EventManager.Instance.AddListener(UIEvent.RefreshThrowTileView, (ValueType value) =>
            { 
                ThreadManager.ExecuteUpdate(() => { RefreshThrowTileView(value); }); 
            });
            EventManager.Instance.AddListener(UIEvent.RemoveThrowTile_1, () =>
            {
                ThreadManager.ExecuteUpdate(() => { RemoveThrowTile_1(); });
            });
            EventManager.Instance.AddListener(UIEvent.RefreshTipView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    GameManager.Instance.StartCoroutine(IEShowTip(value));
                });
            });
        }

        public override void UnInit()
        {
            base.UnInit();

            EventManager.Instance.RemoveListener(UIEvent.ShowDiceAnim, (bool isPlay) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    ShowDiceView(isPlay);
                });
            });
            EventManager.Instance.RemoveListener(UIEvent.RefreshHandTileView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    RefreshHandTileView(value);
                });
            });
            EventManager.Instance.RemoveListener(UIEvent.MoveHandTile, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    GameManager.Instance.StartCoroutine(IEHandTileMove(value));
                });
            });
            EventManager.Instance.RemoveListener(UIEvent.RefreshDrawTileView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    RefreshDrawTileView(value);
                });
            });
            EventManager.Instance.RemoveListener(UIEvent.RefreshThrowTileView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() => { RefreshThrowTileView(value); });
            });
            EventManager.Instance.RemoveListener(UIEvent.RemoveThrowTile_1, () =>
            {
                ThreadManager.ExecuteUpdate(() => { RemoveThrowTile_1(); });
            });
            EventManager.Instance.RemoveListener(UIEvent.RefreshTipView, (ValueType value) =>
            {
                ThreadManager.ExecuteUpdate(() =>
                {
                    GameManager.Instance.StartCoroutine(IEShowTip(value));
                });
            });
        }

        private void ShowDiceView(bool isPlay)
        {
            if(!isPlay)
            {
                dice.SetActive(false);
                return;
            }
            dice.SetActive(true);
            Animator animator = dice.GetComponent<Animator>();
            animator.Play("diceAnim", 0, 0f);
        }
        private IEnumerator IEHandTileMove(ValueType value)
        {
            // 1.获取摸牌数字，摸牌的位置 
            Transform drawTileTrans = null;
            ushort drawTileNum = ((DrawTileData)value).drawTileNum;
            if (goDrawTile[0].transform.childCount > 0)
            { 
                drawTileTrans = goDrawTile[0].transform.GetChild(0);
                DingQueData dingQueData = (DingQueData)ClientDataManager.Instance.GetData("DingQueData");
                Action<GameObject, ushort, bool, TileType> action = controller.GetMethod("InitTile")
                   as Action<GameObject, ushort, bool, TileType>;
                action(drawTileTrans.gameObject, drawTileNum, false, dingQueData.queTileType);
            }
            // 2. 获取手牌并从大到小排序
            HandTileData handTileData = (HandTileData)ClientDataManager.Instance.GetData("HandTileData");
            List<ushort> tiles = handTileData.handTile.ToList();
            tiles.RemoveAll(x => x == 0);
            tiles.Sort((x, y) => { return x > y ? -1 : 1; });
            // 3.指定手牌位置
            int drawTileIndex = tiles.IndexOf(drawTileNum);
            for (int i = 0; i < goHandTile[0].transform.childCount; ++i)
            {
                int handTileIndex = i >= drawTileIndex ? (i + 1) : i;
                goHandTile[0].transform.GetChild(i).transform.localPosition = new Vector3(-115 * handTileIndex, 0, 0);
            }

            yield return new WaitForSeconds(0.5f);

            // 4.指定摸牌位置
            if (drawTileTrans != null)
            {
                drawTileTrans.SetParent(goHandTile[0].transform, false);
                drawTileTrans.SetSiblingIndex(drawTileIndex);
                Vector3 lastPos = new Vector3(-115 * drawTileIndex, 0, 0);
                drawTileTrans.localPosition += new Vector3(160, 0, 0);
                Vector3 secondPos = lastPos + new Vector3(160, 0, 0);
                GameManager.Instance.StartCoroutine(IEDrawTileMove(drawTileTrans, secondPos, lastPos));
            }
        }
        private void RefreshHandTileView(ValueType value)
        {
            HandTileData handTileData = (HandTileData)value;
            ushort index = handTileData.handTileId;
            // 1. 删除手牌
            int n = goHandTile[index].transform.childCount;
            for (int i = n - 1; i >= 0; --i)
                GameObjectManager.Instance.CollectGO(goHandTile[index].transform.GetChild(i).gameObject);
            if (handTileData.tileCnt == 0)
                return;
            // 2. 生成手牌
            ushort[] handTile = handTileData.handTile;
            Array.Sort(handTile);
            Array.Reverse(handTile);
            DingQueData dingQueData = (DingQueData)ClientDataManager.Instance.GetData("DingQueData");
            ushort cnt = (ushort)(index == 0 ? handTileData.tileCnt : Math.Abs(n - handTileData.tileCnt));
            for (int i = 0; i < cnt; ++i)
            {
                ushort tileNum = (ushort)(index == 0 ? handTile[i] : 0); 
                GameObject tile = GetTile(tileNum, index, true);
                tile.transform.SetParent(goHandTile[index].transform, false);
                tile.transform.localPosition = new Vector3
                    (i * handTileInternal[index, 0], i * handTileInternal[index, 1], -10);
                if(index == 0)
                {
                    Action<GameObject, ushort, bool, TileType> action = controller.GetMethod("InitTile") 
                        as Action<GameObject, ushort, bool, TileType>;
                    action(tile, tileNum, false, dingQueData.queTileType);
                }
            }
        }
        private void RefreshDrawTileView(ValueType value)
        {
            DrawTileData drawTileData = (DrawTileData)value;
            ushort index = drawTileData.drawTileId;
            if (goDrawTile[index].transform.childCount > 0)
                GameObjectManager.Instance.CollectGO(goDrawTile[index].transform.GetChild(0).gameObject);
            if (drawTileData.drawTileNum == 0)
                return;
            GameObject drawTile = GetTile(drawTileData.drawTileNum, index, true);
            drawTile.transform.SetParent(goDrawTile[index].transform, false);
            drawTile.transform.localPosition = Vector3.zero;
            DingQueData dingQueData = (DingQueData)ClientDataManager.Instance.GetData("DingQueData");
            if (index == 0)
            {
                Action<GameObject, ushort, bool, TileType> action = controller.GetMethod("InitTile") 
                    as Action<GameObject, ushort, bool, TileType>;
                action(drawTile, drawTileData.drawTileNum, true, dingQueData.queTileType);
            }
        }
        private void RemoveThrowTile_1()
        {
            ushort index = ((PlayTileData)ClientDataManager.Instance.GetData("PlayTileData")).playTileIndex;
            Transform line1 = goThrowTileLine1[index].transform;
            Transform line2 = goThrowTileLine2[index].transform;
            int n = line2.childCount - 1;
            if(n >= 0)
            { 
                GameObjectManager.Instance.CollectGO(line2.GetChild(n).gameObject);
                return;
            }
            n = line1.childCount - 1;
            if (n >= 0)
            {
                GameObjectManager.Instance.CollectGO(line1.GetChild(n).gameObject);
                return;
            }
        }
        private void RefreshThrowTileView(ValueType value)
        {
            ThrowTileData throwTileData = (ThrowTileData)value;
            ushort num = throwTileData.throwTileNum;
            ushort index = throwTileData.throwTileId;
            Transform line1 = goThrowTileLine1[index].transform;
            Transform line2 = goThrowTileLine2[index].transform;
            if (num == 0)
            {
                for (int i = line1.childCount - 1; i >= 0; --i)
                {
                    GameObjectManager.Instance.CollectGO(line1.GetChild(i).gameObject);
                }
                for (int i = line2.childCount - 1; i >= 0; --i)
                {
                    GameObjectManager.Instance.CollectGO(line2.GetChild(i).gameObject);
                }

                return;
            }
            // 1.生成抛牌
            GameObject throwTile = GetTile(num, index, false);
            float x = 0, y = 0;
            if (line1.childCount < 12)
            {
                throwTile.transform.SetParent(line1, false);
                x = throwTileInternal[index, 0] * (line1.childCount - 1);
                y = throwTileInternal[index, 1] * (line1.childCount - 1);
            }
            else if (line2.childCount < 12)
            {
                throwTile.transform.SetParent(line2, false);
                x = throwTileInternal[index, 0] * (line2.childCount - 1);
                y = throwTileInternal[index, 1] * (line2.childCount - 1);
            }
            throwTile.transform.localPosition = new Vector3(x, y, -10);
            // 2.显示提示箭头
            goArrow.transform.SetParent(throwTile.transform, false);
            goArrow.transform.localPosition = new Vector3(0, 30, 0);
        }
        private IEnumerator IEShowTip(ValueType value)
        {
            TipData tipData = (TipData)value;
            bool isTile = tipData.isTile;
            ushort tipNum = tipData.tipNum;
            ushort tipIndex = tipData.tipId;
            bool isOnce = tipData.isOnce;
            GameObject tip;
            if(isTile)
                tip = GetTile(tipNum, 0, true);
            else
                tip = GameObjectManager.Instance.GetGO(effectsName[tipNum]);
            tip.transform.SetParent(goTipArea[tipIndex].transform, false);
            tip.transform.localPosition = new Vector3(0, 0, -10);
            Vector2 rect = tip.GetComponent<RectTransform>().sizeDelta;
            tip.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 200);

            yield return new WaitForSeconds(2.0f);

            tip.GetComponent<RectTransform>().sizeDelta = rect;
            if(isOnce)
                GameObjectManager.Instance.CollectGO(tip);
        }


        private GameObject GetTile(int tileNum, int tileIndex, bool isHandTile)
        {
            GameObject go;
            string atlasName, spriteName;
            if(isHandTile)
            { 
                go = GameObjectManager.Instance.GetGO("handTile" + playerNames[tileIndex]);
                atlasName = "Room1";
                if(tileIndex == 0)
                   spriteName = ((TileTypeTileNum)tileNum).ToString();
                else
                    spriteName = "handTile" + playerNames[tileIndex];
            }
            else
            { 
                go = GameObjectManager.Instance.GetGO("throwTile" + playerNames[tileIndex]);
                if(tileIndex == 1 || tileIndex == 2)
                    go.transform.Rotate(new Vector3(0, 0, 180));
                if(tileIndex == 0 || tileIndex == 2)
                    atlasName = "Room3";
                else
                    atlasName = "Room2";

                spriteName = ((TileTypeTileNum)tileNum).ToString();
            }
            go.GetComponent<SpriteRenderer>().sprite = ResourceManager.Instance.LoadSprite(atlasName, spriteName);
            return go;
        }
        private IEnumerator IEDrawTileMove(Transform trans, Vector3 secondPos, Vector3 lastPos)
        {
            // 1.计算50个差值的位置， 每0.01f移动一次
            for (int i = 0; i < 50; ++i)
            {
                trans.localPosition = Vector2.Lerp(trans.localPosition, secondPos, tileMoveSpeed * Time.deltaTime);
                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(0.2f);
            // 2. 0.2f后将摸牌移动到最后的位置
            trans.localPosition = lastPos;
        }
        
    }
}
